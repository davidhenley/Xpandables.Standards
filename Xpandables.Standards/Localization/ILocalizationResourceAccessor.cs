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

namespace System.Design
{
    /// <summary>
    /// Using with <see langword="ServiceLocazationProvider"/>, gives access to a dictionary of application resources types
    /// to be used for localization. The resource type is identified by its string type name and behave as the data annotations
    /// attributes localization. For internal use.
    /// </summary>
    public interface ILocalizationResourceAccessor : IFluent
    {
        /// <summary>
        /// Contains a collection of resource types to add localization for application viewmodels.
        /// Each viewmodel is associated with a resource type name that matches the viewmodel name.
        /// </summary>
        ICorrelationCollection<string, Type> LocalizationTypes { get; }

        /// <summary>
        /// Contains the resource type for all validation attributes localization using the attribute name as a key.
        /// </summary>
        Type LocalizationValidationType { get; }
    }
}
