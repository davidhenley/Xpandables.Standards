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

using QRCoder;
using System.Drawing.Imaging;
using System.IO;

namespace System.Drawing.QrCode
{
    /// <summary>
    /// Extension methods that extend generic type behaviors.
    /// </summary>
    internal static class QrCodeHelpers
    {
        /// <summary>
        /// Converts the bitmap image to the byte format using the default Jpeg format.
        /// </summary>
        /// <remarks>If error, return default value. See trace listener output for exception.</remarks>
        /// <param name="image">The source image to be converted.</param>
        /// <returns>An array of byte that represents the image.</returns>
        public static byte[] ToByte(this Bitmap image)
            => ToByte(image, ImageFormat.Jpeg);

        /// <summary>
        /// Converts the bitmap image to the byte format using the specified format.
        /// </summary>
        /// <remarks>If error, return default value. See trace listener output for exception.</remarks>
        /// <param name="image">The source image to be converted.</param>
        /// <param name="imageFormat">The target image format.</param>
        /// <returns>An array of byte that represents the image.</returns>
        public static byte[] ToByte(this Bitmap image, ImageFormat imageFormat)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                image.Save(memoryStream, imageFormat);
                return memoryStream.ToArray();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Trace.WriteLine(exception);
                return default;
            }
        }

        private static readonly QRCodeGenerator _qRCodeGenerator = new QRCodeGenerator();

        /// <summary>
        /// Generates image from the codes provided.
        /// </summary>
        /// <param name="source">List of code to be used.</param>
        /// <returns>A list of image matching the codes.</returns>
        public static byte[] GenerateImage(string source)
        {
            using var qrCodeData = _qRCodeGenerator.CreateQrCode(source, QRCodeGenerator.ECCLevel.Q, true);
            using var qrCode = new QRCode(qrCodeData);
            var image = qrCode.GetGraphic(20);
            return image.ToByte();
        }
    }
}