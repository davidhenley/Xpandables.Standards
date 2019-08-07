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
    ///  Defines the methods that build a token from an object source and read that object source from the token.
    /// </summary>
    public interface ICustomTokenEngine
    {
        /// <summary>
        /// Builds and returns a string token using the specified data source.
        /// </summary>
        /// <typeparam name="T">Type of token source.</typeparam>
        /// <param name="source">data source to be used to build token string.</param>
        /// <returns>An instance of string token if OK or an empty string.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        string BuildToken<T>(T source) where T : class;

        /// <summary>
        /// Reads the specified token and returns the data source instance from which the token was built.
        /// </summary>
        /// <typeparam name="T">Type of token.</typeparam>
        /// <param name="token">A instance of token to act on.</param>
        /// <returns>A data query used from the string token.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="token"/> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        T ReadToken<T>(string token) where T : class;
    }
}