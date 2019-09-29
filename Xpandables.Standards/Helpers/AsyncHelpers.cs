
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

namespace System
{
    /// <summary>
    /// Provides with async helpers.
    /// </summary>
    public static class AsyncHelpers
    {
        private static readonly TaskFactory _taskFactory = new TaskFactory(
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            TaskScheduler.Default);

        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Reliability", "CA2008:Ne pas créer de tâches sans passer TaskScheduler", Justification = "<En attente>")]
        public static TResult RunSync<TResult>(this Func<Task<TResult>> func)
            => _taskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();

        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Reliability", "CA2008:Ne pas créer de tâches sans passer TaskScheduler", Justification = "<En attente>")]
        public static void RunSync(this Func<Task> func)
            => _taskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
    }
}
