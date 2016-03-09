// Copyright 2013-2015 Serilog Contributors
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
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.PeriodicBatching
{
    class DelegatingPeriodicBatchingSink : PeriodicBatchingSink
    {
        readonly ILogEventSink _sink;

        public DelegatingPeriodicBatchingSink(ILogEventSink sink, int batchSizeLimit, TimeSpan period) : base(batchSizeLimit, period)
        {
            if (sink == null) throw new ArgumentNullException(nameof(sink));

            _sink = sink;
        }

        protected override Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            foreach (var logEvent in events)
            {
                _sink.Emit(logEvent);
            }

            return Task.FromResult(0);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            var disposable = _sink as IDisposable;
            disposable?.Dispose();
        }
    }
}