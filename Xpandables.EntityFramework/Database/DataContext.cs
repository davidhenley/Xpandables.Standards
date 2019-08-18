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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace System.Design.Database
{
    /// <summary>
    /// This is the <see langword="abstract"/> db context class that inherits from <see cref="DbContext"/>
    /// and implements <see cref="IDataContext"/>.
    /// </summary>
    public abstract partial class DataContext : DbContext, IDataContext
    {
        IQueryable<T> IDataContext.SetOf<T>() => Set<T>();
        Optional<T> IDataContext.Find<T>(params object[] keyValues) => Find<T>(keyValues);
        IEnumerable<TResult> IDataContext.GetAll<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector)
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));
            return from entity in selector(Set<T>())
                   select entity;
        }
        Optional<TResult> IDataContext.GetFirst<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector)
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));
            return selector(Set<T>()).FirstOrEmpty();
        }
        Optional<TResult> IDataContext.GetLast<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector)
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));
            return selector(Set<T>()).LastOrEmpty();
        }
        void IDataContext.Add<T>(T entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            Add(entity);
        }
        void IDataContext.AddRange<T>(IEnumerable<T> entities)
        {
            if (entities is null || entities.Count() <= 0)
                throw new ArgumentNullException(nameof(entities));

            AddRange(entities);
        }
        void IDataContext.Delete<T>(T entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            Remove(entity);
        }
        void IDataContext.DeleteRange<T>(IEnumerable<T> entities)
        {
            if (entities is null || entities.Count() <= 0)
                throw new ArgumentNullException(nameof(entities));

            RemoveRange(entities);
        }
        void IDataContext.Delete<T>(Expression<Func<T, bool>> predicate)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            Set<T>().Where(predicate).ForEach(Remove);
        }
        void IDataContext.Update<T, TUpdated>(TUpdated updatedEntity)
        {
            if (updatedEntity is null) throw new ArgumentNullException(nameof(updatedEntity));
            Set<T>().FirstOrEmpty(entity => entity.Id == updatedEntity.Id)
               .Map(entity => Entry(entity).CurrentValues.SetValues(updatedEntity));
        }
        void IDataContext.UpdateRange<T, TUpdated>(IReadOnlyList<TUpdated> updatedEntities)
        {
            if (updatedEntities is null || updatedEntities.Count() <= 0)
                throw new ArgumentNullException(nameof(updatedEntities));

            foreach (var updatedEntity in updatedEntities)
                Set<T>().FirstOrEmpty(entity => entity.Id == updatedEntity.Id)
                    .Map(entity => Entry(entity).CurrentValues.SetValues(updatedEntity));
        }
        void IDataContext.Update<T, TUpdated>(Expression<Func<T, bool>> predicate, Func<T, TUpdated> updater)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (updater is null) throw new ArgumentNullException(nameof(updater));

            foreach (var entity in Set<T>().Where(predicate))
                Entry(entity).CurrentValues.SetValues(updater(entity));
        }
        void IDataContext.Persist()
        {
            try
            {
                SaveChanges(true);
            }
            catch (Exception exception) when (exception is DbUpdateException)
            {
                throw new InvalidOperationException("Persistence operation failed. See inner exception", exception);
            }
        }
    }
}
