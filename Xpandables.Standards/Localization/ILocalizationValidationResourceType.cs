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
    /// Using with <see langword="IServiceLocalizationProvider"/>, gives access to the validation resource type attribute
    /// to be used for localization. The resource type is identified as a type.
    /// </summary>
    public interface ILocalizationValidationResourceType
    {
#pragma warning disable CS1574
        /// <summary>
        /// Contains the resource type for all validation attributes localization using the attribute name as a key.
        /// <para>For example :</para>
        /// <see cref="RequiredAttribute.ErrorMessageResourceName"/> will be bounded to the <see langword="RequiredAttribute"/>
        /// as key in the resource file.
        /// </summary>
        Type ValidationAttributeResourceType { get; }
    }
#pragma warning restore CS1574
}
