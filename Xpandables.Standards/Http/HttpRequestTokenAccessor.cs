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

namespace System.Http
{
    /// <summary>
    /// The default implementation for <see cref="IHttpRequestTokenAccessor"/>.
    /// </summary>
    public sealed class HttpRequestTokenAccessor : IHttpRequestTokenAccessor
    {
        private readonly IHttpRequestHeaderValuesAccessor _headerValuesAccessor;

        public HttpRequestTokenAccessor(IHttpRequestHeaderValuesAccessor headerValuesAccessor)
            => _headerValuesAccessor = headerValuesAccessor ?? throw new ArgumentNullException(nameof(headerValuesAccessor));

        public Optional<string> GetRequestHttpToken(string key)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));

            return _headerValuesAccessor.GetRequestHeaderValue(key)
                .Map(header => header.StartsWith("Bearer", StringComparison.InvariantCulture)
                ? header.Remove(0, "Bearer ".Length - 1).Trim()
                : header);
        }
    }
}