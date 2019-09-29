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
    /// When used with <see cref="NotifyPropertyChanged{T}"/>, makes that the decorated property will be notified
    /// when the target specified property by <see cref="Name"/> has changed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class NotifyPropertyChangedDependOnAttribute : Attribute
    {
        /// <summary>
        /// Specifies that the decorated property will be notified when the specified one has changed.
        /// <para>We advise the use of <see langword="nameof(propertyName)"/> as value.</para>
        /// </summary>
        /// <param name="name">The name of the target property which changes are notified to the decorated property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> is null.</exception>
        public NotifyPropertyChangedDependOnAttribute(string name)
            => Name = name ?? throw new ArgumentNullException(
                nameof(name),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(NotifyPropertyChangedDependOnAttribute),
                    nameof(name)));

        /// <summary>
        /// Gets the name of the target property which changes are notified to the decorated property.
        /// </summary>
        public string Name { get; }
    }
}