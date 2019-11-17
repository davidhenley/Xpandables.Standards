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

using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Xpandables.EntityFrameworkCore")]

namespace System.Design.Database
{
    /// <summary>
    ///  Provides functionality to evaluate queries against the specific data context wherein the type of the data is known.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    internal interface IDataQueryable<out T> : IQueryable<T>
    {
        /// <summary>
        /// Provides with data context instance.
        /// </summary>
        internal IDataContext Instance { get; }
    }
}
