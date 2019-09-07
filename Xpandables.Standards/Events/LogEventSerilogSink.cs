/************************************************************************************************************
 * Copyright (C) 2019 Francis-Black EWANE
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
************************************************************************************************************/

using Serilog.Core;
using Serilog.Events;
using System.Collections.Generic;
using System.Design.Database;
using System.Threading;

namespace System.Design.TaskEvent
{
    /// <summary>
    /// Provides with a custom emitter for the specified log event type.
    /// <para>Using the <see cref="IAsyncDataContext"/> interface to save the log.</para>
    /// </summary>
    /// <typeparam name="T">The type of the log event.</typeparam>
    public sealed class LogEventSerilogSink<T> : ILogEventSink
        where T : Entity, ILogEvent<T>, new()
    {
        private readonly IAsyncDataContext _dataContext;
        private static SpinLock _spinLock = new SpinLock();

        /// <summary>
        /// Initializes the <see cref="LogEventSerilogSink{T}"/> with the  data context and the log event converter.
        /// </summary>
        /// <param name="dataContext">The data context to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContext"/> is null.</exception>
        public LogEventSerilogSink(IAsyncDataContext dataContext)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        public void Emit(LogEvent logEvent)
        {
            var lockTaken = false;
            try
            {
                _spinLock.Enter(ref lockTaken);
                if (logEvent is null) return;

                var log = new T().LoadFrom(logEvent);
                AsyncHelpers.RunSync(() => _dataContext.AddRangeAsync(log.ToEnumerable()));
                AsyncHelpers.RunSync(() => _dataContext.PersistAsync(CancellationToken.None));
            }
            finally
            {
                if (lockTaken) _spinLock.Exit(false);
            }
        }
    }
}
