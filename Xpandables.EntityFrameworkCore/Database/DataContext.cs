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
    public abstract partial class DataContext
    {
        public virtual IQueryable<T> SetOf<T>() where T : Entity => Set<T>();
        Optional<T> IDataContext.Find<T>(params object[] keyValues) => Find<T>(keyValues);
        public virtual IEnumerable<TResult> GetAll<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector)
            where T : Entity
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));
            return from entity in selector(Set<T>())
                   select entity;
        }
        public virtual Optional<TResult> GetFirst<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector)
            where T : Entity
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));
            return selector(Set<T>()).FirstOrEmpty();
        }
        public virtual Optional<TResult> GetLast<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector)
            where T : Entity
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
            if (entities is null || !entities.Any())
                throw new ArgumentNullException(nameof(entities));

            AddRange(entities);
        }
        public virtual void Delete<T>(T entity)
            where T : Entity
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            Remove(entity);
        }
        public virtual void DeleteRange<T>(IEnumerable<T> entities)
            where T : Entity
        {
            if (entities is null || !entities.Any())
                throw new ArgumentNullException(nameof(entities));

            RemoveRange(entities);
        }
        public virtual void Delete<T>(Expression<Func<T, bool>> predicate)
            where T : Entity
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            Set<T>().Where(predicate).ForEach(Remove);
        }
        public virtual void Update<T, TUpdated>(TUpdated updatedEntity)
            where T : Entity
            where TUpdated : Entity
        {
            if (updatedEntity is null) throw new ArgumentNullException(nameof(updatedEntity));
            Set<T>().FirstOrEmpty(entity => entity.Id == updatedEntity.Id)
                   .Map(entity => Entry(entity).CurrentValues.SetValues(updatedEntity));
        }
        public virtual void UpdateRange<T, TUpdated>(IReadOnlyList<TUpdated> updatedEntities)
            where T : Entity
            where TUpdated : Entity
        {
            if (updatedEntities is null || !updatedEntities.Any())
                throw new ArgumentNullException(nameof(updatedEntities));

            foreach (var updatedEntity in updatedEntities)
                Set<T>().FirstOrEmpty(entity => entity.Id == updatedEntity.Id)
                    .Map(entity => Entry(entity).CurrentValues.SetValues(updatedEntity));
        }
        public virtual void Update<T, TUpdated>(Expression<Func<T, bool>> predicate, Func<T, TUpdated> updater)
            where T : Entity
            where TUpdated : class
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (updater is null) throw new ArgumentNullException(nameof(updater));

            foreach (var entity in Set<T>().Where(predicate))
                Entry(entity).CurrentValues.SetValues(updater(entity));
        }
        public void Persist()
        {
            try
            {
                SaveChanges(true);
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
