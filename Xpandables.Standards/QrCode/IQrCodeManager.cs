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

namespace System.Drawing.QrCode
{
    /// <summary>
    /// Allows an application author to manage qrCodes.
    /// </summary>
    public interface IQrCodeManager
    {
        /// <summary>
        /// Changes the text generator service to the new one.
        /// </summary>
        /// <param name="newTextGeneratorService">The new text generator service.</param>
        /// <returns>The actual instance with the new text generator.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="newTextGeneratorService"/> is null.</exception>
        IQrCodeManager UseTextGenerator(IQrCodeTextGenerator newTextGeneratorService);

        /// <summary>
        /// Changes the image generator service to the new one.
        /// </summary>
        /// <param name="newImageGeneratorService">The new image generator service to use.</param>
        /// <returns>The actual instance with the new image generator service.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="newImageGeneratorService"/> is null.</exception>
        IQrCodeManager UseImageGenerator(IQrCodeImageGenerator newImageGeneratorService);

        /// <summary>
        /// Changes the validator service to the new one.
        /// </summary>
        /// <param name="newValidatorService">The new validator service.</param>
        /// <returns>The actual instance with the new validator service.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="newValidatorService"/> is null.</exception>
        IQrCodeManager UseValidator(IQrCodeValidator newValidatorService);

        /// <summary>
        /// Generates a list of qr-codes.
        /// </summary>
        /// <param name="count">The number of qr-codes to be generates. Must be greater or equal to 1.</param>
        /// <param name="previousIndex">The previous index to be used.</param>
        /// <returns>A new list of qr-codes</returns>
        /// <exception cref="ArgumentException">The <paramref name="previousIndex"/> is not valid previous index.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        IEnumerable<string> Generate(uint count = 1, string previousIndex = "");

        /// <summary>
        /// Generates a list of qr-codes.
        /// </summary>
        /// <param name="textCodeList">The list of qr-codes text to generate image.</param>
        /// <returns>A new list of qr-codes</returns>
        /// <exception cref="InvalidOperationException">Generating images failed. See inner exception.</exception>
        IEnumerable<byte[]> Generate(IEnumerable<string> textCodeList);

        /// <summary>
        /// Validates that the specified qr-code matches the expected requirement.
        /// </summary>
        /// <param name="textCode">Th qr-code to be validated.</param>
        /// <exception cref="ArgumentException">The <paramref name="textCode"/> is not valid.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="textCode"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        void Validate(string textCode);
    }
}