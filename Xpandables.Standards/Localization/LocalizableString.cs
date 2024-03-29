﻿/************************************************************************************************************
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
using System.Reflection;

namespace System.Design
{
#nullable disable
    /// <summary>
    ///     A helper class for providing a localizable string property.
    ///     This class is currently compiled in both System.Web.dll and System.ComponentModel.DataAnnotations.dll.
    /// </summary>
    internal class LocalizableString
    {
        private readonly string _propertyName;
        private Func<string> _cachedResult;
        private string _propertyValue;
        private Type _resourceType;

        /// <summary>
        ///     Constructs a localizable string, specifying the property name associated
        ///     with this item.  The <paramref name="propertyName" /> value will be used
        ///     within any exceptions thrown as a result of localization failures.
        /// </summary>
        /// <param name="propertyName">
        ///     The name of the property being localized.  This name
        ///     will be used within exceptions thrown as a result of localization failures.
        /// </param>
        public LocalizableString(string propertyName)
        {
            _propertyName = propertyName;
        }

        /// <summary>
        ///     Gets or sets the value of this localizable string.  This value can be
        ///     either the literal, non-localized value, or it can be a resource name
        ///     found on the resource type supplied to <see cref="GetLocalizableValue" />.
        /// </summary>
        public string Value
        {
            get => _propertyValue;
            set
            {
                if (_propertyValue != value)
                {
                    ClearCache();
                    _propertyValue = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the resource type to be used for localization.
        /// </summary>
        public Type ResourceType
        {
            get => _resourceType;
            set
            {
                if (_resourceType != value)
                {
                    ClearCache();
                    _resourceType = value;
                }
            }
        }

        /// <summary>
        ///     Clears any cached values, forcing <see cref="GetLocalizableValue" /> to
        ///     perform evaluation.
        /// </summary>
        private void ClearCache()
        {
            _cachedResult = null;
        }

        /// <summary>
        ///     Gets the potentially localized value.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ResourceType" /> has been specified and <see cref="Value" /> is not
        ///     null, then localization will occur and the localized value will be returned.
        ///     <para>
        ///         If <see cref="ResourceType" /> is null then <see cref="Value" /> will be returned
        ///         as a literal, non-localized string.
        ///     </para>
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if localization fails.  This can occur if <see cref="ResourceType" /> has been
        ///     specified, <see cref="Value" /> is not null, but the resource could not be
        ///     accessed.  <see cref="ResourceType" /> must be a public class, and <see cref="Value" />
        ///     must be the name of a public static string property that contains a getter.
        /// </exception>
        /// <returns>
        ///     Returns the potentially localized value.
        /// </returns>
        public string GetLocalizableValue()
        {
            if (_cachedResult == null)
            {
                // If the property value is null, then just cache that value
                // If the resource type is null, then property value is literal, so cache it
                if (_propertyValue == null || _resourceType == null)
                {
                    _cachedResult = () => _propertyValue;
                }
                else
                {
                    // Get the property from the resource type for this resource key
                    var property = _resourceType.GetRuntimeProperty(_propertyValue);

                    // We need to detect bad configurations so that we can throw exceptions accordingly
                    var badlyConfigured = false;

                    // Make sure we found the property and it's the correct type, and that the type itself is public
                    if (!_resourceType.IsVisible || property == null ||
                        property.PropertyType != typeof(string))
                    {
                        badlyConfigured = true;
                    }
                    else
                    {
                        // Ensure the getter for the property is available as public static
                        // TODO - check that GetMethod returns the same as old GetGetMethod()
                        // in all situations regardless of modifiers
                        var getter = property.GetMethod;
                        if (getter == null || !(getter.IsPublic && getter.IsStatic))
                        {
                            badlyConfigured = true;
                        }
                    }

                    // If the property is not configured properly, then throw a missing member exception
                    if (badlyConfigured)
                    {
                        string exceptionMessage = ErrorMessageResources.LocalizationFailed.StringFormat(_propertyName, _resourceType.FullName, _propertyValue);
                        _cachedResult = () => throw new InvalidOperationException(exceptionMessage);
                    }
                    else
                    {
                        // We have a valid property, so cache the resource
                        _cachedResult = () => (string)property.GetValue(null, null);
                    }
                }
            }

            // Return the cached result
            return _cachedResult();
        }
    }
#nullable enable
}
