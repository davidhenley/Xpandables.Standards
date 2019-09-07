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

using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Allows an application author to define a handler for specific type event.
    /// </summary>
    public interface IEventHandler : IFluent
    {
        /// <summary>
        /// Defines the method that should be used when a specific type event is handled.
        /// </summary>
        /// <param name="source">The event instance to be handled.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> can not be null.</exception>
        Task HandleAsync(object source);
    }

    /// <summary>
    /// Allows an application author to define a handler for specific type event.
    /// </summary>
    /// <typeparam name="T">The event type to be handled.</typeparam>
    public interface IEventHandler<in T> : IEventHandler
        where T : class, IEvent
    {
        /// <summary>
        /// Defines the method that should be used when a specific type event is handled.
        /// </summary>
        /// <param name="source">The event instance to be handled.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> can not be null.</exception>
        Task HandleAsync(T source);
    }
}