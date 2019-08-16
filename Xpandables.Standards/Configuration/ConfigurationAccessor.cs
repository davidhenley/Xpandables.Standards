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

using Microsoft.Extensions.Configuration;

namespace System.Configuration
{
    /// <summary>
    /// The default implementation of <see cref="IConfigurationAccessor"/>.
    /// </summary>
    public sealed class ConfigurationAccessor : IConfigurationAccessor
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes the accessor with the configuration instance.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        public ConfigurationAccessor(IConfiguration configuration) => _configuration = configuration;

        Optional<string> IConfigurationAccessor.GetConnectionString(string key) => _configuration.GetConnectionString(key);

        Optional<T> IConfigurationAccessor.GetSection<T>(string key) => _configuration.GetSection(key).Get<T>();

        Optional<T> IConfigurationAccessor.GetValue<T>(string key) => _configuration.GetValue<T>(key);
    }
}