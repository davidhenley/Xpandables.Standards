﻿/************************************************************************************************************
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

namespace System.Configuration
{
    /// <summary>
    /// The implementation of <see cref="ICorrelationTaskRegister"/>.
    /// This class must be used through a decorator and must be registered as follow :
    /// <code>
    ///     services.AddScoped{CorrelationTaskRegister};
    ///     services.AddScoped{ICorrelationTaskRegister}(provider=>provider.GetService{CorrelationTaskRegister}());
    /// </code>
    /// </summary>
    public sealed class CorrelationTaskRegister : ICorrelationTaskRegister
    {
        /// <summary>
        /// The event that will be post raised.
        /// </summary>
        public event Func<ValueTask> PostEvent = async () => await Task.CompletedTask.ConfigureAwait(false);

        /// <summary>
        /// The event that will be raised when exception occurred in order to rollback previous actions.
        /// </summary>
        public event Func<ValueTask> RollbackEvent = async () => await Task.CompletedTask.ConfigureAwait(false);

        /// <summary>
        /// Raises the <see cref="PostEvent"/> event.
        /// </summary>
        public async ValueTask OnPostEventAsync()
        {
            try
            {
                await PostEvent().ConfigureAwait(false);
            }
            finally
            {
                Reset();
            }
        }

        /// <summary>
        /// Raises the <see cref="RollbackEvent"/> event.
        /// </summary>
        public async ValueTask OnRollbackEventAsync()
        {
            try
            {
                await RollbackEvent().ConfigureAwait(false);
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
            PostEvent = async () => await Task.CompletedTask.ConfigureAwait(false);
            RollbackEvent = async () => await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}