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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System
{
    /// <summary>
    ///  When used with <see cref="Validator"/>, specifies that a data field value is required.
    ///  <para>To be used only with nested type.</para>
    /// </summary>
    /// <seealso cref="RequiredAttribute"/>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public sealed class RequiredNestedAttribute : RequiredAttribute
    {
        /// <summary>
        ///  Checks that the value of the required data field is valid.
        /// </summary>
        /// <param name="value"> The data field value to validate.</param>
        /// <param name="validationContext">The <see cref="ValidationContext"/> object that describes
        /// the context where the validation checks are performed.
        /// This parameter cannot be null.</param>
        /// <returns> An instance of <see cref="ValidationResult"/> class.</returns>
        [SuppressMessage("Design", "CA1062:Valider les arguments de méthodes publiques", Justification = "<En attente>")]
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (base.IsValid(value, validationContext) != ValidationResult.Success)
                return base.IsValid(value, validationContext);

            if (value.GetType().IsPrimitive || value is string)
            {
                return new ValidationResult(
                    ErrorMessageResources.RequiredNestedAttributeTypeMissmatched,
                    new string[] { validationContext.MemberName });
            }

            var context = new ValidationContext(value, null, null);
            var requiredValidation = base.IsValid(value, context);
            if (requiredValidation != ValidationResult.Success)
                return requiredValidation;

            var validationResults = new List<ValidationResult>();
            if (Validator.TryValidateObject(value, context, validationResults, true))
                return ValidationResult.Success;

            return new ValidationResult(
                ErrorMessage ?? validationResults.Select(result => result.ErrorMessage).StringJoin(Environment.NewLine),
                validationResults.SelectMany(result => result.MemberNames));
        }
    }
}