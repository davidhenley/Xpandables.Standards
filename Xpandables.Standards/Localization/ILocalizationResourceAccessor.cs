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

namespace System.Design
{
    /// <summary>
    /// Using with <see langword="ServiceLocazationProvider"/>, gives access to a dictionary of application resources types
    /// to be used for localization. The resource type is identified by its string type name and behave as the data annotations
    /// attributes localization.
    /// </summary>
    public interface ILocalizationResourceAccessor : IFluent
    {
#pragma warning disable CS1574 // Désolé... Nous ne pouvons pas résoudre l'attribut cref du commentaire XML
        /// <summary>
        /// Contains a collection of resource types to add localization for application viewmodels.
        /// Each viewmodel is associated with a resource type name that matches the viewmodel name.
        /// This behavior is available for the following types :
        /// <para><see cref="DisplayAttribute"/> :</para>
        /// <para><see cref="DisplayAttribute.Name"/> will be bounded to the <see langword="DisplayPropertyName"/> as key in the resource file.</para> 
        /// <para><see cref="DisplayAttribute.Prompt"/> will be bounded to the <see langword="PromptPropertyName"/> as key in the resource file.</para> 
        /// <para><see cref="DisplayAttribute.Description"/> will be bound to the <see langword="DescriptionPropertyName"/> as key in the resource file.</para> 
        /// <para><see cref="LocalizedDisplayFormatAttribute"/> :</para>
        /// <para><see cref="LocalizedDisplayFormatAttribute.DataFormatString"/> will be bounded to the <see langword="FormatPropertyName"/> 
        /// as key in the resource</para>
        /// <para><see cref="LocalizedDisplayFormatAttribute.NullDisplayText"/> will be bounded to the <see langword="NullDisplayPropertyName"/>
        /// as key in the resource file.</para>
        /// </summary>
        ICorrelationCollection<string, Type> LocalizationTypes { get; }

        /// <summary>
        /// Contains the resource type for all validation attributes localization using the attribute name as a key.
        /// <para>For example :</para>
        /// <see cref="RequiredAttribute.ErrorMessageResourceName"/> will be bounded to the <see langword="RequiredAttribute"/> as key in the resource file.
        /// </summary>
        Type ValidationAttributeResource { get; }
    }
#pragma warning restore CS1574 // Désolé... Nous ne pouvons pas résoudre l'attribut cref du commentaire XML
}
