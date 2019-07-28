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

namespace System.Configuration
{
    /// <summary>
    /// Defines a method used to retrieve the ambient token string from the current http request header.
    /// </summary>
    public interface IHttpRequestTokenProvider
    {
        /// <summary>
        /// Returns the current token value from the current http request matching the "Authorization" key.
        /// If not found, returns an empty optional.
        /// </summary>
        Optional<string> GetRequestHttpToken();

        /// <summary>
        /// Returns the current token value from the current http request with the specified key.
        /// If not found, returns an empty optional.
        /// </summary>
        /// <param name="key">The token key to find.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        Optional<string> GetRequestHttpToken(string key);
    }
}