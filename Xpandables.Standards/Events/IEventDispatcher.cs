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
    /// The base interface that allows an application author to implement dispatching.
    /// </summary>
    /// <remarks>
    /// Any operation that does not deliver or do what it promises to do should throw an exception.
    /// </remarks>
    public interface IEventDispatcher
    {
        /// <summary>
        /// Resolves all types that matches the <see cref="IEventHandler{TEvent}"/> and calls their handlers asynchronously.
        /// The operation will wait for all handlers to be completed.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <param name="event">The event to be dispatched</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> can not be null</exception>
        Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : class, IEvent;

        /// <summary>
        /// Resolves all types that matches the <see cref="IEventHandler{TEvent}"/> where TEvent is <paramref name="event"/> type
        /// and calls their handlers asynchronously.
        /// The operation will wait for all handlers to be completed.
        /// </summary>
        /// <param name="event">The event to be dispatched</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> can not be null</exception>
        Task DispatchAsync(IEvent @event, CancellationToken cancellationToken);
    }
}