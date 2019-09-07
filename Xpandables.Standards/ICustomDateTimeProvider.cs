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

using System.Globalization;

namespace System
{
    /// <summary>
    /// Allows an application author to provide runtime date time according to the context.
    /// <para>Contains default implementation.</para>
    /// </summary>
    public interface ICustomDateTimeProvider
    {
        /// <summary>
        /// Returns the ambient date time.
        /// The default behavior returns <see cref="DateTime.UtcNow"/>.
        /// </summary>
        DateTime GetDateTime();

        /// <summary>
        /// Converts string date time to <see cref="DateTime"/> type.
        /// If error, returns an empty optional.
        /// </summary>
        /// <param name="source">A string containing a date and time to convert.</param>
        /// <param name="provider">An object that supplies culture-specific format information about string.</param>
        /// <param name="styles"> A bitwise combination of enumeration values that indicates the permitted format
        /// of string. A typical value to specify is System.Globalization.DateTimeStyles.None.</param>
        /// <param name="formats">An array of allowable formats of strings.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="provider"/> is null.</exception>
        Optional<DateTime> StringToDateTime(
            string source,
            IFormatProvider provider,
            DateTimeStyles styles,
            params string[] formats);

        /// <summary>
        /// Converts the value of the current System.DateTime object to its equivalent string
        /// representation using the specified format and culture-specific format information.
        /// </summary>
        /// <param name="dateTime">The date time to be converted.</param>
        /// <param name="format">A standard or custom date and time format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>An optional string representation of value of the current System.DateTime object as specified
        /// by format and provider.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="format"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="provider"/> is null.</exception>
        Optional<string> DateTimeToString(DateTime dateTime, string format, IFormatProvider provider);
    }
}
