// <copyright file="StrongNamedTests.cs" company="OpenTelemetry Authors">
// Copyright The OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

#if NETFRAMEWORK
using System.Reflection;
using FluentAssertions;
#endif
using IntegrationTests.Helpers;
using Xunit.Abstractions;

namespace IntegrationTests;

public class StrongNamedTests : TestHelper
{
    public StrongNamedTests(ITestOutputHelper output)
        : base("StrongNamed", output)
    {
    }

    [Fact]
    public void SubmitsTraces()
    {
        using var collector = new MockSpansCollector(Output);
        SetExporter(collector);
        collector.Expect("ByteCode.Plugin.StrongNamedValidation");

        EnableBytecodeInstrumentation();
        SetEnvironmentVariable("OTEL_DOTNET_AUTO_TRACES_ADDITIONAL_SOURCES", "ByteCode.Plugin.StrongNamedValidation");
        SetEnvironmentVariable("OTEL_DOTNET_AUTO_PLUGINS", "TestLibrary.InstrumentationTarget.Plugin, TestLibrary.InstrumentationTarget, Version=1.0.0.0, Culture=neutral, PublicKeyToken=0223b52cbfd4bd5b");
        RunTestApplication();

        // TODO: When native logs are moved to an EventSource implementation check for the log
        // TODO: entries reporting the missing instrumentation type and missing instrumentation methods.
        // TODO: See https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/issues/960

        collector.AssertExpectations();
    }

#if NETFRAMEWORK
    [Fact]
    public void VerifyIfApplicationHasStrongName()
    {
        var testApplicationPath = EnvironmentHelper.GetTestApplicationPath();

        var assembly = Assembly.ReflectionOnlyLoadFrom(testApplicationPath);

        BitConverter.ToString(assembly.GetName().GetPublicKeyToken()).Replace("-", string.Empty).ToLowerInvariant().Should().Be("0223b52cbfd4bd5b");
    }
#endif

}
