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

using System.ComponentModel.DataAnnotations;

namespace System
{
    /// <summary>
    /// Provides with the extension method for <see cref="StringEscapeValidationAttribute"/>.
    /// </summary>
    public static class StringEscapeExtensions
    {
        /// <summary>
        /// Escapes special characters from the target string.
        /// </summary>
        /// <param name="value">The string to act on.</param>
        public static string StringEscape(this string value)
            => new string(
                Array.FindAll(
                    value.ToCharArray(),
                    c => char.IsLetterOrDigit(c)
                        || char.IsWhiteSpace(c)
                        || c == '-'));
    }

    /// <summary>
    /// Escapes special characters from the decorated property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class StringEscapeValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext is null)
            {
                throw new ArgumentNullException(
                    nameof(validationContext),
                    ErrorMessageResources.ArgumentExpected.StringFormat(
                        nameof(StringEscapeValidationAttribute),
                        nameof(validationContext)));
            }

            if (value is string stringValue)
            {
                value = stringValue.StringEscape();

                validationContext
                    .ObjectType
                    .GetProperty(validationContext.MemberName)
                    .SetValue(validationContext.ObjectInstance, value);
            }

            return base.IsValid(value, validationContext);
        }
    }
}
