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
    /// Defines the default process for validating an argument of type-specific.
    /// <para>You must derive from this class to implement a custom validator that match your requirement.
    /// If you want to apply many validators for one argument, see <see cref="CustomCompositeValidator{TArgument}"/>.</para>
    /// </summary>
    [Serializable]
    public class CustomValidator : ICustomValidator
    {
        /// <summary>
        /// Determines the zero-base order in which the validator will be executed.
        /// The default value is zero.
        /// </summary>
        public virtual int Order => 0;

        /// <summary>
        /// When overridden in derived class, this method will validate the specified argument.
        /// Applies the default implementation validation using service provider for validation context.
        /// </summary>
        /// <param name="argument">The target argument to be validated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="argument"/> is null.</exception>
        /// <exception cref="ValidationException">Any validation exception.</exception>
        public virtual void Validate(object argument)
            => Validator
                .ValidateObject(
                    argument,
                    new ValidationContext(argument, null, null),
                    true);
    }

    /// <summary>
    /// Defines the default process for validating an argument of type-specific.
    /// <para>You must derive from this class to implement a custom validator that match your requirement.
    /// If you want to apply many validators for one argument, see <see cref="CustomCompositeValidator{TArgument}"/>.</para>
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument to be validated.</typeparam>
    [Serializable]
    public class CustomValidator<TArgument> : CustomValidator, ICustomValidator<TArgument>
        where TArgument : class
    {
        /// <summary>
        /// When overridden in derived class, this method will validate the specified argument.
        /// Applies the default implementation validation using service provider for validation context.
        /// </summary>
        /// <param name="argument">The target argument to be validated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="argument"/> is null.</exception>
        /// <exception cref="ValidationException">Any validation exception.</exception>
        public virtual void Validate(TArgument argument) => base.Validate(argument);
    }
}