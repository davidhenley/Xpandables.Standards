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

namespace System.GraphQL
{
    /// <summary>
    /// Provides with a method that returns specific meta-data type from specific data source.
    /// </summary>
    /// <typeparam name="TObject">The expected object type.</typeparam>
    /// <typeparam name="TSource">The data source type.</typeparam>
    public interface IObjectBuilder<in TSource, out TObject>
        where TSource : class
        where TObject : class
    {
        /// <summary>
        /// Returns an object for the specified source type.
        /// </summary>
        /// <param name="source">An instance of the data source to act on.</param>
        /// <returns>A specific meta-data type from the source type instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        TObject BuildObjectFrom(TSource source);
    }
}
