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
    /// The default Qr-Code engine.
    /// </summary>
    public sealed class QrCodeManager : IQrCodeManager
    {
        private IQrCodeTextGenerator _textGeneratorService;
        private IQrCodeImageGenerator _imageGeneratorService;
        private IQrCodeValidator _validatorService;

        public QrCodeManager(
            IQrCodeTextGenerator textGeneratorService,
            IQrCodeImageGenerator imageGeneratorService,
            IQrCodeValidator validatorService)
        {
            _textGeneratorService = textGeneratorService;
            _imageGeneratorService = imageGeneratorService;
            _validatorService = validatorService;
        }

        public IQrCodeManager UseTextGenerator(IQrCodeTextGenerator newTextGeneratorService)
        {
            _textGeneratorService = newTextGeneratorService;
            return this;
        }

        public IQrCodeManager UseImageGenerator(IQrCodeImageGenerator newImageGeneratorService)
        {
            _imageGeneratorService = newImageGeneratorService;
            return this;
        }

        public IQrCodeManager UseValidator(IQrCodeValidator newValidatorService)
        {
            _validatorService = newValidatorService;
            return this;
        }

        public IEnumerable<string> Generate(uint count = 1, string previousIndex = "")
            => _textGeneratorService.Generate(count, previousIndex);

        public IEnumerable<byte[]> Generate(IEnumerable<string> textCodeList)
            => _imageGeneratorService.Generate(textCodeList);

        public void Validate(string textCode)
            => _validatorService.Validate(textCode);
    }
}