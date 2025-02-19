﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.Orchestration;

namespace Microsoft.SemanticKernel.Services;

/// <summary>
/// Implementation of <see cref="IAIServiceSelector"/> that selects the AI service based on the order of the model settings.
/// Uses the service id to select the preferred service provider and then returns the service and associated model settings.
/// </summary>
internal sealed class OrderedIAIServiceSelector : IAIServiceSelector
{
    public static OrderedIAIServiceSelector Instance { get; } = new();

    /// <inheritdoc/>
    public (T?, PromptExecutionSettings?) SelectAIService<T>(Kernel kernel, ContextVariables variables, KernelFunction function) where T : class, IAIService
    {
        var executionSettings = function.ExecutionSettings;
        if (executionSettings is null || !executionSettings.Any())
        {
            var service = kernel.Services is IKeyedServiceProvider ?
                kernel.GetAllServices<T>().LastOrDefault() : // see comments in Kernel/KernelBuilder for why we can't use GetKeyedService
                kernel.Services.GetService<T>();
            if (service is not null)
            {
                return (service, null);
            }
        }
        else
        {
            PromptExecutionSettings? defaultRequestSettings = null;
            foreach (var model in executionSettings)
            {
                if (!string.IsNullOrEmpty(model.ServiceId))
                {
                    var service = kernel.Services is IKeyedServiceProvider ?
                        kernel.Services.GetKeyedService<T>(model.ServiceId) :
                        null;
                    if (service is not null)
                    {
                        return (service, model);
                    }
                }
                else if (!string.IsNullOrEmpty(model.ModelId))
                {
                    var service = this.GetServiceByModelId<T>(kernel.Services, model.ModelId!);
                    if (service is not null)
                    {
                        return (service, model);
                    }
                }
                else
                {
                    // First request settings with empty or null service id is the default
                    defaultRequestSettings ??= model;
                }
            }

            if (defaultRequestSettings is not null)
            {
                return (kernel.GetService<T>(), defaultRequestSettings);
            }
        }

        var names = executionSettings is not null ? string.Join("|", executionSettings.Select(model => model.ServiceId).ToArray()) : null;
        throw new KernelException(string.IsNullOrWhiteSpace(names) ?
            $"Service of type {typeof(T)} not registered." :
            $"Service of type {typeof(T)} and names {names} not registered.");
    }

    private T? GetServiceByModelId<T>(IServiceProvider serviceProvider, string modelId) where T : IAIService
    {
        var services = serviceProvider.GetServices<T>();
        foreach (var service in services)
        {
            string? serviceModelId = service.GetModelId();
            if (!string.IsNullOrEmpty(serviceModelId) && serviceModelId == modelId)
            {
                return service;
            }
        }

        return default;
    }
}
