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
    /// Allows an application author to define a handler for event.
    /// </summary>
    public interface ITaskEventHandler
    {
        /// <summary>
        /// Defines the method that should be used when an event is handled.
        /// </summary>
        /// <param name="taskEvent">The event instance to be handled.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="taskEvent"/> can not be null.</exception>
        void Handle(object taskEvent);
    }

    /// <summary>
    /// Allows an application author to define a handler for specific type event.
    /// </summary>
    /// <typeparam name="TTaskEvent">The event type to be handled.</typeparam>
    public interface ITaskEventHandler<in TTaskEvent> : ITaskEventHandler
        where TTaskEvent : class, ITaskEvent
    {
        /// <summary>
        /// Defines the method that should be used when a specific type event is handled.
        /// </summary>
        /// <param name="taskEvent">The event instance to be handled.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="taskEvent"/> can not be null.</exception>
        void Handle(TTaskEvent taskEvent);
    }
}