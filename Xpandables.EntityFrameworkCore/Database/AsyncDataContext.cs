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

        [Diagnostics.CodeAnalysis.SuppressMessage("Naming", "RCS1047:Non-asynchronous method name should not end with 'Async'.",
            Justification = "<En attente>")]
        public virtual IAsyncEnumerable<TResult> GetAllAsync<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector)
            where T : Entity
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));
            return selector(Set<T>()).AsAsyncEnumerable();
        }

        public virtual async Task<Optional<TResult>> GetFirstAsync<T, TResult>(
           Func<IQueryable<T>, IQueryable<TResult>> selector,
           CancellationToken cancellationToken)
            where T : Entity
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));
            return await selector(Set<T>()).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<Optional<TResult>> GetLastAsync<T, TResult>(
          Func<IQueryable<T>, IQueryable<TResult>> selector,
          CancellationToken cancellationToken)
            where T : Entity
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));
            return await selector(Set<T>()).LastOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        Task IDataContext.AddAsync<T>(T entity, CancellationToken cancellationToken)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            return AddAsync(entity, cancellationToken).AsTask();
        }

        Task IDataContext.AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
            if (entities?.Any() != true)
                throw new ArgumentNullException(nameof(entities));

            return AddRangeAsync(entities, cancellationToken);
        }

        public virtual Task DeleteAsync<T>(T entity, CancellationToken cancellationToken)
            where T : Entity
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            Remove(entity);
            return Task.CompletedTask;
        }

        public virtual Task DeleteRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken)
            where T : Entity
        {
            if (entities?.Any() != true)
                throw new ArgumentNullException(nameof(entities));

            RemoveRange(entities);
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
            where T : Entity
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            return Set<T>()
                .Where(predicate)
                .ForEachAsync(entity => Remove(entity), cancellationToken);
        }

        public virtual Task UpdateAsync<T, TUpdated>(TUpdated updatedEntity, CancellationToken cancellationToken)
            where T : Entity
            where TUpdated : Entity
        {
            if (updatedEntity is null) throw new ArgumentNullException(nameof(updatedEntity));
            return Set<T>().Where(entity => entity.Id == updatedEntity.Id)
                 .ForEachAsync(entity => Entry(entity).CurrentValues.SetValues(updatedEntity), cancellationToken);
        }

        public virtual async Task UpdateRangeAsync<T, TUpdated>(
           IReadOnlyList<TUpdated> updatedEntities,
           CancellationToken cancellationToken)
            where T : Entity
            where TUpdated : Entity
        {
            if (updatedEntities?.Any() != true)
                throw new ArgumentNullException(nameof(updatedEntities));

            foreach (var updatedEntity in updatedEntities)
            {
                await Set<T>().Where(entity => entity.Id == updatedEntity.Id)
                    .ForEachAsync(entity => Entry(entity).CurrentValues.SetValues(updatedEntity), cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        public virtual Task UpdateAsync<T, TUpdated>(
          Expression<Func<T, bool>> predicate,
          Func<T, TUpdated> updater,
          CancellationToken cancellationToken)
            where T : Entity
            where TUpdated : class
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (updater is null) throw new ArgumentNullException(nameof(updater));

            return Set<T>().Where(predicate)
                 .ForEachAsync(entity => Entry(entity).CurrentValues.SetValues(updater(entity)), cancellationToken);
        }

        public async Task PersistAsync(CancellationToken cancellationToken)
        {
            try
            {
                await SaveChangesAsync(true, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is DbUpdateException)
            {
                throw new InvalidOperationException(
                    ErrorMessageResources.DataContextPersistenceException,
                    exception);
            }
        }
    }
}
