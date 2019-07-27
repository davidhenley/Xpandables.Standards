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

namespace System.Drawing.QrCode
{
    /// <summary>
    /// Provides a method for validating qr-code.
    /// </summary>
    public interface IQrCodeValidator
    {
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