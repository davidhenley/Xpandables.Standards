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
    /// This class is a helper that provides a default implementation
    /// for most of the methods of <see cref="ITaskEventHandler{TTaskEvent}"/>.
    /// This is an <see langword="abstract"/> class.
    /// </summary>
    public abstract class TaskEventHandler<TTaskEvent> : ITaskEventHandler<TTaskEvent>
        where TTaskEvent : class, ITaskEvent
    {
        /// <summary>
        /// When overridden, this method will be used when a specific type event is handled.
        /// </summary>
        /// <param name="taskEvent">The event to act on..</param>
        /// <exception cref="ArgumentNullException">The <paramref name="taskEvent"/> can not be null.</exception>
        public abstract void Handle(TTaskEvent taskEvent);

        void ITaskEventHandler.Handle(object taskEvent) => Handle((TTaskEvent)taskEvent);
    }
}