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

namespace System
{
    /// <summary>
    /// Default implementation of <see cref="IStringEncryptor"/>.
    /// <para>The implementation uses the <see cref="SHA512Managed"/>.</para>
    /// </summary>
    public sealed class StringEncryptor : IStringEncryptor
    {
        public Optional<string> Encrypt(string value, string key)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            try
            {
                using (var cryptoManaged = new SHA512Managed())
                {
                    var data = Text.Encoding.UTF8.GetBytes(value);
                    var hash = cryptoManaged.ComputeHash(data);
                    var result = BitConverter.ToString(hash).Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
                    return result;
                }
            }
            catch (Exception exception) when (exception is Text.EncoderFallbackException || exception is ObjectDisposedException)
            {
                return Optional<string>.Exception(exception);
            }
        }
    }
}