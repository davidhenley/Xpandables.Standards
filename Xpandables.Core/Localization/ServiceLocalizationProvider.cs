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

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace System.Design
{
    /// <summary>
    /// Provides with data annotations binder for localization.
    /// </summary>
    public sealed class ServiceLocalizationProvider : IModelValidatorProvider, IModelBinderProvider
    {
        private readonly ILocalizationResourceAccessor _localizationResourceAccessor;

        /// <summary>
        /// Initializes the provider with the localization resource.
        /// </summary>
        /// <param name="localizationResourceAccessor">The localization resource accessor to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="localizationResourceAccessor"/> is null.</exception>
        public ServiceLocalizationProvider(ILocalizationResourceAccessor localizationResourceAccessor)
        {
            _localizationResourceAccessor = localizationResourceAccessor ?? throw new ArgumentNullException(nameof(localizationResourceAccessor));
        }

        public void CreateValidators(ModelValidatorProviderContext context)
        {
            BindDisplayAttribute((DefaultModelMetadata)context.ModelMetadata);
            BindDisplayFormatAttribute((DefaultModelMetadata)context.ModelMetadata);

            foreach (var validator in context.ValidatorMetadata.Cast<ValidationAttribute>())
                BindValidationAttribute(validator);
        }

#nullable disable
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            foreach (var property in ((DefaultModelMetadata)context.Metadata).Properties)
            {
                BindDisplayAttribute((DefaultModelMetadata)property);
                BindDisplayFormatAttribute((DefaultModelMetadata)property);

                foreach (var validator in property.ValidatorMetadata.Cast<ValidationAttribute>())
                    BindValidationAttribute(validator);
            }

            return null;
        }

#nullable enable

        private void BindDisplayAttribute(DefaultModelMetadata modelMetadata)
            => modelMetadata
                .Attributes
                .PropertyAttributes
                .AsOptional()
                .Map(attributes => attributes
                    .FirstOrEmpty(attr => attr is DisplayAttribute)
                    .CastOptional<DisplayAttribute>()
                    .Map(attribute =>
                    {
                        attribute.Name = $"Display{modelMetadata.Name}";
                        attribute.Prompt = $"Prompt{modelMetadata.Name}";
                        attribute.Description = $"Description{modelMetadata.Name}";
                        attribute.ResourceType = _localizationResourceAccessor.LocalizationTypes[modelMetadata.ContainerType.Name];
                    }));

        private void BindDisplayFormatAttribute(DefaultModelMetadata modelMetadata)
            => modelMetadata
                .Attributes
                .PropertyAttributes
                .AsOptional()
                .Map(attributes => attributes
                    .FirstOrEmpty(attr => attr is LocalizedDisplayFormatAttribute)
                    .CastOptional<LocalizedDisplayFormatAttribute>()
                    .Map(attribute =>
                    {
                        attribute.DataFormatString = $"Format{modelMetadata.Name}";
                        attribute.NullDisplayText = $"NullDisplay{modelMetadata.Name}";
                        attribute.DataFormatStringResourceType = _localizationResourceAccessor.LocalizationTypes[modelMetadata.ContainerType.Name];
                        attribute.NullDisplayTextResourceType = _localizationResourceAccessor.LocalizationTypes[modelMetadata.ContainerType.Name];
                    }));

        private void BindValidationAttribute(ValidationAttribute validator)
        {
            validator.ErrorMessageResourceType = _localizationResourceAccessor.ValidationAttributeResource;
            validator.ErrorMessageResourceName = ((Type)validator.TypeId).Name;
        }
    }
}
