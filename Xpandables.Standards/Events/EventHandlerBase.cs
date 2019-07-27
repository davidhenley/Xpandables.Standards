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

using System.Threading;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// This class is a helper that provides a default implementation for most of the methods of <see cref="IEventHandler{TEvent}"/>.
    /// This is an <see langword="abstract"/> class.
    /// </summary>
    public abstract class EventHandlerBase<TEvent> : IEventHandler<TEvent>
        where TEvent : class, IEvent
    {
        /// <summary>
        /// When overridden, this method will be used when a specific type event is handled asynchronously.
        /// </summary>
        /// <param name="event">The event instance that may be used in handling the notification.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> can not be null.</exception>
        /// <exception cref="InvalidOperationException">The event can not be handled. See inner exception.</exception>
        public abstract Task HandleAsync(TEvent @event, CancellationToken cancellationToken);

        async Task IEventHandler.HandleAsync(object notification, CancellationToken cancellationToken)
            => await HandleAsync((TEvent)notification, cancellationToken).ConfigureAwait(false);
    }
}