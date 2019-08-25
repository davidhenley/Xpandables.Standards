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
    /// Provides with the data context options instance.
    /// </summary>
    public interface IDataContextOptionsProvider<TDataContextOption>
        where TDataContextOption : class
    {
        /// <summary>
        /// Gets the data context options according to the environment.
        /// </summary>
        /// <returns>An implementation of <typeparamref name="TDataContextOption"/>.</returns>
        ExecutionResult<TDataContextOption> GetDataContextOptions();
    }
}