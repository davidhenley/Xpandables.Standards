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
    /// This class is a helper that provides a default implementation for most of
    /// the methods of <see cref="IAsyncTaskEventHandler{TEvent}"/>.
    /// This is an <see langword="abstract"/> class.
    /// </summary>
    public abstract class AsyncTaskEventHandler<TTaskEvent> : IAsyncTaskEventHandler<TTaskEvent>
        where TTaskEvent : class, ITaskEvent
    {
        /// <summary>
        /// When overridden, this method will be used when a specific type event must be handled asynchronously.
        /// </summary>
        /// <param name="taskEvent">The event instance that may be used in handling the notification.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="taskEvent"/> can not be null.</exception>
        /// <exception cref="OperationCanceledException">The operation has been cancelled.</exception>
        public abstract Task HandleAsync(TTaskEvent taskEvent, CancellationToken cancellationToken);

        async Task IAsyncTaskEventHandler.HandleAsync(object taskEvent, CancellationToken cancellationToken)
            => await HandleAsync((TTaskEvent)taskEvent, cancellationToken).ConfigureAwait(false);
    }
}