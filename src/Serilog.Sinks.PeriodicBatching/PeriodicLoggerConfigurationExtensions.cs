// Copyright 2013-2016 Serilog Contributors
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

using System;
using Serilog.Configuration;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog
{
    public static class PeriodicLoggerConfigurationExtensions
    {
        public static LoggerSinkConfiguration WriteAsyncTo(this LoggerConfiguration loggerConfiguration, TimeSpan period)
        {
            var sinkConfiguration = loggerConfiguration.WriteTo;

            // TODO: maxBatchSize, inheritedConfiguration
            return new LoggerSinkConfiguration(loggerConfiguration, sink => sinkConfiguration.Sink(new DelegatingPeriodicBatchingSink(sink, 100, period)), lc => {});
        }
    }
}