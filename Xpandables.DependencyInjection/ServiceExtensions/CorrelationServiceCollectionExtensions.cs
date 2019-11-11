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

using Microsoft.Extensions.DependencyInjection;

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Provides methods to register correlation task behavior to services.
    /// </summary>
    public static class CorrelationServiceCollectionExtensions
    {
        /// <summary>
        /// Adds correlation behavior to commands and queries that are decorated with the <see cref="ICorrelationBehavior"/> to the services
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCorrelationBehavior(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<CorrelationTask>();
            services.AddScoped<ICorrelationTask>(provider => provider.GetRequiredService<CorrelationTask>());

            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandCorrelationBehavior<>));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryCorrelationBehavior<,>));
            return services;
        }
    }
}
