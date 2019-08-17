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

namespace System.Design.TaskEvent
{
    /// <summary>
    /// The base interface that allows an application author to implement dispatching.
    /// </summary>
    public interface IAsyncTaskEventDispatcher
    {
        /// <summary>
        /// Resolves all types that matches the <see cref="ITaskEventHandler{TEvent}"/> and calls their handlers asynchronously.
        /// The operation will wait for all handlers to be completed.
        /// </summary>
        /// <param name="taskEvent">The event to be dispatched</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="taskEvent"/> can not be null</exception>
        /// <exception cref="OperationCanceledException">The operation has been cancelled.</exception>
        Task DispatchAsync(ITaskEvent taskEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves all types that matches the <see cref="IAsyncTaskEventHandler{TEvent}"/>
        /// and calls their handlers asynchronously.
        /// The operation will wait for all handlers to be completed.
        /// </summary>
        /// <typeparam name="TTaskEvent">The event type</typeparam>
        /// <param name="taskEvent">The event to be dispatched</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="taskEvent"/> can not be null</exception>
        /// <exception cref="OperationCanceledException">The operation has been cancelled.</exception>
        Task DispatchAsync<TTaskEvent>(TTaskEvent taskEvent, CancellationToken cancellationToken = default)
            where TTaskEvent : class, ITaskEvent;
    }
}