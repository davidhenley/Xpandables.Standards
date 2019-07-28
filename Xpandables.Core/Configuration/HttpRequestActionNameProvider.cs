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

using Microsoft.AspNetCore.Http;

namespace System.Configuration
{
    public sealed class HttpRequestActionNameProvider : IHttpRequestActionNameProvider
    {
        private readonly IHttpContextProvider _httpContextProvider;

        public HttpRequestActionNameProvider(IHttpContextProvider httpContextProvider)
        {
            _httpContextProvider = httpContextProvider ?? throw new ArgumentNullException(nameof(httpContextProvider));
        }

        Optional<string> IHttpRequestActionNameProvider.GetActionName()
            => _httpContextProvider.GetHttpContext<HttpContext>()
                .Map(httpContext => httpContext.Request)
                .Map(request => request.Path)
                .Map(path => path.Value)
                .Map(value => value.Substring(value.LastIndexOf('/') + 1));
    }
}
