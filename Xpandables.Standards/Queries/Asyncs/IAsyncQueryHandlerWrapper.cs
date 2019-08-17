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

namespace System.Design.Query
{
    /// <summary>
    /// This interface allows application authors to avoid use of C# dynamics with query pattern.
    /// </summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <remarks>
    /// From https://gist.github.com/dotnetjunkie/d9bdb09534a75635ca552755faaa1cd5
    /// </remarks>
    public interface IAsyncQueryHandlerWrapper<TResult>
    {
        /// <summary>
        /// Handles the specified query and returns the expected result type.
        /// </summary>
        /// <param name="criteria">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been cancelled.</exception>
        Task<TResult> HandleAsync(IQuery<TResult> criteria, CancellationToken cancellationToken = default);
    }
}