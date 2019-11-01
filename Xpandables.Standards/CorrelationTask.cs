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
    /// The implementation of <see cref="ICorrelationTask"/>.
    /// This class must be used through a behavior and must be registered as follow :
    /// <code>
    ///     services.AddScoped{CorrelationTask};
    ///     services.AddScoped{ICorrelationTask}(provider=>provider.GetService{CorrelationTask}());
    /// </code>
    /// </summary>
    public sealed class CorrelationTask : ICorrelationTask
    {
        /// <summary>
        /// The event that will be executed after the main one in the same control flow only if there is no exception.
        /// </summary>
        public event Func<ValueTask> PostEvent = async () => await Task.CompletedTask.ConfigureAwait(false);

        /// <summary>
        /// The event that will be executed after the main one when exception. The event will received the control flow handled exception.
        /// </summary>
        public event Func<Exception, ValueTask> RollbackEvent = async (_) => await Task.CompletedTask.ConfigureAwait(false);

        /// <summary>
        /// Raises the <see cref="PostEvent"/> event.
        /// </summary>
        internal async ValueTask OnPostEventAsync()
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
        /// <param name="exception">The control flow handled exception.</param>
        internal async ValueTask OnRollbackEventAsync(Exception exception)
        {
            try
            {
                await RollbackEvent(exception).ConfigureAwait(false);
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
            RollbackEvent = async (_) => await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}