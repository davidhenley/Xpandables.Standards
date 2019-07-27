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

using System.Linq;

namespace System.Drawing.QrCode
{
    /// <summary>
    /// This class is used for CRC.
    /// </summary>
    internal class SecurityChecker
    {
        private const ushort Polynomial = 0xFAAF;
        private readonly ushort[] _table = new ushort[256];

        /// <summary>
        /// Computes check sum for the specified array of bytes.
        /// </summary>
        /// <param name="bytes">The array of bytes to act on.</param>
        /// <returns>The number that represents the check sum.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="bytes"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The process failed to execute. See inner exception.</exception>
        public int ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            foreach (var b in bytes)
            {
                var index = (byte)(crc ^ b);
                crc = (ushort)((crc >> 8) ^ _table[index]);
            }

            var hexValue = crc.ToString("X");

            var crcInt = 0;

            hexValue.ToList().ForEach(p => crcInt += Convert.ToChar(p));

            return crcInt;
        }

        /// <summary>
        /// Returns the default instance and initializes the CRC table.
        /// </summary>
        public SecurityChecker()
        {
            for (ushort i = 0; i < _table.Length; ++i)
            {
                ushort value = 0;
                var temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ Polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }

                _table[i] = value;
            }
        }
    }
}