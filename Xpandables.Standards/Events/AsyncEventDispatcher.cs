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
    /// Implementing the <see cref=" IAsyncEventDispatcher"/> interface.
    /// </summary>
    public sealed class AsyncEventDispatcher : IAsyncEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes the dispatcher with a service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider to be used.</param>
        public AsyncEventDispatcher(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        async Task IAsyncEventDispatcher.DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        {
            if (@event is null) throw new ArgumentNullException(nameof(@event));

            var taskEvents = from handler in _serviceProvider.GetServices<IAsyncEventHandler<TEvent>>()
                             select handler.HandleAsync(@event, cancellationToken);

            await Task.WhenAll(taskEvents).ConfigureAwait(false);
        }

        async Task IAsyncEventDispatcher.DispatchAsync(IEvent @event, CancellationToken cancellationToken)
        {
            if (@event is null) throw new ArgumentNullException(nameof(@event));

            var typeHandler = typeof(IAsyncEventHandler<>).MakeGenericType(new Type[] { @event.GetType() });
            var handlers = _serviceProvider.GetServices<IAsyncEventHandler>(typeHandler);

            var taskEvents = from handler in handlers
                             select handler.HandleAsync(@event, cancellationToken);

            await Task.WhenAll(taskEvents).ConfigureAwait(false);
        }
    }
}