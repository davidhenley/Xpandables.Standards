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
using System.ComponentModel.DataAnnotations;

namespace System.Design
{
    /// <summary>
    /// Using with <see langword="IServiceLocalizationProvider"/>, gives access to a collection of resources types
    /// to be used for localization. The resource type is identified by its string type name and behave as the data annotations
    /// attributes localization.
    /// </summary>
    public interface ILocalizationResourceType
    {
#pragma warning disable CS1574
        /// <summary>
        /// Contains a collection of resource types to add localization for application viewmodels.
        /// Each viewmodel is associated with a resource type name that matches the viewmodel name.
        /// This behavior is available for the following attributes :
        /// <para><see cref="DisplayAttribute"/> :</para>
        /// <see cref="DisplayAttribute.Name"/> will be bounded to the <see langword="DisplayPropertyName"/> 
        /// as key in the resource file.
        /// <see cref="DisplayAttribute.Prompt"/> will be bounded to the <see langword="PromptPropertyName"/>
        /// as key in the resource file.
        /// <see cref="DisplayAttribute.Description"/> will be bound to the <see langword="DescriptionPropertyName"/>
        /// as key in the resource file.
        /// <para><see cref="LocalizedDisplayFormatAttribute"/> :</para>
        /// <see cref="LocalizedDisplayFormatAttribute.DataFormatString"/> will be bounded to the <see langword="FormatPropertyName"/> 
        /// as key in the resource.
        /// <see cref="LocalizedDisplayFormatAttribute.NullDisplayText"/> will be bounded to the
        /// <see langword="NullDisplayPropertyName"/> as key in the resource file.
        /// </summary>
        IEnumerable<Type> LocalizationTypes { get; }
#pragma warning restore CS1574
    }
}
