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
using System.Design.Mediator;

namespace System.Design.DependencyInjection
{
    /// <summary>
    ///  Provides method to register <see cref="IProcessor"/>.
    /// </summary>
    public static class ProcessorServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IProcessor"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="decorateWith">The decorator to be added with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXProcessor(
            this IServiceCollection services,
            Decorators decorateWith = Decorators.None)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddTransient<IProcessor, Processor>();
            if ((decorateWith & Decorators.Validation) == Decorators.Validation)
                services.XTryDecorate<IProcessor, ProcessorValidationDecorator>();
            if ((decorateWith & Decorators.Persistence) == Decorators.Persistence)
                services.XTryDecorate<IProcessor, ProcessorPersistenceDecorator>();
            if ((decorateWith & Decorators.Transaction) == Decorators.Transaction)
                services.XTryDecorate<IProcessor, ProcessorTransactionDecorator>();
            if ((decorateWith & Decorators.EventRegister) == Decorators.EventRegister)
                services.XTryDecorate<IProcessor, ProcessorEventRegisterDecorator>();

            return services;
        }
    }
}
