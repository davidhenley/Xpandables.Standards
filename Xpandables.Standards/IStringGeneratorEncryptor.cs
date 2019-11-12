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

using System.Security.Cryptography;

namespace System
{
    /// <summary>
    /// Provides with a string generator and encryptor.
    /// Contains a default implementation and uses <see cref="source"/> to generate values string.
    /// </summary>
    public interface IStringGeneratorEncryptor : IFluent
    {
        private const string source = "abcdefghijklmonpqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789,;!(-è_çàà)=@%µ£¨//?§/.?";
        private static readonly IStringGenerator _stringGenerator = new StringGenerator();
        private static readonly IStringEncryptor _stringEncryptor = new StringEncryptor();

        /// <summary>
        /// Compares the encrypted value with the specified one.
        /// Returns <see langword="true"/> if equality otherwise <see langword="false"/>.
        /// </summary>
        /// <param name="encrypted">The encrypted value.</param>
        /// <param name="value">The value to compare with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        bool Equals(EncryptedValue encrypted, string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            string password = _stringEncryptor.Encrypt(value, encrypted.Key);
            return password == encrypted.Value;
        }

        /// <summary>
        /// Returns an encrypted string from the value using a randomize key.
        /// <para>The implementation uses the <see cref="SHA512Managed"/>.</para>
        /// </summary>
        /// <param name="value">The value to be encrypted.</param>
        /// <param name="keySize">The size of the string key to be used to encrypt the string value.</param>
        /// <returns>An encrypted object that contains the encrypted value and its key.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="keySize"/> is lower than zero and greater than <see cref="byte.MaxValue"/>.</exception>
        Optional<EncryptedValue> Encrypt(string value, int keySize = 12)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            keySize
                .WhenNotInRange(1, byte.MaxValue, $"{keySize} must be between 1 and {byte.MaxValue}.")
                .ThrowArgumentOutOfRangeException();

            return _stringGenerator
                .Generate(keySize, source)
                .MapOptional(key => _stringEncryptor.Encrypt(value, key).And(key))
                .Map(pair => new EncryptedValue(pair.Right, pair.Left));
        }
    }

    /// <summary>
    /// Provides with <see cref="IStringEncryptor"/> and <see cref="IStringGenerator"/> implementation.
    /// </summary>
    public class StringGeneratorEncryptor : IStringGeneratorEncryptor { }
}
