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

using System.Collections.Generic;
using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// String extension methods.
    /// </summary>
    public static class StringHelpers
    {
        /// <summary>
        /// Determines whether the string is a well formatted email address.
        /// </summary>
        /// <param name="source">The email address to be checked.</param>
        /// <returns><see langword="true"/> if well formatted, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static bool IsValidEmailAddress(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException(nameof(source));

            try
            {
                _ = new MailAddress(source);
                return true;
            }
            catch (Exception exception) when (exception is FormatException || exception is ArgumentException)
            {
                Diagnostics.Debug.WriteLine(exception);
                return false;
            }
        }

        /// <summary>
        /// Determines whether the string is a well formatted email address.
        /// </summary>
        /// <param name="source">The email address to be checked.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <returns><see langword="true"/> if well formatted, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="pattern"/> is null.</exception>
        public static bool IsValidPhone(this string source, string pattern)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));

            try
            {
                return Regex.IsMatch(source, pattern);
            }
            catch (Exception exception) when (exception is ArgumentException || exception is RegexMatchTimeoutException)
            {
                Diagnostics.Debug.WriteLine(exception);
                return false;
            }
        }

        /// <summary>
        /// Replaces the argument object into the current text equivalent <see cref="string"/>
        /// using the default <see cref="CultureInfo.InvariantCulture"/>.
        /// </summary>
        /// <param name="value">The format string.</param>
        /// <param name="args">The object to be formatted.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null or <paramref name="args"/> is null.</exception>
        /// <exception cref="InvalidOperationException">See inner exception.</exception>
        /// <returns>value <see cref="string"/> filled with <paramref name="args"/>.</returns>
        public static string StringFormat(this string value, params object[] args)
            => StringFormat(value, CultureInfo.InvariantCulture, args);

        /// <summary>
        /// Replaces the argument object into the current text equivalent <see cref="string"/> using the specified culture.
        /// </summary>
        /// <param name="value">The format string.</param>
        /// <param name="cultureInfo">CultureInfo to be used.</param>
        /// <param name="args">The object to be formatted.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null or <paramref name="cultureInfo"/> or <paramref name="args"/> is null.</exception>
        /// <exception cref="InvalidOperationException">See inner exception.</exception>
        /// <returns>value <see cref="string"/> filled with <paramref name="args"/></returns>
        public static string StringFormat(this string value, CultureInfo cultureInfo, params object[] args)
        {
            try
            {
                return string.Format(cultureInfo, value, args);
            }
            catch (FormatException exception)
            {
                throw new InvalidOperationException(
                    "Formatting string failed. See inner exception",
                    exception);
            }
        }

        /// <summary>
        /// Concatenates all the elements of an <see cref="IEnumerable{T}"/>,
        /// using the specified string separator between each element.
        /// </summary>
        /// <typeparam name="T">The generic type parameter.</typeparam>
        /// <param name="collection">The collection to act on.</param>
        /// <param name="separator">The string to use as a separator.
        /// Separator is included in the returned string only if value has more than one element.</param>
        /// <returns>A string that consists of the elements in value delimited by the separator string.
        /// If value is an empty array, the method returns String.Empty.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is null.</exception>
        public static string StringJoin<T>(this IEnumerable<T> collection, string separator)
            => string.Join(separator, collection);

        /// <summary>
        /// Concatenates all the elements of an <see cref="IEnumerable{T}"/>,
        /// using the specified char separator between each element.
        /// </summary>
        /// <typeparam name="T">The generic type parameter.</typeparam>
        /// <param name="collection">The collection to act on.</param>
        /// <param name="separator">The string to use as a separator.
        /// Separator is included in the returned string only if value has more than one element.</param>
        /// <returns>A string that consists of the elements in value delimited by the separator string.
        /// If value is an empty array, the method returns String.Empty.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is null.</exception>
        public static string StringJoin<T>(this IEnumerable<T> collection, char separator)
            => string.Join(separator.ToString(), collection);

        /// <summary>
        /// Converts a string to a value type.
        /// </summary>
        /// <typeparam name="T">Type source.</typeparam>
        /// <param name="value">The string value.</param>
        /// <returns>The string value converted to the specified value type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Conversion failed. See inner exception.</exception>
        public static T ToValueType<T>(this string value)
            where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.CurrentCulture);
            }
            catch (Exception exception) when (exception is InvalidCastException
                                            || exception is FormatException
                                            || exception is OverflowException)
            {
                throw new InvalidOperationException(
                     $"Conversion to {typeof(T).Name} failed. See inner exception",
                     exception);
            }
        }

        /// <summary>
        /// Converts a string to a value type.
        /// Returns the <paramref name="valueIfException"/> when conversion failed.
        /// </summary>
        /// <typeparam name="T">Type target.</typeparam>
        /// <param name="value">The string value.</param>
        /// <param name="valueIfException">The value to be returned if conversion failed.</param>
        /// <returns>The string value converted to the specified value type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Conversion failed. See inner exception.</exception>
        public static T ToValueType<T>(this string value, T valueIfException)
            where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.CurrentCulture);
            }
            catch (Exception exception) when (exception is InvalidCastException
                                            || exception is FormatException
                                            || exception is OverflowException)
            {
                return valueIfException;
            }
        }
    }
}