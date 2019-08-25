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
    /// The implementation of <see cref="ITaskEventRegister"/>.
    /// This class must be used through a decorator and must be registered as follow : SimpleInjector sample
    /// <code>
    ///     Container.Register(PostEventRegister, LifeStyle.Scoped);
    ///     Container.Register(typeof(IPostEventRegister), ()=> Container.GetInstance(PostEventRegister));
    /// </code>
    /// </summary>
    public sealed class TaskEventRegister : ITaskEventRegister
    {
        /// <summary>
        /// The event that will be post raised.
        /// </summary>
        public event Action PostEvent = () => { };

        /// <summary>
        /// The event that will be raised when exception occurred in order to rollback previous actions.
        /// </summary>
        public event Action RollbackEvent = () => { };

        /// <summary>
        /// Raises the <see cref="PostEvent"/> event.
        /// </summary>
        public void OnPostEvent()
        {
            try
            {
                PostEvent();
            }
            finally
            {
                Reset();
            }
        }

        /// <summary>
        /// Raises the <see cref="RollbackEvent"/> event.
        /// </summary>
        public void OnRollbackEvent()
        {
            try
            {
                RollbackEvent();
            }
            finally
            {
                Reset();
            }
        }

        /// <summary>
        /// Clears the event.
        /// </summary>
        private void Reset()
        {
            PostEvent = () => { };
            RollbackEvent = () => { };
        }
    }
}