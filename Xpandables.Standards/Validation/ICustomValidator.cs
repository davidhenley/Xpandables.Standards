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

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Defines a method contract used to validate an argument.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface ICustomValidator
    {
        /// <summary>
        /// Applies validation process and throws the <see cref="ValidationException"/> if necessary.
        /// </summary>
        /// <param name="argument">The target argument to be validated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="argument"/> is null.</exception>
        /// <exception cref="ValidationException">Any validation exception.</exception>
        void Validate(object argument);

        /// <summary>
        /// Determines the order for the underlying object.
        /// </summary>
        int Order { get; }
    }

    /// <summary>
    /// Defines a method contract used to validate a type-specific argument.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument to be validated.</typeparam>
    public interface ICustomValidator<in TArgument> : ICustomValidator
        where TArgument : class
    {
        /// <summary>
        /// Applies validation the argument and throws the <see cref="ValidationException"/> if necessary.
        /// </summary>
        /// <param name="argument">The target argument to be validated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="argument"/> is null.</exception>
        /// <exception cref="ValidationException">Any validation exception.</exception>
        void Validate(TArgument argument);
    }
}