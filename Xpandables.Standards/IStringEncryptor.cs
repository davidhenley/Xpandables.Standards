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

using System.Security.Cryptography;

namespace System
{
    /// <summary>
    /// Provides with a method to encrypt string with its key.
    /// <para>Contains default implementation.</para>
    /// </summary>
    public interface IStringEncryptor
    {
        /// <summary>
        /// Returns an encrypted string from the value using the specified key.
        /// <para>The implementation uses the <see cref="SHA512Managed"/>.</para>
        /// </summary>
        /// <param name="value">The value to be encrypted.</param>
        /// <param name="key">The key value to be used for encryption.</param>
        /// <returns>An encrypted object that contains the encrypted value and its key.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        ExecutionResult<string> Encrypt(string value, string key)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            try
            {
                using var cryptoManaged = new SHA512Managed();
                var data = Text.Encoding.UTF8.GetBytes(value);
                var hash = cryptoManaged.ComputeHash(data);
                var result = BitConverter.ToString(hash).Replace("-", string.Empty, StringComparison.InvariantCulture);
                return new ExecutionResult<string>(result);
            }
            catch (Exception exception) when (exception is Text.EncoderFallbackException || exception is ObjectDisposedException)
            {
                return new ExecutionResult<string>(exception);
            }
        }
    }
}