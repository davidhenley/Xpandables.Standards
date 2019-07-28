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
    /// Resolves the HttpContextAccessor that provides the current HttpContext instance.
    /// </summary>
    public interface IHttpContextProvider
    {
        /// <summary>
        /// Returns the current http context instance.
        /// If not found, returns an empty optional.
        /// </summary>
        Optional<object> GetHttpContext();
    }

    /// <summary>
    /// Extension methods for <see cref="IHttpContextProvider"/>.
    /// </summary>
    public static class HttpContextProviderHelpers
    {
        /// <summary>
        /// Returns the current http context instance and converts it to the specified type.
        /// If not found, returns and empty optional.
        /// </summary>
        /// <typeparam name="T">Type of http context to be retrieved.</typeparam>
        public static Optional<T> GetHttpContext<T>(this IHttpContextProvider httpContextProvider)
            where T : class
            => httpContextProvider?.GetHttpContext().OfTypeOptional<T>();
    }
}