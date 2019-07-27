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
    /// Allows an application author to dynamically seed a data context.
    /// This is useful when you need a data context not to be empty.
    /// To be used with <see cref="DataContextSeederDecorator"/>.
    /// </summary>
    public interface IDataContextSeeder
    {
        /// <summary>
        /// Seeds the specified data context.
        /// </summary>
        /// <param name="dataContext">The data context instance to act on.</param>
        void SeedDataContext(IDataContext dataContext);
    }
}