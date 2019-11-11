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
using Serilog.Formatting.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;

namespace System.Design.Logging
{
    /// <summary>
    /// The default log entity.
    /// </summary>
    [Table("Log")]
    public class LogEntity : LogEventBase<LogEntity>
    {
        /// <summary>
        /// Initializes a new log entity with default values.
        /// </summary>
        public static LogEntity CreateLogEntity() => new LogEntity();

        /// <summary>
        /// Loads the underlying instance from the event.
        /// </summary>
        /// <param name="logEvent">The event source.</param>
        public override LogEntity LoadFrom(LogEvent logEvent)
        {
            if (logEvent is null)
                throw new ArgumentNullException(nameof(logEvent));

            var json = ConvertLogEventToJson(logEvent);
            using var jobject = Text.Json.JsonDocument.Parse(json);
            var properties = jobject.RootElement.GetProperty("Properties");

            return
                WithException(logEvent.Exception)
                .WithLevel(logEvent.Level.ToString())
                .WithProperties(properties.ToString())
                .WithMessage(logEvent.RenderMessage())
                .WithMessageTemplate(logEvent.MessageTemplate?.ToString())
                .WithTimeSpan(logEvent.Timestamp);

            static string ConvertLogEventToJson(LogEvent log)
            {
                var stringBuilder = new StringBuilder();
                using (var writer = new StringWriter(stringBuilder))
                    new JsonFormatter().Format(log, writer);

                return stringBuilder.ToString();
            }
        }
    }
}
