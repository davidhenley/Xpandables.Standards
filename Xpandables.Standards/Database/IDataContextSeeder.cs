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
    /// Allows an application author to dynamically seed a data context before it's retrieved.
    /// This is useful when you need a data context not to be empty.
    /// The target data context should be decorated with the <see cref="ISeederBehavior"/> interface and the class seeder implementation should be
    /// registered to services collections with the extension method <see langword="AddXDataContextSeederBehavior"/>.
    /// </summary>
    public interface IDataContextSeeder
    {
        /// <summary>
        /// Seeds the specified data context as you wish.
        /// Warning : Do not throw exception from this method unless it's absolutely necessary.
        /// This method get called by the <see cref="DataContextSeederBehavior"/>.
        /// </summary>
        /// <param name="dataContext">The data context instance to act on.</param>
        /// <returns>A seeded data context.</returns>
        IDataContext Seed(IDataContext dataContext);
    }
}