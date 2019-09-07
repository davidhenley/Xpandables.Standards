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
    /// Provides with the base description for an event log using <see cref="Serilog"/>.
    /// </summary>
    public interface ILogEvent
    {
        /// <summary>
        /// Contains the event identifier.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Contains the handled exception.
        /// </summary>
        string Exception { get; }

        /// <summary>
        /// Contains the event level.
        /// </summary>
        string Level { get; }

        /// <summary>
        /// Contains the event message.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Contains the event message template.
        /// </summary>
        string MessageTemplate { get; }

        /// <summary>
        /// Contains the event time span.
        /// </summary>
        DateTimeOffset TimeSpan { get; }

        /// <summary>
        /// Contains the properties in Json format.
        /// </summary>
        string Properties { get; }
    }

    /// <summary>
    /// Provides with the base description for an event log using <see cref="Serilog"/>.
    /// </summary>
    /// <typeparam name="T">The type of the log event class.</typeparam>
    public interface ILogEvent<T> : ILogEvent
        where T : class, ILogEvent<T>
    {
        /// <summary>
        /// Adds a message to the underlying instance.
        /// </summary>
        /// <param name="message">The event message.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        T WithMessage(string message);

        /// <summary>
        /// Adds a message template to the underlying instance.
        /// </summary>
        /// <param name="messageTemplate">the event message template.</param>
        T WithMessageTemplate(string? messageTemplate);

        /// <summary>
        /// Adds a time span to the underlying instance.
        /// </summary>
        /// <param name="dateTimeOffset">The event time span.</param>
        T WithTimeSpan(DateTimeOffset dateTimeOffset);

        /// <summary>
        /// Adds an exception to the underlying instance.
        /// </summary>
        /// <param name="exception">The event exception.</param>
        T WithException(Exception? exception);

        /// <summary>
        /// Adds a level to the underlying instance.
        /// </summary>
        /// <param name="level">The event level.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="level"/> is null.</exception>
        T WithLevel(string level);

        /// <summary>
        /// Adds a Json object to the underlying instance.
        /// </summary>
        /// <param name="properties">The Json properties</param>
        /// <exception cref="ArgumentNullException">The <paramref name="properties"/> is null.</exception>
        T WithProperties(string properties);

        /// <summary>
        /// Loads the underlying instance from the event.
        /// </summary>
        /// <param name="logEvent">The event source.</param>
        T LoadFrom(LogEvent logEvent);
    }
}
