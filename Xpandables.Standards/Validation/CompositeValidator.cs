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

using System.Collections.Generic;
using System.Linq;
using Xpandables;

namespace System.Patterns
{
    /// <summary>
    /// The composite validator used to wrap all validators for a specific type.
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument to be validated</typeparam>
    [Serializable]
    public sealed class CompositeValidator<TArgument> : Explicit<ICompositeValidator<TArgument>>, ICompositeValidator<TArgument>
        where TArgument : class
    {
        private readonly IEnumerable<IValidator<TArgument>> _validators;

        /// <summary>
        /// Initializes the composite validator with all validators for the argument.
        /// </summary>
        /// <param name="validators"></param>
        public CompositeValidator(IEnumerable<IValidator<TArgument>> validators)
            => _validators = validators ?? Enumerable.Empty<IValidator<TArgument>>();

        void ICompositeValidator<TArgument>.Validate(TArgument argument)
        {
            foreach (var validator in _validators.OrderBy(o => o.Order))
                validator.Validate(argument);
        }

        void ICompositeValidator.Validate(object argument) => Instance.Validate(argument as TArgument);
    }
}