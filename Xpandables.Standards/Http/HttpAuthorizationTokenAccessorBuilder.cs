/************************************************************************************************************
 * Copyright (C) 2018 Francis-Black EWANE
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
    /// A helper class used to implement the <see cref="IHttpAuthorizationTokenAccessor"/> interface.
    /// </summary>
    public sealed class HttpAuthorizationTokenAccessorBuilder : IHttpAuthorizationTokenAccessor
    {
        private readonly Func<string> _tokenAccessor;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpAuthorizationTokenAccessorBuilder"/> with the delegate to be used
        /// as <see cref="IHttpAuthorizationTokenAccessor"/> implementation.
        /// </summary>
        /// <param name="tokenAccessor">The delegate to be used when the handler will be invoked.
        /// <para>The delegate should match all the behaviors expected in
        /// the <see cref="IHttpAuthorizationTokenAccessor"/>
        /// method such as thrown exceptions.</para></param>
        /// <exception cref="ArgumentNullException">The <paramref name="tokenAccessor"/> is null.</exception>
        public HttpAuthorizationTokenAccessorBuilder(Func<string> tokenAccessor)
            => _tokenAccessor = tokenAccessor ?? throw new ArgumentNullException(
                nameof(tokenAccessor),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(HttpAuthorizationTokenAccessorBuilder),
                    nameof(tokenAccessor)));

        public Optional<string> GetToken() => _tokenAccessor();
    }
}