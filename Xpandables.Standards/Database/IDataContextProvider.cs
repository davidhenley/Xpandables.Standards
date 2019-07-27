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

namespace System.Data
{
    /// <summary>
    /// Provides with the data context instance.
    /// </summary>
    /// <remarks>
    /// Any operation that does not deliver or do what it promises to do should throw an exception.
    /// </remarks>
    public interface IDataContextProvider
    {
        /// <summary>
        /// Gets the ambient data context according to the environment.
        /// </summary>
        /// <returns>An implementation of <see cref="IDataContext" />.</returns>
        /// <exception cref="InvalidOperationException">Creating the context failed. See inner exception.</exception>
        IDataContext GetDataContext();
    }
}