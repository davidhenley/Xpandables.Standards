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
    /// Defines a method used to retrieve the ambient user claims from the current HTTP request header.
    /// </summary>
    public interface IHttpRequestUserClaimAccessor
    {
        /// <summary>
        /// Returns a instance of <typeparamref name="TUserClaim"/>.
        /// </summary>
        /// <typeparam name="TUserClaim">The type of user claims.</typeparam>
        Optional<TUserClaim> GetRequestUserClaim<TUserClaim>() where TUserClaim : class;
    }
}