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

using System.Design.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace System.Design
{
    /// <summary>
    /// The default implementation for <see cref="IEventDispatcher"/>.
    /// Implements methods to execute the <see cref="IEventHandler{T}"/> process dynamically.
    /// This class can not be inherited.
    /// </summary>
    public sealed class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public EventDispatcher(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        public async Task RaiseEventAsync<T>(T source) where T : class, IEvent
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            var tasks = _serviceProvider
                .XGetServices<IEventHandler<T>>()
                .Select(handler => handler.HandleAsync(source));

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async Task RaiseEventAsync(IEvent source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            var typeHandler = typeof(IEventHandler<>).MakeGenericType(new Type[] { source.GetType() });

            var tasks = _serviceProvider
                .XGetServices<IEventHandler>(typeHandler)
                .Select(handler => handler.HandleAsync(source));

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}