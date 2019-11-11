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

using System.Security.Cryptography;
using System.Text;

namespace System
{
    /// <summary>
    /// Provides with a method to be used to generate random string value.
    /// <para>Contains default implementation.</para>
    /// </summary>
    public interface IStringGenerator
    {
        /// <summary>
        /// Generates a string of the specified length that contains random characters from the lookup characters.
        /// <para>The implementation uses the <see cref="RNGCryptoServiceProvider"/>.</para>
        /// </summary>
        /// <remarks>
        /// Inspiration from https://stackoverflow.com/questions/32932679/using-rngcryptoserviceprovider-to-generate-random-string
        /// </remarks>
        /// <param name="length">The length of the expected string value.</param>
        /// <param name="lookupCharacters">The string to be used to pick characters from.</param>
        /// <returns>A new string of the specified length with random characters.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length"/> is lower or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="lookupCharacters"/> is null.</exception>
        Optional<string> Generate(int length, string lookupCharacters)
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
            catch (Exception exception) when (exception is ArgumentNullException
                                            || exception is ArgumentException
                                            || exception is ArgumentOutOfRangeException)
            {
                return Optional<string>.Exception(exception);
            }
        }
    }

    /// <summary>
    /// Default implementation of <see cref="IStringGenerator"/>.
    /// </summary>
    public sealed class StringGenerator : IStringGenerator { }
}
