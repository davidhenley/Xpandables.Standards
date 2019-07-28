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

namespace System.Data
{
    /// <summary>
    /// This class allows us to add asynchronously seed support to data context.
    /// <para>This decorator will always call the <see cref="IAsyncDataContextProvider"/> implementation to seed database.</para>
    /// </summary>
    public sealed class AsyncDataContextSeederDecorator : IAsyncDataContextProvider
    {
        private readonly IAsyncDataContextProvider _decoratee;
        private readonly IAsyncDataContextSeeder _seeder;

        public AsyncDataContextSeederDecorator(IAsyncDataContextProvider decoratee, IAsyncDataContextSeeder seeder)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _seeder = seeder ?? throw new ArgumentNullException(nameof(seeder));
        }

        async Task<IDataContext> IAsyncDataContextProvider.GetDataContextAsync()
        {
            var dataContext = await _decoratee.GetDataContextAsync().ConfigureAwait(false);
            await _seeder.SeedAsync(dataContext).ConfigureAwait(false);
            await dataContext.PersistAsync(CancellationToken.None).ConfigureAwait(false);

            return dataContext;
        }
    }
}