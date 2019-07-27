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
using System.Linq;
using System.Text;

namespace System.Drawing.QrCode
{
    /// <inheritdoc />
    /// <summary>
    /// Generates qr-codes.
    /// </summary>
    public sealed class QrCodeTextGenerator : IQrCodeTextGenerator
    {
        private Func<string, string> qrCodeTextGenerator = GenerateCode;

        public IEnumerable<string> Generate(uint count = 1, string previous = "")
        {
            var qrCode = previous ?? "0";
            var number = (int)count;
            foreach (var unused in Enumerable.Range(1, number))
            {
                qrCode = qrCodeTextGenerator(qrCode);
                yield return qrCode;
            }
        }

        public IQrCodeTextGenerator UseQrTextGenerator(Func<string, string> generator)
        {
            qrCodeTextGenerator = generator;
            return this;
        }

        /// <summary>
        /// Generates a new code from the specified index.
        /// </summary>
        /// <param name="index">The index to be used.</param>
        /// <returns>A new string code.</returns>
        private static string GenerateCode(string index)
        {
            var part = Normalize(DateTime.Now.Day, 2) + Normalize(DateTime.Now.Month, 2);
            var codeBase = part + index.Normalize();
            var crc = new SecurityChecker().ComputeChecksum(Encoding.ASCII.GetBytes(codeBase));
            return codeBase + Normalize(crc);
        }

        /// <summary>
        /// Converts the value to string adding the zero to the left side for specified position.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="length">The number of zero to be added.</param>
        /// <returns>A new string.</returns>
        private static string Normalize(int value, int length = 4)
            => value.ToString().PadLeft(length, '0');
    }
}