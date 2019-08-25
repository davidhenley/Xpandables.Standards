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

using System.Diagnostics;

namespace System
{
    /// <summary>
    /// Defines a representation of an interval of a specific type.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    [Serializable]
    [DebuggerDisplay("{Starting}, {Ending}")]
    public sealed class Interval<T> : IFluent
    {
        /// <summary>
        /// Returns a new instance of <see cref="Interval{TValue}"/> with the specified values.
        /// </summary>
        /// <param name="starting">The starting value</param>
        /// <param name="ending">The ending value</param>
        public Interval(T starting, T ending)
        {
            Starting = starting;
            Ending = ending;
        }

        /// <summary>
        /// Contains the starting value.
        /// </summary>
        public T Starting { get; }

        /// <summary>
        /// Contains the ending value.
        /// </summary>
        public T Ending { get; }
    }
}