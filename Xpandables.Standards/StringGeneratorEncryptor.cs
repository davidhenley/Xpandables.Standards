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

namespace System
{
    /// <summary>
    /// The default implementation for <see cref="IStringGeneratorEncryptor"/>.
    /// </summary>
    public class StringGeneratorEncryptor : IStringGeneratorEncryptor
    {
        private const string source = "abcdefghijklmonpqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789,;!(-è_çàà)=@%µ£¨//?§/.?";
        private static readonly IStringGenerator stringGenerator = new StringGenerator();
        private static readonly IStringEncryptor stringEncryptor = new StringEncryptor();

        public Optional<ValueEncrypted> Encrypt(string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            var key = stringGenerator.Generate(12, source);
            return stringEncryptor.Encrypt(value, key)
                    .Map(result => ValueEncrypted.CreateWith(key, result));
        }

        public bool Equals(ValueEncrypted encrypted, string value)
        {
            if (encrypted is null) throw new ArgumentNullException(nameof(encrypted));
            if (value is null) throw new ArgumentNullException(nameof(value));

            string password = stringEncryptor.Encrypt(value, encrypted.Key);
            return password == encrypted.Value;
        }
    }
}
