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
using System.Threading;
using System.Threading.Tasks;

namespace System.Data
{
    public partial interface IDataContext
    {
        /// <summary>
        /// Finds a domain object matching the primary key values specified and returns its value.
        /// If not found, returns an optional empty type value.
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="keyValues">The primary key values to be found.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="keyValues"/> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous find operation.
        /// The task result contains the entity found, or optional empty.</returns>
        Task<Optional<T>> FindAsync<T>(CancellationToken cancellationToken, params object[] keyValues) where T : Entity;

        /// <summary>
        /// Returns the first domain object matching the expression selector.
        /// If not found, returns an optional empty type value.
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <typeparam name="U">Anonymous result object type.</typeparam>
        /// <param name="selector">Describes the expression used to select the domain object.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous get operation.
        /// The task result contains the matching anonymous value.</returns>
        Task<Optional<U>> GetAsync<T, U>(Func<IQueryable<T>, IQueryable<U>> selector, CancellationToken cancellationToken) where T : Entity;

        /// <summary>
        /// Adds a domain objects to the data storage.
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <param name="toBeAdded">The domain object to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="toBeAdded"/> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        Task AddAsync<T>(T toBeAdded, CancellationToken cancellationToken = default) where T : Entity;

        /// <summary>
        /// Adds a collection of domain objects to the data storage.
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <param name="toBeAddedCollection">The domain objects collection to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="toBeAddedCollection"/> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        Task AddRangeAsync<T>(IEnumerable<T> toBeAddedCollection, CancellationToken cancellationToken = default) where T : Entity;

        /// <summary>
        /// Deletes a domain object from the data storage.
        /// You can use a third party library with <see cref="Set{T}"/> for performance.
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <param name="toBeDeleted">Contains the domain object to be deleted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="toBeDeleted"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        Task DeleteAsync<T>(T toBeDeleted, CancellationToken cancellationToken = default) where T : Entity;

        /// <summary>
        /// Deletes the domain objects matching the collection of entities from the storage using the id.
        /// You can use a third party library with <see cref="Set{T}"/> for performance.
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <param name="toBeDeletedCollection">Contains the domain objects to be deleted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="toBeDeletedCollection"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        Task DeleteRangeAsync<T>(IEnumerable<T> toBeDeletedCollection, CancellationToken cancellationToken = default) where T : Entity;

        /// <summary>
        /// Deletes the domain objects matching the predicate from the storage.
        /// You can use a third party library with <see cref="Set{T}"/> for performance.
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <param name="predicate">The predicate to be used to filter domain objects.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        Task DeleteAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where T : Entity;

        /// <summary>
        /// Updates the domain object matching the id in te updated value.
        /// <para>Only the columns corresponding to properties you set in the object will be updated -- any properties
        /// you don't set will be left alone.</para>
        /// <para>If you have property you want to set to its default,
        /// then you must explicitly set that property's value.</para>
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <typeparam name="U">Type of the object that contains updated values.</typeparam>
        /// <param name="updatedValue">Contains the updated values for the target domain.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="updatedValue"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        Task UpdateAsync<T, U>(U updatedValue, CancellationToken cancellationToken = default) where T : Entity where U : Entity;

        /// <summary>
        /// Updates the domain objects matching the collection of entities.
        /// <para>Only the columns corresponding to properties you set in the object will be updated -- any properties
        /// you don't set will be left alone.</para>
        /// <para>If you have property you want to set to its default,
        /// then you must explicitly set that property's value.</para>
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <typeparam name="U">Type of the object that contains updated values.</typeparam>
        /// <param name="updatedValues">Contains the collection of updated values.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="updatedValues"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        Task UpdateRangeAsync<T, U>(IReadOnlyList<U> updatedValues, CancellationToken cancellationToken = default)
            where T : Entity where U : Entity;

        /// <summary>
        /// Updates the domain objects matching the predicate by using the updater.
        /// <para>Only the columns corresponding to properties you set in the object will be updated -- any properties
        /// you don't set will be left alone.</para>
        /// <para>If you have property you want to set to its default,
        /// then you must explicitly set that property's value.</para>
        /// </summary>
        /// <typeparam name="T">Domain object type.</typeparam>
        /// <typeparam name="U">Type of the object that contains updated values.</typeparam>
        /// <param name="predicate">The predicate to be used to filter domain objects.</param>
        /// <param name="updater">The delegate to be used for updating domain objects.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="updater"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        Task UpdateAsync<T, U>(Expression<Func<T, bool>> predicate, Func<T, U> updater, CancellationToken cancellationToken = default)
            where T : Entity where U : class;

        /// <summary>
        /// Persists all pending domain objects to the data storage.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="InvalidOperationException">All exceptions related to the operation.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        /// <returns>A task that represents the asynchronous persist all operation.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Task PersistAsync(CancellationToken cancellationToken);
    }
}