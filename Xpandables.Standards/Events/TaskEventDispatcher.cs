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

namespace System.Design.TaskEvent
{
    /// <summary>
    /// Implementing the <see cref=" ITaskEventDispatcher"/> interface.
    /// </summary>
    public sealed class TaskEventDispatcher : ITaskEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes the dispatcher with a service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider to be used.</param>
        public TaskEventDispatcher(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        void ITaskEventDispatcher.Dispatch<TEvent>(TEvent taskEvent)
        {
            if (taskEvent is null) throw new ArgumentNullException(nameof(taskEvent));

            _serviceProvider.GetServices<ITaskEventHandler<TEvent>>()
                .ForEach((ITaskEventHandler<TEvent> handler) => handler.Handle(taskEvent));
        }

        void ITaskEventDispatcher.Dispatch(ITaskEvent taskEvent)
        {
            if (taskEvent is null) throw new ArgumentNullException(nameof(taskEvent));

            var typeHandler = typeof(ITaskEventHandler<>).MakeGenericType(new Type[] { taskEvent.GetType() });
            _serviceProvider.GetServices<ITaskEventHandler>(typeHandler)
                .ForEach((ITaskEventHandler handler) => handler.Handle(taskEvent));
        }
    }
}