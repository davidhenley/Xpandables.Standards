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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Xpandables.Core.Configuration
{
    /// <summary>
    /// Provides method to configure services and the app's request pipeline.
    /// </summary>
    public interface ICustomStartup
    {
        /// <summary>
        /// Determines the execution order.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The environmement hosting.</param>
        void Configure(IApplicationBuilder app, IHostingEnvironment env);

        /// <summary>
        ///  Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        void ConfigureServices(IServiceCollection services);
    }
}
