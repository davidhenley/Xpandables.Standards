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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Implementing the <see cref=" IEventDispatcher"/> interface.
    /// </summary>
    public sealed class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes the dispatcher with a service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider to be used.</param>
        public EventDispatcher(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        public Task DispatchAsync<T>(T source, CancellationToken cancellationToken)
            where T : class, IEvent
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            var tasks = _serviceProvider
                .GetServices<IEventHandler<T>>()
                .Select(handler => handler.HandleAsync(source));

            return Task.WhenAll(tasks);
        }

        public Task DispatchAsync(IEvent source, CancellationToken cancellationToken)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            var typeHandler = typeof(IEventHandler<>).MakeGenericType(new Type[] { source.GetType() });

            var tasks = _serviceProvider
                .GetServices<IEventHandler>(typeHandler)
                .Select(handler => handler.HandleAsync(source));

            return Task.WhenAll(tasks);
        }
    }
}