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

using System.ComponentModel;

namespace System
{
    /// <summary>
    /// Interface that is used to build fluent interfaces and hides methods declared by <see cref="object"/> from IntelliSense.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IFluent
    {
        /// <summary>
        /// Re declaration that hides the <see cref="object.GetType()"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetType();

        /// <summary>
        /// Re declaration that hides the <see cref="object.GetHashCode()"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        /// <summary>
        /// Re declaration that hides the <see cref="object.ToString()"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();

        /// <summary>
        /// Re declaration that hides the <see cref="object.Equals(object)"/> method from IntelliSense.
        /// </summary>
        /// <param name="obj"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);
    }
}
