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
    ///  Defines a method that build a token from an object source.
    /// </summary>
    public interface ICustomTokenBuilder
    {
        /// <summary>
        /// Encodes the source and returns a string token.
        /// </summary>
        /// <typeparam name="T">Type of source.</typeparam>
        /// <param name="source">data source to be used to build token string.</param>
        /// <returns>An instance of string token if OK or an empty string.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        Optional<string> Build<T>(T source) where T : class;

        /// <summary>
        /// Decodes the token and return the specified type.
        /// </summary>
        /// <typeparam name="T">The type to be returned.</typeparam>
        /// <param name="source">The token string.</param>
        /// <returns>Ann optional of <typeparamref name="T"/> if OK or an empty type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        Optional<T> Read<T>(string source) where T : class;
    }
}