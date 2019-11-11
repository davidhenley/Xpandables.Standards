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
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// Implementation for <see cref="INotifyPropertyChanged"/>.
    /// You can combine the use with <see cref="NotifyPropertyChangedDependOnAttribute"/> to propagate notification.
    /// <para>This is an <see langword="abstract"/> and serializable class.</para>
    /// </summary>
    /// <typeparam name="T">Type of the derived class.</typeparam>
    [Serializable]
    public abstract class NotifyPropertyChanged<T> : INotifyPropertyChanged
        where T : class
    {
        /// <summary>
        /// Initializes the <see cref="Dependencies"/> collection.
        /// </summary>
        protected NotifyPropertyChanged()
        {
            Dependencies = DependenciesProvider();
            PropertyChanged = (s, e) => { };
        }

        /// <summary>
        /// Checks if the property does not match the old one.
        /// If so, sets the property and notifies listeners.
        /// </summary>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <typeparam name="TProperty">Type of the property selector.</typeparam>
        /// <param name="storage">The current value of the property (the back-end property).</param>
        /// <param name="value">The new value of the property (the value).</param>
        /// <param name="selector">The expression delegate to retrieve the property name.</param>
        /// <returns><see langword="true"/>if the value was changed, <see langword="false"/>
        /// if the existing value matches the desired value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        protected bool SetProperty<TValue, TProperty>(
            ref TValue storage, TValue value, Expression<Func<T, TProperty>> selector)
            => SetProperty(ref storage, value, GetMemberNameFromExpression(selector));

        /// <summary>
        /// Checks if the property does not match the old one.
        /// If so, sets the property and notifies listeners.
        /// </summary>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="storage">The current value of the property (the back-end property).</param>
        /// <param name="value">The new value of the property (the value).</param>
        /// <param name="selector">The expression delegate to retrieve the property name.
        /// The expression expected is <see langword="nameof"/> with a delegate.</param>
        /// <returns><see langword="true"/>if the value was changed, <see langword="false"/>i
        /// f the existing value matched the desired value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="selector"/> is not a <see cref="ConstantExpression"/>.</exception>
        protected bool SetProperty<TValue>(
            ref TValue storage, TValue value, Expression<Func<T, string>> selector)
            => SetProperty(ref storage, value, GetMemberNameFromExpression(selector));

        /// <summary>
        /// Checks if the property does not match the old one.
        /// If so, sets the property and notifies listeners.
        /// </summary>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="storage">The current value of the property (the back-end property).</param>
        /// <param name="value">The new value of the property (the value).</param>
        /// <param name="propertyName">The name of the property. Optional (Already known at compile time).</param>
        /// <returns><see langword="true"/>if the value was changed, <see langword="false"/>
        /// if the existing value matches the desired value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyName"/> is null or empty.</exception>
        protected virtual bool SetProperty<TValue>(
            ref TValue storage, TValue value, [CallerMemberName] string propertyName = "")
        {
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException(nameof(propertyName));

            if (EqualityComparer<TValue>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            PropagatePropertyChangedOnDependents(propertyName);

            return true;

            void PropagatePropertyChangedOnDependents(string property)
            {
                Action<string> onPropertyChangedAction = new Action<string>(OnPropertyChanged);

                (from keyValues in Dependencies
                 from dependent in keyValues.Value
                 where keyValues.Key.Equals(property, StringComparison.OrdinalIgnoreCase)
                 select dependent)
                 .ToList()
                 .ForEach(onPropertyChangedAction);
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The property name that has changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Contains a collection of dependencies on property changed notification.
        /// </summary>
        private IDictionary<string, List<string>> Dependencies { get; }

        /// <summary>
        /// Event raised when a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Returns the member name from the expression.
        /// The expression delegate is <see langword="nameof"/>, otherwise the result is null.
        /// </summary>
        /// <param name="nameOfExpression">The expression delegate for the property : <see langword="nameof"/>
        /// with delegate expected.</param>
        /// <returns>A string that represents the name of the member found in the expression.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="nameOfExpression"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="nameOfExpression"/> is
        /// not a <see cref="ConstantExpression"/>.</exception>
        private static string GetMemberNameFromExpression(Expression<Func<T, string>> nameOfExpression)
        {
            if (nameOfExpression is null) throw new ArgumentNullException(nameof(nameOfExpression));

            return nameOfExpression.Body is ConstantExpression constantExpression
                ? constantExpression.Value.ToString()
                : throw new ArgumentException(
                    ErrorMessageResources.PropertyChangedMemberExpressionExpected,
                    nameof(nameOfExpression));
        }

        /// <summary>
        /// Returns the member name from the expression if found, otherwise returns null.
        /// </summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member name.</param>
        /// <returns>A string that represents the name of the member found in the expression.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyExpression"/> is null.</exception>
        private static string GetMemberNameFromExpression<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            if (propertyExpression is null) throw new ArgumentNullException(nameof(propertyExpression));

            return (propertyExpression.Body as MemberExpression
                ?? ((UnaryExpression)propertyExpression.Body).Operand as MemberExpression)
                ?.Member.Name ??
                throw new ArgumentException(
                    ErrorMessageResources.PropertyChangedMemberExpressionExpected,
                    nameof(propertyExpression));
        }

        /// <summary>
        /// Provides with the collection of dependencies found in the underlying type.
        /// </summary>
        protected IDictionary<string, List<string>> DependenciesProvider()
        {
            var dependencies = new Dictionary<string, List<string>>();

            var properties = (from p in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                              where p.GetCustomAttributes<NotifyPropertyChangedDependOnAttribute>(true).Any()
                              select p).ToArray();

            foreach (var property in properties)
            {
                var attributes = (from a in property.GetCustomAttributes<NotifyPropertyChangedDependOnAttribute>(true)
                                  select a.Name).ToArray();
                foreach (var dependency in attributes)
                {
                    if (property.Name == dependency)
                    {
                        throw new InvalidOperationException(
                            ErrorMessageResources.PropertyChangedCircularDependency,
                            new ArgumentException(
                                ErrorMessageResources
                                    .PropertyChangedCircularDependencyItself
                                        .StringFormat(dependency, typeof(T).Name)));
                    }

                    if (dependencies.TryGetValue(dependency, out var notifiers))
                    {
                        Predicate<string> predicateProperty = new Predicate<string>(PredicateFindProperty);
                        if (notifiers.Find(predicateProperty) != null)
                        {
                            throw new InvalidOperationException(
                                ErrorMessageResources.PropertyChangedDuplicateDependency,
                                new ArgumentException(
                                    ErrorMessageResources
                                        .PropertyChangedDuplicateDependencyMore
                                            .StringFormat(property.Name, dependency)));
                        }

                        notifiers.Add(property.Name);
                    }
                    else
                    {
                        Predicate<string> predicateFind = new Predicate<string>(PredicateFindDependency);
                        if (dependencies.TryGetValue(property.Name, out var propertyNotifiers)
                            && propertyNotifiers.Find(predicateFind) != null)
                        {
                            throw new InvalidOperationException(
                                ErrorMessageResources.PropertyChangedCircularDependency,
                                new ArgumentException(
                                    ErrorMessageResources
                                        .PropertyChangedCircularDependencyMore
                                            .StringFormat(property.Name, dependency, property.Name)));
                        }

                        dependencies.Add(dependency, new List<string> { property.Name });
                    }

                    bool PredicateFindDependency(string value) => value == dependency;
                    bool PredicateFindProperty(string value) => value == property.Name;
                }
            }
            return dependencies;
        }
    }
}