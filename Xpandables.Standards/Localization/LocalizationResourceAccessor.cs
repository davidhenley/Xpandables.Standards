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

using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Design
{
    public sealed class LocalizationResourceAccessor : ILocalizationResourceAccessor
    {
        public LocalizationResourceAccessor(
            [NotNull] ILocalizationResourceType localizationResourceType,
            [NotNull] ILocalizationValidationResourceType localizationValidationResourceType)
        {
            if (localizationResourceType is null) throw new ArgumentNullException(nameof(localizationResourceType));
            if (localizationValidationResourceType is null) throw new ArgumentNullException(nameof(localizationValidationResourceType));

            if (localizationResourceType.LocalizationTypes?.Any() != true)
                throw new ArgumentNullException(nameof(localizationResourceType));
            if (localizationValidationResourceType.ValidationAttributeResourceType is null)
                throw new ArgumentNullException(nameof(localizationValidationResourceType));

            LocalizationTypes = new CorrelationCollection<string, Type>();
            foreach (var type in localizationResourceType.LocalizationTypes)
                LocalizationTypes.AddOrUpdateValue(type.Name, type);

            LocalizationValidationType = localizationValidationResourceType.ValidationAttributeResourceType;
        }

        public ICorrelationCollection<string, Type> LocalizationTypes { get; }

        public Type LocalizationValidationType { get; }
    }
}
