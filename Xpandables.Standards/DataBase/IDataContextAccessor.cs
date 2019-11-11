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

namespace System.Design.Database
{
    /// <summary>
    /// Provides with a method to access the ambient data context instance.
    /// You need to register the class implementation to the services collection using the extension method :
    /// <see langword="AddXDataContext{TDataContextAccessor}"/> where "TDataContextAccessor" will be your class implementation.
    /// </summary>
    public interface IDataContextAccessor
    {
        /// <summary>
        /// Returns an optional instance that may contain the ambient data context according to the environment.
        /// You need to use one of the <see cref="Optional{T}.WhenEmpty(Func{T})"/> method in case of empty result 
        /// and/or one of <see cref="Optional{T}.WhenException(Func{Exception, T})"/> method in case of exception.
        /// </summary>
        /// <returns>An instance of <see cref="Optional{T}"/> of <see cref="IDataContext" />.</returns>
        Optional<IDataContext> GetDataContext();
    }
}