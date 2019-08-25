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
using System.Text;

namespace System
{
    /// <summary>
    /// Default implementation of <see cref="IStringGenerator"/>.
    /// </summary>
    public sealed class StringGenerator : IStringGenerator
    {
        public string Generate(int length, string lookupCharacters)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
            if (string.IsNullOrWhiteSpace(lookupCharacters)) throw new ArgumentNullException(nameof(lookupCharacters));

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
    }
}