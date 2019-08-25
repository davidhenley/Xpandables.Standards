﻿/************************************************************************************************************
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

using System;
using System.Globalization;

namespace Xpandables
{
    public class CustomDateTimeProvider : ICustomDateTimeProvider
    {
        public DateTime GetDateTime() => DateTime.UtcNow;

        public ExecutionResult<DateTime> StringToDateTime(
            string source,
            IFormatProvider provider,
            DateTimeStyles styles,
            params string[] formats)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (provider is null) throw new ArgumentNullException(nameof(provider));

            try
            {
                if (DateTime.TryParseExact(source, formats, provider, styles, out var dateTime))
                    return new ExecutionResult<DateTime>(dateTime);

                return new ExecutionResult<DateTime>();
            }
            catch (Exception exception) when (exception is ArgumentException)
            {
                return new ExecutionResult<DateTime>(exception);
            }
        }

        public ExecutionResult<string> DateTimeToString(DateTime dateTime, string format, IFormatProvider provider)
        {
            if (format is null) throw new ArgumentNullException(nameof(format));
            if (provider is null) throw new ArgumentNullException(nameof(provider));

            try
            {
                return new ExecutionResult<string>(dateTime.ToString(format, provider));
            }
            catch (Exception exception) when (exception is FormatException || exception is ArgumentOutOfRangeException)
            {
                return new ExecutionResult<string>(exception);
            }
        }
    }
}
