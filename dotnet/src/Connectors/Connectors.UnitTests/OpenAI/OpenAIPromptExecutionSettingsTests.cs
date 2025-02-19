﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Xunit;

namespace SemanticKernel.Connectors.UnitTests.OpenAI;

/// <summary>
/// Unit tests of OpenAIPromptExecutionSettings
/// </summary>
public class OpenAIPromptExecutionSettingsTests
{
    [Fact]
    public void ItCreatesOpenAIRequestSettingsWithCorrectDefaults()
    {
        // Arrange
        // Act
        OpenAIPromptExecutionSettings executionSettings = OpenAIPromptExecutionSettings.FromRequestSettings(null, 128);

        // Assert
        Assert.NotNull(executionSettings);
        Assert.Equal(0, executionSettings.Temperature);
        Assert.Equal(0, executionSettings.TopP);
        Assert.Equal(0, executionSettings.FrequencyPenalty);
        Assert.Equal(0, executionSettings.PresencePenalty);
        Assert.Equal(1, executionSettings.ResultsPerPrompt);
        Assert.Equal(Array.Empty<string>(), executionSettings.StopSequences);
        Assert.Equal(new Dictionary<int, int>(), executionSettings.TokenSelectionBiases);
        Assert.Null(executionSettings.ServiceId);
        Assert.Equal(128, executionSettings.MaxTokens);
    }

    [Fact]
    public void ItUsesExistingOpenAIRequestSettings()
    {
        // Arrange
        OpenAIPromptExecutionSettings actualSettings = new()
        {
            Temperature = 0.7,
            TopP = 0.7,
            FrequencyPenalty = 0.7,
            PresencePenalty = 0.7,
            ResultsPerPrompt = 2,
            StopSequences = new string[] { "foo", "bar" },
            ChatSystemPrompt = "chat system prompt",
            MaxTokens = 128,
            ServiceId = "service",
            TokenSelectionBiases = new Dictionary<int, int>() { { 1, 2 }, { 3, 4 } },
        };

        // Act
        OpenAIPromptExecutionSettings executionSettings = OpenAIPromptExecutionSettings.FromRequestSettings(actualSettings);

        // Assert
        Assert.NotNull(executionSettings);
        Assert.Equal(actualSettings, executionSettings);
    }

    [Fact]
    public void ItCanUseOpenAIRequestSettings()
    {
        // Arrange
        PromptExecutionSettings actualSettings = new()
        {
            ServiceId = "service",
        };

        // Act
        OpenAIPromptExecutionSettings executionSettings = OpenAIPromptExecutionSettings.FromRequestSettings(actualSettings, null);

        // Assert
        Assert.NotNull(executionSettings);
        Assert.Equal(actualSettings.ServiceId, executionSettings.ServiceId);
    }

    [Fact]
    public void ItCreatesOpenAIRequestSettingsFromExtraPropertiesSnakeCase()
    {
        // Arrange
        PromptExecutionSettings actualSettings = new()
        {
            ServiceId = "service",
            ExtensionData = new Dictionary<string, object>()
            {
                { "temperature", 0.7 },
                { "top_p", 0.7 },
                { "frequency_penalty", 0.7 },
                { "presence_penalty", 0.7 },
                { "results_per_prompt", 2 },
                { "stop_sequences", new [] { "foo", "bar" } },
                { "chat_system_prompt", "chat system prompt" },
                { "max_tokens", 128 },
                { "service_id", "service" },
                { "token_selection_biases", new Dictionary<int, int>() { { 1, 2 }, { 3, 4 } } }
            }
        };

        // Act
        OpenAIPromptExecutionSettings executionSettings = OpenAIPromptExecutionSettings.FromRequestSettings(actualSettings, null);

        // Assert
        AssertRequestSettings(executionSettings);
    }

    [Fact]
    public void ItCreatesOpenAIRequestSettingsFromExtraPropertiesPascalCase()
    {
        // Arrange
        PromptExecutionSettings actualSettings = new()
        {
            ServiceId = "service",
            ExtensionData = new Dictionary<string, object>()
            {
                { "Temperature", 0.7 },
                { "TopP", 0.7 },
                { "FrequencyPenalty", 0.7 },
                { "PresencePenalty", 0.7 },
                { "ResultsPerPrompt", 2 },
                { "StopSequences", new[] { "foo", "bar" } },
                { "ChatSystemPrompt", "chat system prompt" },
                { "MaxTokens", 128 },
                { "ServiceId", "service" },
                { "TokenSelectionBiases", new Dictionary<int, int>() { { 1, 2 }, { 3, 4 } } }
            }
        };

        // Act
        OpenAIPromptExecutionSettings executionSettings = OpenAIPromptExecutionSettings.FromRequestSettings(actualSettings);

        // Assert
        AssertRequestSettings(executionSettings);
    }

    [Fact]
    public void ItCreatesOpenAIRequestSettingsFromJsonSnakeCase()
    {
        // Arrange
        var json = @"{
  ""temperature"": 0.7,
  ""top_p"": 0.7,
  ""frequency_penalty"": 0.7,
  ""presence_penalty"": 0.7,
  ""results_per_prompt"": 2,
  ""stop_sequences"": [ ""foo"", ""bar"" ],
  ""chat_system_prompt"": ""chat system prompt"",
  ""token_selection_biases"": { ""1"": 2, ""3"": 4 },
  ""service_id"": ""service"",
  ""max_tokens"": 128
}";
        var actualSettings = JsonSerializer.Deserialize<PromptExecutionSettings>(json);

        // Act
        OpenAIPromptExecutionSettings executionSettings = OpenAIPromptExecutionSettings.FromRequestSettings(actualSettings);

        // Assert
        AssertRequestSettings(executionSettings);
    }

    [Fact]
    public void ItCreatesOpenAIRequestSettingsFromJsonPascalCase()
    {
        // Arrange
        var json = @"{
  ""Temperature"": 0.7,
  ""TopP"": 0.7,
  ""FrequencyPenalty"": 0.7,
  ""PresencePenalty"": 0.7,
  ""ResultsPerPrompt"": 2,
  ""StopSequences"": [ ""foo"", ""bar"" ],
  ""ChatSystemPrompt"": ""chat system prompt"",
  ""TokenSelectionBiases"": { ""1"": 2, ""3"": 4 },
  ""ServiceId"": ""service"",
  ""MaxTokens"": 128
}";
        var actualSettings = JsonSerializer.Deserialize<PromptExecutionSettings>(json);

        // Act
        OpenAIPromptExecutionSettings executionSettings = OpenAIPromptExecutionSettings.FromRequestSettings(actualSettings);

        // Assert
        AssertRequestSettings(executionSettings);
    }

    private static void AssertRequestSettings(OpenAIPromptExecutionSettings executionSettings)
    {
        Assert.NotNull(executionSettings);
        Assert.Equal(0.7, executionSettings.Temperature);
        Assert.Equal(0.7, executionSettings.TopP);
        Assert.Equal(0.7, executionSettings.FrequencyPenalty);
        Assert.Equal(0.7, executionSettings.PresencePenalty);
        Assert.Equal(2, executionSettings.ResultsPerPrompt);
        Assert.Equal(new string[] { "foo", "bar" }, executionSettings.StopSequences);
        Assert.Equal("chat system prompt", executionSettings.ChatSystemPrompt);
        Assert.Equal(new Dictionary<int, int>() { { 1, 2 }, { 3, 4 } }, executionSettings.TokenSelectionBiases);
        Assert.Equal("service", executionSettings.ServiceId);
        Assert.Equal(128, executionSettings.MaxTokens);
    }
}
