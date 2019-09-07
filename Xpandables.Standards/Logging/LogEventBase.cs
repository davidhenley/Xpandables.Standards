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

using Serilog.Events;

namespace System.Design.Logging
{
    /// <summary>
    /// Helper class to implement <see cref="ILogEvent{T}"/>.
    /// <para>You must derive from this class to add custom behavior to the event log.</para>
    /// </summary>
    /// <typeparam name="T">The type of derived class.</typeparam>
    public abstract class LogEventBase<T> : Entity, ILogEvent<T>
        where T : LogEventBase<T>
    {
        protected LogEventBase()
        {
            Exception = string.Empty;
            Level = string.Empty;
            Message = string.Empty;
            MessageTemplate = string.Empty;
            TimeSpan = new DateTimeOffset(DateTime.Now);
            Properties = string.Empty;
        }

        /// <summary>
        /// Contains the handled exception.
        /// </summary>
        public string Exception { get; private set; }

        /// <summary>
        /// Contains the event level.
        /// </summary>
        public string Level { get; private set; }

        /// <summary>
        /// Contains the event message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Contains the event message template.
        /// </summary>
        public string MessageTemplate { get; private set; }

        /// <summary>
        /// Contains the event time span.
        /// </summary>
        public DateTimeOffset TimeSpan { get; private set; }

        /// <summary>
        /// Contains the properties in Json format.
        /// </summary>
        public string Properties { get; private set; }

        /// <summary>
        /// Adds a message to the underlying instance.
        /// </summary>
        /// <param name="message">The event message.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        public T WithMessage(string message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            return (T)this;
        }

        /// <summary>
        /// Adds a message template to the underlying instance.
        /// </summary>
        /// <param name="messageTemplate">the event message template.</param>
        public T WithMessageTemplate(string? messageTemplate)
        {
            MessageTemplate = messageTemplate ?? string.Empty;
            return (T)this;
        }

        /// <summary>
        /// Adds a time span to the underlying instance.
        /// </summary>
        /// <param name="dateTimeOffset">The event time span.</param>
        public T WithTimeSpan(DateTimeOffset dateTimeOffset)
        {
            TimeSpan = dateTimeOffset;
            return (T)this;
        }

        /// <summary>
        /// Adds an exception to the underlying instance.
        /// </summary>
        /// <param name="exception">The event exception.</param>
        public T WithException(Exception? exception)
        {
            Exception = exception?.ToString() ?? string.Empty;
            return (T)this;
        }

        /// <summary>
        /// Adds a level to the underlying instance.
        /// </summary>
        /// <param name="level">The event level.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="level"/> is null.</exception>
        public T WithLevel(string level)
        {
            Level = level ?? throw new ArgumentNullException(nameof(level));
            return (T)this;
        }

        /// <summary>
        /// Adds a Json object to the underlying instance.
        /// </summary>
        /// <param name="properties">The Json properties</param>
        /// <exception cref="ArgumentNullException">The <paramref name="properties"/> is null.</exception>
        public T WithProperties(string properties)
        {
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
            return (T)this;
        }

        /// <summary>
        /// Loads the underlying instance from the event.
        /// </summary>
        /// <param name="logEvent">The event source.</param>
        public abstract T LoadFrom(LogEvent logEvent);
    }
}
