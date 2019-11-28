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

using System.Design;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies how data fields are localized, displayed and formatted by ASP.NET Dynamic Data.
    ///  Allows overriding various display-related options for a given field. The options have the same meaning as in BoundField.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class LocalizedDisplayFormatAttribute : DisplayFormatAttribute
    {
        private readonly LocalizableString _dataFormatString = new LocalizableString(nameof(DataFormatString));

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LocalizedDisplayFormatAttribute() { }

        /// <summary>
        ///  Gets or sets the format string, which may be a resource key string.
        /// </summary>
        [Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1721:Les noms de propriétés ne doivent pas correspondre à ceux des méthodes get", Justification = "<En attente>")]
        public new string DataFormatString
        {
            get => _dataFormatString.Value;
            set => _dataFormatString.Value = value;
        }

        /// <summary>
        /// Gets or sets the type that contains the resources for <see cref="DataFormatString"/>.
        /// Using the <see cref="DataFormatStringResourceType"/> along with <see cref="DataFormatString"/>,
        /// allows the <see cref="GetDataFormatString"/> method to return localized values.
        /// </summary>
        public Type DataFormatStringResourceType
        {
            get => _dataFormatString.ResourceType;
            set => _dataFormatString.ResourceType = value;
        }

        /// <summary>
        /// Returns the UI format string for <see cref="DataFormatString"/>.
        /// When <see cref="DataFormatStringResourceType" /> has not been specified, the value of <see cref="DataFormatString" /> 
        /// will be returned. When <see cref="DataFormatStringResourceType" /> has been specified and <see cref="DataFormatString" />
        /// represents a resource key within that resource type, then the localized value will be returned. When <see cref="DataFormatString" />
        /// and <see cref="DataFormatStringResourceType" /> have not been set, returns <c>null</c>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// After setting both the <see cref="DataFormatStringResourceType" /> property and the <see cref="DataFormatString" /> property,
        /// but a public static property with a name matching the <see cref="DataFormatString" /> value couldn't be found
        /// on the <see cref="DataFormatStringResourceType" />.
        /// </exception>
        [Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1721:Les noms de propriétés ne doivent pas correspondre à ceux des méthodes get", Justification = "<En attente>")]
        public string GetDataFormatString() => _dataFormatString.GetLocalizableValue();
    }
}
