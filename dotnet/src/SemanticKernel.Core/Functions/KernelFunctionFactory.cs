﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.AI;

#pragma warning disable IDE0130

namespace Microsoft.SemanticKernel;

/// <summary>
/// Provides factory methods for creating commonly-used implementations of <see cref="KernelFunction"/>, such as
/// those backed by a prompt to be submitted to an LLM or those backed by a .NET method.
/// </summary>
public static class KernelFunctionFactory
{
    #region FromMethod
    /// <summary>
    /// Creates an <see cref="KernelFunction"/> instance for a method, specified via a delegate.
    /// </summary>
    /// <param name="method">The method to be represented via the created <see cref="KernelFunction"/>.</param>
    /// <param name="functionName">Optional function name. If null, it will default to one derived from the method represented by <paramref name="method"/>.</param>
    /// <param name="description">Optional description of the method. If null, it will default to one derived from the method represented by <paramref name="method"/>, if possible (e.g. via a <see cref="DescriptionAttribute"/> on the method).</param>
    /// <param name="parameters">Optional parameter descriptions. If null, it will default to one derived from the method represented by <paramref name="method"/>.</param>
    /// <param name="returnParameter">Optional return parameter description. If null, it will default to one derived from the method represented by <paramref name="method"/>.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to use for logging. If null, no logging will be performed.</param>
    /// <returns>The created <see cref="KernelFunction"/> wrapper for <paramref name="method"/>.</returns>
    public static KernelFunction CreateFromMethod(
        Delegate method,
        string? functionName = null,
        string? description = null,
        IEnumerable<KernelParameterMetadata>? parameters = null,
        KernelReturnParameterMetadata? returnParameter = null,
        ILoggerFactory? loggerFactory = null) =>
        CreateFromMethod(method.Method, method.Target, functionName, description, parameters, returnParameter, loggerFactory);

    /// <summary>
    /// Creates an <see cref="KernelFunction"/> instance for a method, specified via an <see cref="MethodInfo"/> instance
    /// and an optional target object if the method is an instance method.
    /// </summary>
    /// <param name="method">The method to be represented via the created <see cref="KernelFunction"/>.</param>
    /// <param name="target">The target object for the <paramref name="method"/> if it represents an instance method. This should be null if and only if <paramref name="method"/> is a static method.</param>
    /// <param name="functionName">Optional function name. If null, it will default to one derived from the method represented by <paramref name="method"/>.</param>
    /// <param name="description">Optional description of the method. If null, it will default to one derived from the method represented by <paramref name="method"/>, if possible (e.g. via a <see cref="DescriptionAttribute"/> on the method).</param>
    /// <param name="parameters">Optional parameter descriptions. If null, it will default to one derived from the method represented by <paramref name="method"/>.</param>
    /// <param name="returnParameter">Optional return parameter description. If null, it will default to one derived from the method represented by <paramref name="method"/>.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to use for logging. If null, no logging will be performed.</param>
    /// <returns>The created <see cref="KernelFunction"/> wrapper for <paramref name="method"/>.</returns>
    public static KernelFunction CreateFromMethod(
        MethodInfo method,
        object? target = null,
        string? functionName = null,
        string? description = null,
        IEnumerable<KernelParameterMetadata>? parameters = null,
        KernelReturnParameterMetadata? returnParameter = null,
        ILoggerFactory? loggerFactory = null) =>
        KernelFunctionFromMethod.Create(method, target, functionName, description, parameters, returnParameter, loggerFactory);
    #endregion

    #region FromPrompt
    // TODO: Revise these Create method XML comments

    /// <summary>
    /// Creates a semantic function passing in the definition in natural language, i.e. the prompt template.
    /// </summary>
    /// <param name="promptTemplate">Plain language definition of the semantic function, using SK template language</param>
    /// <param name="executionSettings">Optional LLM request settings</param>
    /// <param name="functionName">A name for the given function. The name can be referenced in templates and used by the pipeline planner.</param>
    /// <param name="description">Optional description, useful for the planner</param>
    /// <param name="promptTemplateFactory">Optional: Prompt template factory</param>
    /// <param name="loggerFactory">Logger factory</param>
    /// <returns>A function ready to use</returns>
    public static KernelFunction CreateFromPrompt(
        string promptTemplate,
        PromptExecutionSettings? executionSettings = null,
        string? functionName = null,
        string? description = null,
        IPromptTemplateFactory? promptTemplateFactory = null,
        ILoggerFactory? loggerFactory = null) =>
        KernelFunctionFromPrompt.Create(promptTemplate, executionSettings, functionName, description, promptTemplateFactory, loggerFactory);

    /// <summary>
    /// Create a semantic function instance, given a prompt function model.
    /// </summary>
    /// <param name="promptConfig">Prompt configuration model</param>
    /// <param name="promptTemplateFactory">Prompt template factory</param>
    /// <param name="loggerFactory">Logger factory</param>
    /// <returns>A function ready to use</returns>
    public static KernelFunction CreateFromPrompt(
        PromptTemplateConfig promptConfig,
        IPromptTemplateFactory? promptTemplateFactory = null,
        ILoggerFactory? loggerFactory = null) => KernelFunctionFromPrompt.Create(promptConfig, promptTemplateFactory, loggerFactory);

    /// <summary>
    /// Create a semantic function instance, given a prompt function model.
    /// </summary>
    /// <param name="promptTemplate">Prompt template</param>
    /// <param name="promptConfig">Prompt configuration model</param>
    /// <param name="loggerFactory">Logger factory</param>
    /// <returns>A function ready to use</returns>
    public static KernelFunction CreateFromPrompt(
        IPromptTemplate promptTemplate,
        PromptTemplateConfig promptConfig,
        ILoggerFactory? loggerFactory = null) => KernelFunctionFromPrompt.Create(promptTemplate, promptConfig, loggerFactory);
    #endregion
}
