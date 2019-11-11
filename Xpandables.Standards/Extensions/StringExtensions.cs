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
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// String extension methods.
    /// </summary>
    public static class StringExtensions
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
            if (string.IsNullOrWhiteSpace(source)) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(pattern)) throw new ArgumentNullException(nameof(pattern));

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
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null or
        /// <paramref name="cultureInfo"/> or <paramref name="args"/> is null.</exception>
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
                    ErrorMessageResources.StringHelperFormattingFailed,
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
            => string.Join(separator.ToString(CultureInfo.InvariantCulture), collection);

        /// <summary>
        /// Converts a string to a value type.
        /// </summary>
        /// <typeparam name="T">Type source.</typeparam>
        /// <param name="value">The string value.</param>
        /// <returns>The string value converted to the specified value type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null or empty.</exception>
        public static Optional<T> ToValueType<T>(this string value)
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
                return Optional<T>.Exception(exception);
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

        /// <summary>
        /// Converts string date to <see cref="DateTime"/> type.
        /// If error, returns an exception optional.
        /// </summary>
        /// <param name="source">A string containing a date and time to convert.</param>
        /// <param name="provider">An object that supplies culture-specific format information about string.</param>
        /// <param name="styles"> A bitwise combination of enumeration values that indicates the permitted format
        /// of string. A typical value to specify is System.Globalization.DateTimeStyles.None.</param>
        /// <param name="formats">An array of allowable formats of strings.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="provider"/> is null.</exception>
        public static Optional<DateTime> ToDateTime(
            this string source,
            IFormatProvider provider,
            DateTimeStyles styles,
            params string[] formats)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (provider is null) throw new ArgumentNullException(nameof(provider));

            try
            {
                if (DateTime.TryParseExact(source, formats, provider, styles, out var dateTime))
                    return dateTime;

                return Optional<DateTime>.Empty();
            }
            catch (Exception exception) when (exception is ArgumentException)
            {
                return Optional<DateTime>.Exception(exception);
            }
        }

        /// <summary>
        /// Converts the value of the current System.DateTime object to its equivalent string
        /// representation using the specified format and culture-specific format information.
        /// If error, returns an exception
        /// </summary>
        /// <param name="dateTime">The date time to be converted.</param>
        /// <param name="format">A standard or custom date and time format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>An optional string representation of value of the current System.DateTime object as specified
        /// by format and provider.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="format"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="provider"/> is null.</exception>
        public static Optional<string> DateTimeToString(this DateTime dateTime, string format, IFormatProvider provider)
        {
            if (format is null) throw new ArgumentNullException(nameof(format));
            if (provider is null) throw new ArgumentNullException(nameof(provider));

            try
            {
                return dateTime.ToString(format, provider);
            }
            catch (Exception exception) when (exception is FormatException
                                            || exception is ArgumentOutOfRangeException)
            {
                return Optional<string>.Exception(exception);
            }
        }

        /// <summary>
        /// Returns an encrypted string from the value using the specified key.
        /// <para>The implementation uses the <see cref="SHA512Managed"/>.</para>
        /// </summary>
        /// <param name="source">The value to be encrypted.</param>
        /// <param name="key">The key value to be used for encryption.</param>
        /// <returns>An encrypted object that contains the encrypted value and its key.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        public static Optional<string> Encrypt(this string source, string key)
        {
            if (string.IsNullOrWhiteSpace(source)) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            try
            {
                using var cryptoManaged = new SHA512Managed();
                var data = Text.Encoding.UTF8.GetBytes(source);
                var hash = cryptoManaged.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception exception) when (exception is Text.EncoderFallbackException || exception is ObjectDisposedException)
            {
                return Optional<string>.Exception(exception);
            }
        }

        /// <summary>
        /// Generates a string of the specified length that contains random characters from the lookup characters.
        /// <para>The implementation uses the <see cref="RNGCryptoServiceProvider"/>.</para>
        /// </summary>
        /// <remarks>
        /// Inspiration from https://stackoverflow.com/questions/32932679/using-rngcryptoserviceprovider-to-generate-random-string
        /// </remarks>
        /// <param name="lookupCharacters">The string to be used to pick characters from.</param>
        /// <param name="length">The length of the expected string value.</param>
        /// <returns>A new string of the specified length with random characters.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length"/> is lower or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="lookupCharacters"/> is null.</exception>
        public static Optional<string> GenerateString(this string lookupCharacters, int length)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
            if (string.IsNullOrWhiteSpace(lookupCharacters)) throw new ArgumentNullException(nameof(lookupCharacters));

            try
            {
                var stringResult = new StringBuilder(length);
                using (var random = new RNGCryptoServiceProvider())
                {
                    var count = (int)Math.Ceiling(Math.Log(lookupCharacters.Length, 2) / 8.0);
                    Diagnostics.Debug.Assert(count <= sizeof(uint));

                    var offset = BitConverter.IsLittleEndian ? 0 : sizeof(uint) - count;
                    var max = (int)(Math.Pow(2, count * 8) / lookupCharacters.Length) * lookupCharacters.Length;

                    var uintBuffer = new byte[sizeof(uint)];
                    while (stringResult.Length < length)
                    {
                        random.GetBytes(uintBuffer, offset, count);
                        var number = BitConverter.ToUInt32(uintBuffer, 0);
                        if (number < max)
                            stringResult.Append(lookupCharacters[(int)(number % lookupCharacters.Length)]);
                    }
                }

                return stringResult.ToString();
            }
            catch (Exception exception) when (exception is ArgumentOutOfRangeException
                                            || exception is ArgumentException
                                            || exception is ArgumentNullException)
            {
                return Optional<string>.Exception(exception);
            }
        }

        private const string lookupCharacters = "abcdefghijklmonpqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789,;!(-è_çàà)=@%µ£¨//?§/.?";

        /// <summary>
        /// Returns an encrypted string from the value using a randomize key.
        /// <para>The implementation uses the <see cref="SHA512Managed"/>.</para>
        /// </summary>
        /// <param name="source">The value to be encrypted.</param>
        /// <returns>An encrypted object that contains the encrypted value and its key.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static Optional<EncryptedValues> Encrypt(this string source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            return lookupCharacters
                .GenerateString(12)
                .MapOptional(key => source.Encrypt(key).And(() => key))
                .Map(pair => new EncryptedValues(pair.Right, pair.Left));
        }

        /// <summary>
        /// Compares the encrypted value with the specified one.
        /// Returns <see langword="true"/> if equality otherwise <see langword="false"/>.
        /// </summary>
        /// <param name="encrypted">The encrypted value.</param>
        /// <param name="value">The value to compare with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public static bool Equals(this EncryptedValues encrypted, string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            string compare = value.Encrypt(encrypted.Key);
            return compare == encrypted.Value;
        }
    }
}