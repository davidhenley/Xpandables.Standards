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

namespace System
{
    /// <summary>
    ///  Defines a method to decode a token from a string.
    /// </summary>
    public interface ICustomTokenDecoder
    {
        /// <summary>
        /// Decodes the specified token and returns the value as the specific type.
        /// </summary>
        /// <typeparam name="T">Type of value expected.</typeparam>
        /// <param name="token">A instance of token to act on.</param>
        /// <returns>A data query used from the string token.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="token"/> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        T Decode<T>(string token) where T : class;
    }
}