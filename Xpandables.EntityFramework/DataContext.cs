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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data
{
    /// <summary>
    /// This is the <see langword="abstract"/> db context class that inherits from <see cref="DbContext"/> and implements <see cref="IDataContext"/>.
    /// </summary>
    public abstract partial class DataContext : DbContext, IDataContext
    {
        private static readonly MethodInfo convertToStringMethodInfo =
            typeof(DataContext).GetMethod(nameof(ConvertEnumerationToString), BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo convertToEnumerationMethodInfo =
            typeof(DataContext).GetMethod(nameof(ConvertStringToEnumeration), BindingFlags.NonPublic | BindingFlags.Static);
        private static string ConvertEnumerationToString<T>(T enumeration) where T : Enumeration => enumeration.DisplayName;
        private static T ConvertStringToEnumeration<T>(string displayName) where T : Enumeration => Enumeration.FromDisplayName<T>(displayName);

        private static Expression<Func<T, U>> ConverterMethodToLambdaExpression<T, U>(MethodInfo methodInfo, string argumentName)
        {
            methodInfo = methodInfo.MakeGenericMethod(new Type[] { typeof(T) == typeof(string) ? typeof(U) : typeof(T) });
            var param = Expression.Parameter(typeof(T), argumentName);
            var methodCall = Expression.Call(null, methodInfo, new Expression[] { param });
            return Expression.Lambda<Func<T, U>>(methodCall, new ParameterExpression[] { param });
        }

        /// <summary>
        /// Returns a generic <see cref="ValueConverter{TModel, TProvider}"/> for <see cref="Enumeration"/>.
        /// To be used with
        /// <see cref="Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder{TProperty}.HasConversion(ValueConverter)"/>.
        /// </summary>
        /// <typeparam name="T">Type of enumeration.</typeparam>
        /// <returns>A value converter matching for <typeparamref name="T"/> type.</returns>
        protected static ValueConverter<T, string> EnumerationConverter<T>()
            where T : Enumeration
        {
            var convertToStringLamda = ConverterMethodToLambdaExpression<T, string>(convertToStringMethodInfo, "enumeration");
            var convertToEnumerationLamda = ConverterMethodToLambdaExpression<string, T>(convertToEnumerationMethodInfo, "displayName");

            return new ValueConverter<T, string>(convertToStringLamda, convertToEnumerationLamda);
            //return new ValueConverter<T, string>(v => v.DisplayName, v => Enumeration.FromDisplayName<T>(v));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContext"/> class
        /// using the specified options. The <see langword="DataContext.OnConfiguring(DbContextOptionsBuilder)"/>
        /// method will still be called to allow further configuration of the options.
        /// </summary>
        /// <param name="contextOptions">The options for this context.</param>
        protected DataContext(DbContextOptions contextOptions)
            : base(contextOptions) { }

        IQueryable<T> IDataContext.Set<T>() => Set<T>();

        Optional<T> IDataContext.Find<T>(params object[] keyValues) => Find<T>(keyValues);

        IEnumerable<U> IDataContext.GetAll<T, U>(Func<IQueryable<T>, IQueryable<U>> selector)
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));
            return from entity in selector(Set<T>())
                   select entity;
        }

        Optional<U> IDataContext.Get<T, U>(Func<IQueryable<T>, IQueryable<U>> selector)
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));
            return selector(Set<T>()).FirstOrEmpty();
        }

        void IDataContext.Add<T>(T toBeAdded)
        {
            if (toBeAdded is null) throw new ArgumentNullException(nameof(toBeAdded));
            Add(toBeAdded);
        }

        void IDataContext.AddRange<T>(IEnumerable<T> toBeAddedCollection)
        {
            if (toBeAddedCollection is null) throw new ArgumentNullException(nameof(toBeAddedCollection));
            AddRange(toBeAddedCollection);
        }

        void IDataContext.Delete<T>(T toBeDeleted)
        {
            if (toBeDeleted is null) throw new ArgumentNullException(nameof(toBeDeleted));
            Remove(toBeDeleted);
        }

        void IDataContext.DeleteRange<T>(IEnumerable<T> toBeDeletedCollection)
        {
            if (toBeDeletedCollection is null) throw new ArgumentNullException(nameof(toBeDeletedCollection));
            RemoveRange(toBeDeletedCollection);
        }

        void IDataContext.Delete<T>(Expression<Func<T, bool>> predicate)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            Set<T>().Where(predicate).ForEach(Remove);
        }

        void IDataContext.Update<T, U>(U updatedValue)
        {
            if (updatedValue is null) throw new ArgumentNullException(nameof(updatedValue));
            Set<T>().FirstOrEmpty(entity => entity.Id == updatedValue.Id)
                .Map(entity => Entry(entity).CurrentValues.SetValues(updatedValue));
        }

        void IDataContext.UpdateRange<T, U>(IReadOnlyList<U> updatedValues)
        {
            if (updatedValues is null) throw new ArgumentNullException(nameof(updatedValues));
            foreach (var updatedValue in updatedValues)
                Set<T>().FirstOrEmpty(entity => entity.Id == updatedValue.Id)
                    .Map(entity => Entry(entity).CurrentValues.SetValues(updatedValue));
        }

        void IDataContext.Update<T, U>(Expression<Func<T, bool>> predicate, Func<T, U> updater)
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