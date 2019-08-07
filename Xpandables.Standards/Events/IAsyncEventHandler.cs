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
    /// Allows an application author to define an event to be handled.
    /// </summary>
    /// <remarks>
    /// Any operation that does not deliver or do what it promises to do should throw an exception.
    /// </remarks>
    public interface IAsyncEventHandler
    {
        /// <summary>
        /// Defines the method that should be used when an event is handled.
        /// </summary>
        /// <param name="event">The notification instance that may be used in handling the notification.</param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> can not be null.</exception>
        /// <exception cref="InvalidOperationException">The event can not be handled. See inner exception.</exception>
        Task HandleAsync(object @event, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Allows an application author to define a specific type <typeparamref name="TEvent"/> to be handled.
    /// </summary>
    /// <typeparam name="TEvent">The event type to be handled.</typeparam>
    /// <remarks>
    /// Any operation that does not deliver or do what it promises to do should throw an exception.
    /// </remarks>
    public interface IAsyncEventHandler<in TEvent> : IAsyncEventHandler
        where TEvent : class, IEvent
    {
        /// <summary>
        /// Defines the method that should be used when a specific type event is handled.
        /// </summary>
        /// <param name="event">The event instance that may be used in handling the notification.</param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> can not be null.</exception>
        /// <exception cref="InvalidOperationException">The event can not be handled. See inner exception.</exception>
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
    }
}