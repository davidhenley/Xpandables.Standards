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
    /// Defines two tasks that can be used to follow process after a control flow with <see cref="PostEvent"/> and on exception during the control flow
    /// with <see cref="RollbackEvent"/>. In order to be activated, the target class should implement the <see cref="ICorrelationBehavior"/> interface,
    /// the target handling class should reference the current interface (to set the action) and you should register the behavior with the expected extension 
    /// method <see langword="AddXCorrelationBehavior"/>.
    /// </summary>
    public interface ICorrelationTask
    {
        /// <summary>
        /// The event that will be executed after the main one in the same control flow only if there is no exception.
        /// </summary>
        event Func<ValueTask> PostEvent;

        /// <summary>
        /// The event that will be executed after the main one when exception. The event will received the control flow handled exception.
        /// </summary>
        event Func<Exception, ValueTask> RollbackEvent;
    }
}