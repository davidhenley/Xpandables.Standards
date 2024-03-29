﻿/************************************************************************************************************
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
    /// The default implementation for <see cref="IHttpRequestUserClaimAccessor"/> that uses <see cref="IHttpRequestTokenAccessor"/>
    /// and <see cref="ITokenEngine"/>.
    /// </summary>
    public class HttpRequestUserClaimAccessor : IHttpRequestUserClaimAccessor
    {
        private readonly IHttpRequestTokenAccessor _httpRequestTokenAccessor;
        private readonly ITokenEngine _tokenEngine;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRequestUserClaimAccessor"/> with specified arguments.
        /// </summary>
        /// <param name="httpRequestTokenAccessor">The token accessor instance.</param>
        /// <param name="tokenEngine">The token engine.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="httpRequestTokenAccessor"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="tokenEngine"/> is null.</exception>
        public HttpRequestUserClaimAccessor(IHttpRequestTokenAccessor httpRequestTokenAccessor, ITokenEngine tokenEngine)
        {
            _httpRequestTokenAccessor = httpRequestTokenAccessor ?? throw new ArgumentNullException(nameof(httpRequestTokenAccessor));
            _tokenEngine = tokenEngine ?? throw new ArgumentNullException(nameof(tokenEngine));
        }

        public Optional<TUserClaim> GetRequestUserClaim<TUserClaim>()
            where TUserClaim : class
            => _httpRequestTokenAccessor
              .GetRequestHttpToken()
              .MapOptional(token => _tokenEngine.Read<TUserClaim>(token));
    }
}
