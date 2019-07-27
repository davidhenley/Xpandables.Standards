/************************************************************************************************************
 * Copyright (C) 2018 Francis-Black EWANE
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

namespace System
{
    /// <summary>
    /// Defines the log event.
    /// </summary>
    public class LogEvent : EventArgs
    {
        /// <summary>
        /// Provides with an instance of <see cref="LogEvent"/>.
        /// </summary>
        /// <returns></returns>
        public static LogEvent CreateLogEvent() => new LogEvent();

        /// <summary>
        /// Initializes a new <see cref="LogEvent"/> with the specified values.
        /// </summary>
        /// <param name="eventId">The event identifier</param>
        /// <param name="eventTypeName">The event type name.</param>
        /// <param name="severity">The severity level.</param>
        /// <param name="createdOn">The creation date.</param>
        /// <param name="content">The event content.</param>
        /// <param name="exception">The handled exception.</param>
        public LogEvent(
            Guid eventId,
            string eventTypeName,
            LogSeverity severity,
            DateTime createdOn,
            string content,
            Exception exception = default)
        {
            EventId = eventId;
            EventTypeName = eventTypeName;
            Severity = severity;
            CreatedOn = createdOn;
            Content = content;
            Exception = exception;
        }

        private LogEvent()
        {
            CreatedOn = DateTime.Now;
        }

        /// <summary>
        /// Adds event id.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public LogEvent WithEventId(Guid eventId)
        {
            EventId = eventId;
            return this;
        }

        /// <summary>
        /// Adds event type name.
        /// </summary>
        /// <param name="eventTypeName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="eventTypeName"/> is null.</exception>
        public LogEvent WithEventTypeName(string eventTypeName)
        {
            EventTypeName = eventTypeName ?? throw new ArgumentNullException(nameof(eventTypeName));
            return this;
        }

        /// <summary>
        /// Adds severity.
        /// </summary>
        /// <param name="severity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="severity"/> is null.</exception>
        public LogEvent WithSeverity(LogSeverity severity)
        {
            Severity = severity ?? throw new ArgumentNullException(nameof(severity));
            return this;
        }

        /// <summary>
        /// Adds content.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public LogEvent WithContent(string content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            return this;
        }

        /// <summary>
        /// Adds exception.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public LogEvent WithException(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            return this;
        }

        /// <summary>
        /// The event identifier.
        /// </summary>
        public Guid EventId { get; private set; }

        /// <summary>
        /// The event type name.
        /// </summary>
        public string EventTypeName { get; private set; }

        /// <summary>
        /// The severity level.
        /// </summary>
        public LogSeverity Severity { get; private set; }

        /// <summary>
        /// The creation date.
        /// </summary>
        public DateTime CreatedOn { get; }

        /// <summary>
        /// The event content.
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// The handled exception.
        /// </summary>
        public Exception Exception { get; private set; }
    }
}