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
    /// Defines the default implementation for validating an argument of type-specific.
    /// <para>You must derive from this class to implement a custom validator that match your requirement.
    /// If you want to apply many validators for one argument, see <see cref="CompositeValidatorRule{TArgument}"/>.</para>
    /// </summary>
    [Serializable]
    public class ValidationRule : IValidationRule { }

    /// <summary>
    /// Defines the default implementation for validating an argument of type-specific.
    /// <para>You must derive from this class to implement a custom validator that match your requirement.
    /// If you want to apply many validators for one argument, see <see cref="CompositeValidatorRule{TArgument}"/>.</para>
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument to be validated.</typeparam>
    [Serializable]
    public class ValidationRule<TArgument> : ValidationRule, IValidationRule<TArgument>
        where TArgument : class
    { }
}