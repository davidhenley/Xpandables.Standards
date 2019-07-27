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

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// Specifies the target checking for the attribute.
    /// That is a set of flags.
    /// </summary>
    [Flags]
    public enum AppliedOnTarget
    {
        /// <summary>
        /// The expected value must match an email.
        /// </summary>
        Email = 0x0,

        /// <summary>
        /// The expected value must match a localized phone number.
        /// </summary>
        Phone = 0x1
    }

    /// <summary>
    /// When used with <see cref="Validator"/>, specifies that the decorated data field is required
    /// and the value must be an email or a localized phone number.
    /// </summary>
    /// <seealso cref="RequiredAttribute"/>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public sealed class RequiredEmailOrPhoneAttribute : RequiredAttribute
    {
        private const string DefaultPhoneRegex = @"^\+(?:[0-9]●?){6,14}[0-9]$";

        [NonSerialized]
        private readonly string _phoneRegex;

        private readonly AppliedOnTarget _appliedOn;

        /// <summary>
        /// Returns a new instance of <see cref="RequiredEmailOrPhoneAttribute"/> that specifies
        /// that the decorated data field is required and the value
        /// must match an email or a localized phone number.
        /// </summary>
        /// <param name="phoneRegex">The regular expression for phone validation.</param>
        /// <param name="appliedOn">the target check to apply.</param>
        public RequiredEmailOrPhoneAttribute(
            string phoneRegex = DefaultPhoneRegex, AppliedOnTarget appliedOn = AppliedOnTarget.Email)
        {
            _phoneRegex = phoneRegex;
            _appliedOn = appliedOn;
        }

        /// <summary>
        ///  Checks that the value of the required data field is valid for phone or email.
        /// </summary>
        /// <param name="value"> The data field value to validate.</param>
        /// <param name="validationContext">The <see cref="ValidationContext"/> object that describes
        /// the context where the validation checks are performed.
        /// This parameter cannot be null.</param>
        /// <returns> An instance of <see cref="ValidationResult"/> class.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (base.IsValid(value, validationContext) != ValidationResult.Success)
                return base.IsValid(value, validationContext);

            if (!(value is string stringValue))
                return new ValidationResult($"String type expected. {value.GetType().Name}");

            if ((AppliedOnTarget.Email & _appliedOn) == _appliedOn && (AppliedOnTarget.Phone & _appliedOn) == _appliedOn)
            {
                var phoneResult = PhoneValidation(stringValue);
                var emailResult = EmailValidation(stringValue);

                if (phoneResult == ValidationResult.Success || emailResult == ValidationResult.Success)
                    return ValidationResult.Success;

                return new ValidationResult(
                    $"{ phoneResult.ErrorMessage}{Environment.NewLine}{emailResult.ErrorMessage}", phoneResult.MemberNames);
            }

            if ((AppliedOnTarget.Email & _appliedOn) == _appliedOn)
            {
                return EmailValidation(stringValue);
            }

            return PhoneValidation(stringValue);

            ValidationResult EmailValidation(string expectedEmail)
            {
                if (expectedEmail.IsValidEmailAddress())
                    return ValidationResult.Success;

                return new ValidationResult(
                    ErrorMessage ?? $"{stringValue} : Address is not in a recognized format.",
                    new string[] { validationContext.MemberName });
            }

            ValidationResult PhoneValidation(string expectedPhone)
            {
                if (Regex.IsMatch(expectedPhone, _phoneRegex))
                    return ValidationResult.Success;

                return new ValidationResult(
                    ErrorMessage ?? $"{expectedPhone} is not a valid phone number.",
                    new string[] { validationContext.MemberName });
            }
        }
    }
}