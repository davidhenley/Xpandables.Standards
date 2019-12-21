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

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace System.Http
{
    /// <summary>
    /// Provides with a handler that can be used with <see cref="HttpClient"/> to add header authorization value
    /// before request execution.
    /// </summary>
    public class HttpAuthorizationTokenHandler : HttpClientHandler
    {
        private readonly IHttpRequestTokenAccessor _httpRequestTokenAccessor;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpAuthorizationTokenHandler"/> with the token accessor.
        /// </summary>
        /// <param name="httpRequestTokenAccessor">The token accessor to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="httpRequestTokenAccessor"/> is null.</exception>
        public HttpAuthorizationTokenHandler(IHttpRequestTokenAccessor httpRequestTokenAccessor)
        {
            _httpRequestTokenAccessor = httpRequestTokenAccessor ?? throw new ArgumentNullException(nameof(httpRequestTokenAccessor));
        }

        /// <summary>
        /// Creates an instance of System.Net.Http.HttpResponseMessage based on the information
        /// provided in the System.Net.Http.HttpRequestMessage as an operation that will not block.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> is null.</exception>
        [Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2208:Instancier les exceptions d'argument correctement",
            Justification = "<En attente>")]
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            if (request.Headers.Authorization is AuthenticationHeaderValue authorization)
            {
                string token = _httpRequestTokenAccessor.GetRequestHttpToken().GetValueOrDefault()
                    ?? throw new InvalidOperationException(
                        ErrorMessageResources.ArgumentExpected.StringFormat(
                            nameof(HttpAuthorizationTokenHandler),
                            nameof(IHttpRequestTokenAccessor.GetRequestHttpToken)),
                    new ArgumentNullException("Expected token not found."));

                request.Headers.Authorization = new AuthenticationHeaderValue(authorization.Scheme, token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
