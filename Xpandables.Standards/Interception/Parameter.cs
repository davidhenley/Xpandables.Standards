﻿/************************************************************************************************************
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

using System.Reflection;

namespace System.Interception
{
#pragma warning disable CA2235 // Mark all non-serializable fields

    /// <inheritdoc />
    /// <summary>
    /// Defines the structure of a argument of a method at runtime. This class is <see langword="serializable"/>.
    /// </summary>
    [Serializable]
    public sealed class Parameter
    {
        /// <summary>
        /// Builds a new instance of <see cref="Parameter"/> with the position, name , value...
        /// </summary>
        /// <param name="position">The parameter position in the method signature</param>
        /// <param name="source">The parameter info to act on.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>An instance of new <see cref="Parameter"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="position"/> must be greater
        /// or equal to zero.</exception>
        public static Parameter BuildWith(int position, ParameterInfo source, object value)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            return new Parameter(
                position,
                source.Name,
                value,
                GetTypeFromParameterInfo(source),
                GetPassedStatusFromParameterInfo(source));
        }

        private Parameter(int position, string name, object value, Type type, PassedStatus isPassed)
        {
            if (position < 0)
                throw new ArgumentOutOfRangeException($"{position} must be greater or equal to zero.");

            Position = position;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            IsPassed = isPassed;
        }

        /// <summary>
        /// Gets the index position of the parameter in the method signature.
        /// The value must be greater or equal to zero, otherwise the interface contract
        /// will throw an <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// Gets the name of the parameter as defined in the method signature.
        /// The value can not be null, otherwise the interface contract will throw an <see cref="ArgumentNullException"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the parameter at runtime.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Sets a new value to the parameter.
        /// The new value type must match the argument <see cref="Type"/>,
        /// otherwise it will throw a <see cref="FormatException"/>
        /// </summary>
        /// <param name="newValue">The new value to be used.</param>
        public Parameter ChangeValueTo(object newValue)
        {
            Value = newValue;
            return this;
        }

        /// <summary>
        /// Gets the type of the argument.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Determines whether the argument is <see langword="out"/>, <see langword="in"/>
        /// or by <see langword="ref"/> parameter.
        /// </summary>
        public PassedStatus IsPassed { get; }

        /// <summary>
        /// Determines whether the argument is <see langword="out"/>, <see langword="in"/>
        /// or <see langword="ref"/> parameter.
        /// </summary>
        [Serializable]
        public enum PassedStatus
        {
            /// <summary>
            /// Standard parameter.
            /// </summary>
            In = 0,

            /// <summary>
            /// <see langword="out"/> parameter.
            /// </summary>
            Out = 1,

            /// <summary>
            /// <see langword="ref"/> parameter.
            /// </summary>
            Ref = 2
        }

        /// <summary>
        /// Returns the <see cref="PassedStatus"/> of the parameter.
        /// </summary>
        /// <param name="parameterInfo">The parameter to act on.</param>
        /// <returns>A <see cref="PassedStatus"/> that matches the parameter.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo"/> is null.</exception>
        public static PassedStatus GetPassedStatusFromParameterInfo(ParameterInfo parameterInfo)
        {
            if (parameterInfo is null)
                throw new ArgumentNullException(nameof(parameterInfo));

            return parameterInfo.IsOut
                ? PassedStatus.Out
                : parameterInfo.ParameterType.IsByRef
                    ? PassedStatus.Ref
                    : PassedStatus.In;
        }

        /// <summary>
        /// Returns the type of the parameter.
        /// </summary>
        /// <param name="parameterInfo">The parameter to act on.</param>
        /// <returns>The parameter type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo"/> is null.</exception>
        public static Type GetTypeFromParameterInfo(ParameterInfo parameterInfo)
        {
            if (parameterInfo is null)
                throw new ArgumentNullException(nameof(parameterInfo));

            return parameterInfo.ParameterType.IsByRef
                       ? parameterInfo.ParameterType.GetElementType()
                       : parameterInfo.ParameterType;
        }
    }
}