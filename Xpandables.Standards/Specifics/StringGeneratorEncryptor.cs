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
        private readonly IStringGenerator _stringGenerator;
        private readonly IStringEncryptor _stringEncryptor;

        public StringGeneratorEncryptor(IStringGenerator stringGenerator, IStringEncryptor stringEncryptor)
        {
            _stringGenerator = stringGenerator ?? throw new ArgumentNullException(nameof(stringGenerator));
            _stringEncryptor = stringEncryptor ?? throw new ArgumentNullException(nameof(stringEncryptor));
        }

        public Optional<EncryptedValues> Encrypt(string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            return _stringGenerator
                .Generate(12, source)
                .MapOptional(key => _stringEncryptor.Encrypt(value, key).And(key))
                .Map(pair =>new EncryptedValues(pair.Right, pair.Left));
        }

        public bool Equals(EncryptedValues encrypted, string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            string password = _stringEncryptor.Encrypt(value, encrypted.Key);
            return password == encrypted.Value;
        }
    }
}
