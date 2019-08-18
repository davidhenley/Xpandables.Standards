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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System.Design.TaskEvent
{
    /// <summary>
    /// Implementing the <see cref=" IAsyncTaskEventDispatcher"/> interface.
    /// </summary>
    public sealed class AsyncTaskEventDispatcher : IAsyncTaskEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes the dispatcher with a service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider to be used.</param>
        public AsyncTaskEventDispatcher(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        async Task IAsyncTaskEventDispatcher.DispatchAsync<TTaskEvent>(
            TTaskEvent taskEvent,
            CancellationToken cancellationToken)
        {
            if (taskEvent is null) throw new ArgumentNullException(nameof(taskEvent));

            var taskEvents = _serviceProvider.GetServices<IAsyncTaskEventHandler<TTaskEvent>>()
                .Map(handlers => handlers.Select(handler => handler.HandleAsync(taskEvent, cancellationToken)))
                .Cast<IEnumerable<Task>>();

            await Task.WhenAll(taskEvents).ConfigureAwait(false);
        }

        async Task IAsyncTaskEventDispatcher.DispatchAsync(
            ITaskEvent taskEvent,
            CancellationToken cancellationToken)
        {
            if (taskEvent is null) throw new ArgumentNullException(nameof(taskEvent));

            var typeHandler = typeof(IAsyncTaskEventHandler<>).MakeGenericType(new Type[] { taskEvent.GetType() });

            var taskEvents = _serviceProvider.GetServices<IAsyncTaskEventHandler>(typeHandler)
                .Map(handlers => handlers.Select(handler => handler.HandleAsync(taskEvent, cancellationToken)))
                .Cast<IEnumerable<Task>>();

            await Task.WhenAll(taskEvents).ConfigureAwait(false);
        }
    }
}