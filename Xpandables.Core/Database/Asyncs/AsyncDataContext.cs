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

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Design.Database
{
    public abstract partial class DataContext
    {
        async Task<Optional<T>> IDataContext.FindAsync<T>(CancellationToken cancellationToken, params object[] keyValues)
             => await FindAsync<T>(keyValues, cancellationToken).ConfigureAwait(false);
        IAsyncEnumerable<TResult> IDataContext.GetAllAsync<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector)
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));
            return selector(Set<T>()).AsAsyncEnumerable();
        }
        async Task<Optional<TResult>> IDataContext.GetFirstAsync<T, TResult>(
           Func<IQueryable<T>, IQueryable<TResult>> selector,
           CancellationToken cancellationToken)
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));
            return await selector(Set<T>()).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }
        async Task<Optional<TResult>> IDataContext.GetLastAsync<T, TResult>(
          Func<IQueryable<T>, IQueryable<TResult>> selector,
          CancellationToken cancellationToken)
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));
            return await selector(Set<T>()).LastOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }
        async Task IDataContext.AddAsync<T>(T entity, CancellationToken cancellationToken)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            await AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }
        async Task IDataContext.AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
            if (entities is null || entities.Count() <= 0)
                throw new ArgumentNullException(nameof(entities));

            await AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
        }
        async Task IDataContext.DeleteAsync<T>(T entity, CancellationToken cancellationToken)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            Remove(entity);
            await Task.CompletedTask.ConfigureAwait(false);
        }
        async Task IDataContext.DeleteRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
            if (entities is null || entities.Count() <= 0)
                throw new ArgumentNullException(nameof(entities));

            RemoveRange(entities);
            await Task.CompletedTask.ConfigureAwait(false);
        }
        async Task IDataContext.DeleteAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            await Set<T>()
                .Where(predicate)
                .ForEachAsync(entity => Remove(entity), cancellationToken)
                .ConfigureAwait(false);
        }
        async Task IDataContext.UpdateAsync<T, TUpdated>(TUpdated updatedEntity, CancellationToken cancellationToken)
        {
            if (updatedEntity is null) throw new ArgumentNullException(nameof(updatedEntity));
            await Set<T>().Where(entity => entity.Id == updatedEntity.Id)
                 .ForEachAsync(entity => Entry(entity).CurrentValues.SetValues(updatedEntity), cancellationToken)
                 .ConfigureAwait(false);
        }
        async Task IDataContext.UpdateRangeAsync<T, TUpdated>(
           IReadOnlyList<TUpdated> updatedEntities,
           CancellationToken cancellationToken)
        {
            if (updatedEntities is null || updatedEntities.Count() <= 0)
                throw new ArgumentNullException(nameof(updatedEntities));

            foreach (var updatedEntity in updatedEntities)
                await Set<T>().Where(entity => entity.Id == updatedEntity.Id)
                    .ForEachAsync(entity => Entry(entity).CurrentValues.SetValues(updatedEntity), cancellationToken)
                    .ConfigureAwait(false);
        }
        async Task IDataContext.UpdateAsync<T, TUpdated>(
          Expression<Func<T, bool>> predicate,
          Func<T, TUpdated> updater,
          CancellationToken cancellationToken)
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
