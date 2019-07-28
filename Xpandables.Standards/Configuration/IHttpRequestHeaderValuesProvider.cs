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

namespace System.Configuration
{
    /// <summary>
    /// Provides with methods to retrieve an http request header value matching a key.
    /// </summary>
    public interface IHttpRequestHeaderValuesProvider
    {
        /// <summary>
        /// Gets the http header value from the current http request matching the specified key.
        /// If not found, returns an empty optional.
        /// </summary>
        /// <param name="key">The key of the value to match.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        Optional<string> GetRequestHeaderValue(string key);

        /// <summary>
        /// Gets all http header values from the current http request matching the specified key.
        /// If not found, returns an empty enumerable.
        /// </summary>
        /// <param name="key">The key of the value to match.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        IEnumerable<string> GetRequestHeaderValues(string key);
    }
}