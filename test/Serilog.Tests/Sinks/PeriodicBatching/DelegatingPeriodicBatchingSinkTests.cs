using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Serilog.Core;
using Serilog.Events;
using Xunit;

namespace Serilog.Tests.Sinks.PeriodicBatching
{
    public class DelegatingPeriodicBatchingSinkTests
    {
        static readonly TimeSpan HugeWait = TimeSpan.FromMilliseconds(500);
        static readonly TimeSpan SmallWait = TimeSpan.FromMilliseconds(100);
        static readonly TimeSpan TinyWait = TimeSpan.FromMilliseconds(20);

        [Fact]
        public void IndependentSinkFlushing()
        {
            var sink1 = new MockEventSink();
            var sink2 = new MockEventSink();
            var sink3 = new MockEventSink();

            var logger = new LoggerConfiguration()
                .WriteTo.Sink(sink1)
                .WriteAsyncTo(SmallWait).Sink(sink2)
                .WriteAsyncTo(HugeWait).Sink(sink3)
                .CreateLogger();

            logger.Information("Warm-up...");

            Thread.Sleep(TinyWait);

            sink1.Reset();
            sink2.Reset();
            sink3.Reset();

            logger.Information("Hello!");

            Assert.True(sink1.WasCalled);
            Assert.False(sink2.WasCalled);
            Assert.False(sink3.WasCalled);

            Thread.Sleep(SmallWait + TinyWait);

            Assert.True(sink2.WasCalled);
            Assert.False(sink3.WasCalled);

            Thread.Sleep(HugeWait + TinyWait);

            Assert.True(sink3.WasCalled);
        }

        class MockEventSink : ILogEventSink
        {
            readonly List<LogEvent> _events = new List<LogEvent>();

            public bool WasCalled { get { return _events.Any(); } }

            public IReadOnlyList<LogEvent> Events => _events;

            public void Reset()
            {
                _events.Clear();
            }

            public void Emit(LogEvent logEvent)
            {
                _events.Add(logEvent);
            }
        }
    }
}