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
    /// Defines an event that will be raised after another one or exception and executed in the asynchronous control flows.
    /// </summary>
    public interface IAsyncEventRegister
    {
        /// <summary>
        /// The event that will be post raised.
        /// </summary>
        event Func<Task> PostEvent;

        /// <summary>
        /// The event that will be raised on exception.
        /// </summary>
        event Func<Task> RollbackEvent;
    }
}