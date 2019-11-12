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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace System.Design.Database
{
    /// <summary>
    /// Allows an application author to synchronously manage domain objects using EntityFrameworkCore.
    /// It contains all the methods that describe the domain objects manager.
    /// <para>When argument is null, an <see cref="ArgumentNullException"/> will be thrown.</para>
    /// <para>When execution failed, an <see cref="InvalidOperationException"/> will be thrown.</para>
    /// When a value is not found, an optional empty value of the expected type will be returned.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public partial interface IDataContext : IDisposable
    {
        /// <summary>
        /// Provides with a query-able instance for <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of entity.</typeparam>
        /// <returns>An <see cref="IQueryable{T}"/>.</returns>
        IQueryable<T> SetOf<T>() where T : Entity;

        /// <summary>
        /// Finds a domain object matching the primary key values specified and returns its value.
        /// If not found, returns an optional empty type value.
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <param name="keyValues">The primary key values to be found.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="keyValues"/> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        Optional<T> Find<T>(params object[] keyValues) where T : Entity;

        /// <summary>
        /// Returns all domain objects matching the expression selector.
        /// If not found, returns an empty enumerable.
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <typeparam name="TResult">Anonymous result object type.</typeparam>
        /// <param name="selector">Describes the expression used to select the domain object.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        IEnumerable<TResult> GetAll<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector) where T : Entity;

        /// <summary>
        /// Returns the first domain object matching the expression selector.
        /// If not found, returns an optional empty type value.
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <typeparam name="TResult">Anonymous result object type.</typeparam>
        /// <param name="selector">Describes the expression used to select the domain object.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        Optional<TResult> GetFirst<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector) where T : Entity;

        /// <summary>
        /// Returns the last domain object matching the expression selector.
        /// If not found, returns an optional empty type value.
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <typeparam name="TResult">Anonymous result object type.</typeparam>
        /// <param name="selector">Describes the expression used to select the domain object.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        Optional<TResult> GetLast<T, TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector) where T : Entity;

        /// <summary>
        /// Adds a domain objects to the data storage.
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <param name="entity">The domain object to be added and persisted.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        void Add<T>(T entity) where T : Entity;

        /// <summary>
        /// Adds a collection of domain objects to the data storage.
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <param name="entities">The domain objects collection to be added and persisted.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entities"/> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        void AddRange<T>(IEnumerable<T> entities) where T : Entity;

        /// <summary>
        /// Deletes a domain object from the data storage.
        /// You can use a third party library with <see cref="SetOf{T}"/> for performance.
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <param name="entity">Contains the domain object to be deleted.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        void Delete<T>(T entity) where T : Entity;

        /// <summary>
        /// Deletes the domain objects matching the collection of entities from the storage using the id.
        /// You can use a third party library with <see cref="SetOf{T}"/> for performance.
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <param name="entities">Contains the domain objects to be deleted.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entities"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        void DeleteRange<T>(IEnumerable<T> entities) where T : Entity;

        /// <summary>
        /// Deletes the domain objects matching the predicate from the storage.
        /// You can use a third party library with <see cref="SetOf{T}"/> for performance.
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <param name="predicate">The predicate to be used to filter domain objects.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        void Delete<T>(Expression<Func<T, bool>> predicate) where T : Entity;

        /// <summary>
        /// Updates the domain object matching the id in te updated value.
        /// <para>Only the columns corresponding to properties you set in the object will be updated -- any properties
        /// you don't set will be left alone.</para>
        /// <para>If you have property you want to set to its default,
        /// then you must explicitly set that property's value.</para>
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <typeparam name="TUpdated">Type of the object that contains updated values.</typeparam>
        /// <param name="updatedEntity">Contains the updated values for the target domain.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="updatedEntity"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        void Update<T, TUpdated>(TUpdated updatedEntity) where T : Entity where TUpdated : Entity;

        /// <summary>
        /// Updates the domain objects matching the collection of entities.
        /// <para>Only the columns corresponding to properties you set in the object will be updated -- any properties
        /// you don't set will be left alone.</para>
        /// <para>If you have property you want to set to its default,
        /// then you must explicitly set that property's value.</para>
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <typeparam name="TUpdated">Type of the object that contains updated values.</typeparam>
        /// <param name="updatedEntities">Contains the collection of updated values.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="updatedEntities"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        void UpdateRange<T, TUpdated>(IReadOnlyList<TUpdated> updatedEntities) where T : Entity where TUpdated : Entity;

        /// <summary>
        /// Updates the domain objects matching the predicate by using the updater.
        /// <para>Only the columns corresponding to properties you set in the object will be updated -- any properties
        /// you don't set will be left alone.</para>
        /// <para>If you have property you want to set to its default,
        /// then you must explicitly set that property's value.</para>
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <typeparam name="TUpdated">Type of the object that contains updated values.</typeparam>
        /// <param name="predicate">The predicate to be used to filter domain objects.</param>
        /// <param name="updater">The delegate to be used for updating domain objects.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="updater"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        void Update<T, TUpdated>(Expression<Func<T, bool>> predicate, Func<T, TUpdated> updater) where T : Entity where TUpdated : class;

        /// <summary>
        /// Persists all pending domain objects to the data storage.
        /// </summary>
        /// <exception cref="InvalidOperationException">All exceptions related to the operation.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void Persist();
    }
}