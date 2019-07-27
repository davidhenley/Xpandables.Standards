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

using System.Text;

namespace System.Drawing.QrCode
{
    /// <summary>
    /// Provides default validation for qr-codes.
    /// </summary>
    public sealed class QrCodeValidator : IQrCodeValidator
    {
        public void Validate(string textCode)
        {
            try
            {
                var part = textCode.Substring(0, 8);
                var crcSource = textCode.Substring(8);
                var crc = new SecurityChecker().ComputeChecksum(Encoding.ASCII.GetBytes(part));

                if (Normalize(crc) != crcSource)
                    throw new ArgumentException($"The qr-code {textCode} has an invalid CRC.");
            }
            catch (Exception exception) when (!(exception is ArgumentException))
            {
                throw new InvalidOperationException("Qr-Code validation failed. See inner exception.", exception);
            }
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