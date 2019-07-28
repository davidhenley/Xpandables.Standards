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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    /// Defines a set of items for a requested page.
    /// Implements <see cref="IEnumerable{T}"/> and <see cref="IAddable{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type</typeparam>
    [Serializable]
    public sealed class Paged<TItem> : IEnumerable<TItem>, IAddable<TItem>
    {
        /// <summary>
        /// Returns a new instance of <see cref="Paged{TItem}"/> with then specified arguments.
        /// </summary>
        /// <param name="paging">Informations about paging.</param>
        /// <param name="items">The collections of items.</param>
        /// <param name="pagesCount">The number of pages found.</param>
        /// <param name="itemsCount">The number of element in the global collection.</param>
        /// <param name="nextPageUrl">The next page URL.</param>
        public Paged(Paging paging, IEnumerable<TItem> items, int pagesCount, int itemsCount, string nextPageUrl = default)
        {
            Paging = paging;
            Items = items;
            ItemsCount = itemsCount;
            PagesCount = pagesCount;
            NextPageUrl = nextPageUrl;
        }

        /// <summary>
        /// Contains information about the requested page.
        /// </summary>
        public Paging Paging { get; }

        /// <summary>
        /// Contains the number of items found.
        /// </summary>
        public int ItemsCount { get; }

        /// <summary>
        /// Contains the number of pages according to paging.
        /// </summary>
        public int PagesCount { get; }

        /// <summary>
        /// The URL to the next page. If null, there are no more pages.
        /// </summary>
        public string NextPageUrl { get; }

        /// <summary>
        /// Contains the list of items of specified type for the given page.
        /// </summary>
        public IEnumerable<TItem> Items { get; private set; } = Enumerable.Empty<TItem>();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<TItem> GetEnumerator()
            => Items?.GetEnumerator();

        /// <summary>
        ///  Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        /// <summary>
        /// For serialization use.
        /// </summary>
        /// <param name="item"></param>
        public void Add(TItem item) => Items = Items.Concat(new[] { item }).ToList();
    }
}