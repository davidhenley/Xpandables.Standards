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

namespace System
{
    /// <summary>
    /// Allows an application author to access the configuration file.
    /// </summary>
    public interface IConfigurationAccessor
    {
        /// <summary>
        /// Gets the configuration value with the specified key.
        /// If not found, returns an empty optional.
        /// </summary>
        /// <typeparam name="T">Type of the value to be returned.</typeparam>
        /// <param name="key">The key of the section.</param>
        Optional<T> GetValue<T>(string key) where T : class;

        /// <summary>
        /// Gets the configuration section with the specified key.
        /// If not found, returns an empty optional.
        /// </summary>
        /// <typeparam name="T">Type of the section to be return.</typeparam>
        /// <param name="key"></param>
        Optional<T> GetSection<T>(string key) where T : class;

        /// <summary>
        /// Gets the connection string with the specified key.
        /// If not found, returns an empty optional.
        /// </summary>
        /// <param name="key">The key of the connection string section.</param>
        Optional<string> GetConnectionString(string key);
    }
}