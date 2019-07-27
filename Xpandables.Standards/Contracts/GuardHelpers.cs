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

namespace System
{
    /// <summary>
    ///  Contains static helper methods for <see cref="Guard{T}"/>.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Creates a new instance of <see cref="Guard{T}"/> that contains a non-null value of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">An instance of the value.</param>
        /// <returns>A guard holding a non-null value of <typeparamref name="T"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public static Guard<T> Create<T>(T value) where T : class => new Guard<T>(value);
    }
}