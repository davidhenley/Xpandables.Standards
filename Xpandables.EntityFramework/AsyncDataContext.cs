/************************************************************************************************************
 * Copyright (C) 2018 Francis-Black EWANE
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

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data
{
    public abstract partial class DataContext
    {
        async Task<Optional<T>> IDataContext.FindAsync<T>(CancellationToken cancellationToken, params object[] keyValues)
            => await FindAsync<T>(keyValues, cancellationToken).ConfigureAwait(false);

        async Task<Optional<U>> IDataContext.GetAsync<T, U>(Func<IQueryable<T>, IQueryable<U>> selector, CancellationToken cancellationToken)
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));
            return await selector(Set<T>()).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        async Task IDataContext.AddAsync<T>(T toBeAdded, CancellationToken cancellationToken)
        {
            if (toBeAdded is null) throw new ArgumentNullException(nameof(toBeAdded));
            await AddAsync(toBeAdded, cancellationToken).ConfigureAwait(false);
        }

        async Task IDataContext.AddRangeAsync<T>(IEnumerable<T> toBeAddedCollection, CancellationToken cancellationToken)
        {
            if (toBeAddedCollection is null) throw new ArgumentNullException(nameof(toBeAddedCollection));
            await AddRangeAsync(toBeAddedCollection, cancellationToken).ConfigureAwait(false);
        }

        async Task IDataContext.DeleteAsync<T>(T toBeDeleted, CancellationToken cancellationToken)
        {
            if (toBeDeleted is null) throw new ArgumentNullException(nameof(toBeDeleted));
            Remove(toBeDeleted);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task DeleteAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
            where T : Entity
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            await Set<T>().Where(predicate).ForEachAsync(entity => Remove(entity), cancellationToken)
                .ConfigureAwait(false);
        }

        async Task IDataContext.DeleteRangeAsync<T>(IEnumerable<T> toBeDeletedCollection, CancellationToken cancellationToken)
        {
            if (toBeDeletedCollection is null) throw new ArgumentNullException(nameof(toBeDeletedCollection));
            RemoveRange(toBeDeletedCollection);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task UpdateAsync<T, U>(U updatedValue, CancellationToken cancellationToken)
            where T : Entity where U : Entity
        {
            if (updatedValue is null) throw new ArgumentNullException(nameof(updatedValue));
            await Set<T>().Where(entity => entity.Id == updatedValue.Id)
                    .ForEachAsync(entity => Entry(entity).CurrentValues.SetValues(updatedValue), cancellationToken)
                    .ConfigureAwait(false);
        }

        public async Task UpdateRangeAsync<T, U>(IReadOnlyList<U> updatedValues, CancellationToken cancellationToken)
            where T : Entity where U : Entity
        {
            if (updatedValues is null) throw new ArgumentNullException(nameof(updatedValues));
            foreach (var updatedValue in updatedValues)
                await Set<T>().Where(entity => entity.Id == updatedValue.Id)
                    .ForEachAsync(entity => Entry(entity).CurrentValues.SetValues(updatedValue), cancellationToken)
                    .ConfigureAwait(false);
        }

        public async Task UpdateAsync<T, U>(Expression<Func<T, bool>> predicate, Func<T, U> updater, CancellationToken cancellationToken)
            where T : Entity where U : class
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (updater is null) throw new ArgumentNullException(nameof(updater));

            await Set<T>().Where(predicate)
                    .ForEachAsync(entity => Entry(entity).CurrentValues.SetValues(updater(entity)), cancellationToken)
                    .ConfigureAwait(false);
        }

        async Task IDataContext.PersistAsync(CancellationToken cancellationToken)
        {
            try
            {
                await SaveChangesAsync(true, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is DbUpdateException)
            {
                throw new InvalidOperationException("Persistence operation failed. See inner exception", exception);
            }
        }
    }
}