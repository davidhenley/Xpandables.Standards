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

using System.ComponentModel;

namespace System
{
    /// <summary>
    /// Determines the logging event level.
    /// </summary>
    [Description("Defines the logging event level.")]
    [Serializable]
    [TypeConverter(typeof(EnumerationTypeConverter))]
    public sealed class LogSeverity : Enumeration
    {
        private LogSeverity(int value, string displayName)
            : base(displayName, value) { }

        /// <summary>
        /// For information that's valuable only to a developer debugging an issue.
        /// These messages may contain sensitive application data and so shouldn't be enabled in a production environment.
        /// Disabled by default.
        /// </summary>
        public static LogSeverity Trace => new LogSeverity(0, nameof(Trace));

        /// <summary>
        /// For information that has short-term usefulness during development and debugging.
        /// Example: Entering method Configure with flag set to true.
        /// You typically wouldn't enable Debug level logs in production unless you are troubleshooting, due to the high volume of logs.
        /// </summary>
        public static LogSeverity Debug => new LogSeverity(1, nameof(Debug));

        /// <summary>
        /// For tracking the general flow of the application. These logs typically have some long-term value.
        /// Example: Request received for path /api/...
        /// </summary>
        public static LogSeverity Information => new LogSeverity(2, nameof(Information));

        /// <summary>
        /// For abnormal or unexpected events in the application flow.
        /// These may include errors or other conditions that don't cause the application to stop, but which may need to be investigated.
        /// Handled exceptions are a common place to use the Warning log level. Example: FileNotFoundException for file quotes.txt.
        /// </summary>
        public static LogSeverity Warning => new LogSeverity(3, nameof(Warning));

        /// <summary>
        /// For errors and exceptions that cannot be handled.
        /// These messages indicate a failure in the current activity or operation (such as the current HTTP request),
        /// not an application-wide failure.
        /// Example log message: Cannot insert record due to duplicate key violation.
        /// </summary>
        public static LogSeverity Error => new LogSeverity(4, nameof(Error));

        /// <summary>
        /// For failures that require immediate attention. Examples: data loss scenarios, out of disk space.
        /// </summary>
        public static LogSeverity Critical => new LogSeverity(5, nameof(Critical));
    }
}