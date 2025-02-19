﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

/**
 * This example shows different ways how to define and execute native functions using custom and primitive types.
 */
// ReSharper disable once InconsistentNaming
public static class Example60_AdvancedNativeFunctions
{
    public static async Task RunAsync()
    {
        await NativeFunctionsChainingAsync();
    }

    #region Native Functions Chaining

    /// <summary>
    /// This example executes Function1, which in turn executes Function2.
    /// </summary>
    private static async Task NativeFunctionsChainingAsync()
    {
        Console.WriteLine("Running Native Function Chaining example...");

        var kernel = new KernelBuilder().Build();

        var functions = kernel.ImportPluginFromObject<FunctionsChainingPlugin>();

        var result = await kernel.InvokeAsync(functions["Function1"]);
        var customType = result.GetValue<MyCustomType>()!;

        Console.WriteLine(customType.Number); // 2
        Console.WriteLine(customType.Text); // From Function1 + From Function2
    }

    /// <summary>
    /// Plugin example with two native functions, where one function is called from another.
    /// </summary>
    private sealed class FunctionsChainingPlugin
    {
        public const string PluginName = nameof(FunctionsChainingPlugin);

        [KernelFunction, KernelName("Function1")]
        public async Task<MyCustomType> Function1Async(Kernel kernel)
        {
            // Execute another function
            var result = await kernel.InvokeAsync(PluginName, "Function2");
            var value = result?.GetValue<MyCustomType>()!;

            return new MyCustomType
            {
                Number = 2 * value?.Number ?? 0,
                Text = "From Function1 + " + value?.Text
            };
        }

        [KernelFunction, KernelName("Function2")]
        public static MyCustomType Function2()
        {
            return new MyCustomType
            {
                Number = 1,
                Text = "From Function2"
            };
        }
    }

    #endregion

    #region Custom Type

    /// <summary>
    /// In order to use custom types, <see cref="TypeConverter"/> should be specified,
    /// that will convert object instance to string representation.
    /// </summary>
    /// <remarks>
    /// <see cref="TypeConverter"/> is used to represent complex object as meaningful string, so
    /// it can be passed to AI for further processing using semantic functions.
    /// It's possible to choose any format (e.g. XML, JSON, YAML) to represent your object.
    /// </remarks>
    [TypeConverter(typeof(MyCustomTypeConverter))]
    private sealed class MyCustomType
    {
        public int Number { get; set; }

        public string? Text { get; set; }
    }

    /// <summary>
    /// Implementation of <see cref="TypeConverter"/> for <see cref="MyCustomType"/>.
    /// In this example, object instance is serialized with <see cref="JsonSerializer"/> from System.Text.Json,
    /// but it's possible to convert object to string using any other serialization logic.
    /// </summary>
#pragma warning disable CA1812 // instantiated by Kernel
    private sealed class MyCustomTypeConverter : TypeConverter
#pragma warning restore CA1812
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => true;

        /// <summary>
        /// This method is used to convert object from string to actual type. This will allow to pass object to
        /// native function which requires it.
        /// </summary>
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            return JsonSerializer.Deserialize<MyCustomType>((string)value);
        }

        /// <summary>
        /// This method is used to convert actual type to string representation, so it can be passed to AI
        /// for further processing.
        /// </summary>
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            return JsonSerializer.Serialize(value);
        }
    }

    #endregion
}
