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
using System.Reflection;

namespace System
{
    /// <summary>
    /// Helper class used to implement enumeration.
    /// This is an <see langword="abstract"/> and serializable class.
    /// </summary>
    [Serializable]
    public abstract class EnumerationType : IEqualityComparer<EnumerationType>, IEquatable<EnumerationType>, IComparable<EnumerationType>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EnumerationType"/> with the specified value and display name.
        /// </summary>
        /// <param name="displayName">The enumeration display name.</param>
        /// <param name="value">The enumeration value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="displayName"/> is null.</exception>
        protected EnumerationType(string displayName, int value)
        {
            Value = value;
            DisplayName = displayName ?? throw new ArgumentNullException(
                nameof(displayName),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(EnumerationType),
                    nameof(displayName)));
        }

        /// <summary>
        /// Gets the value of the enumeration.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Gets the name of the enumeration.
        /// </summary>
        [field: NonSerialized]
        public string DisplayName { get; }

        /// <summary>
        /// Gets the list of all enumeration found in the current instance.
        /// If you want to return all enumerations from base classes, use the non-generic method.
        /// </summary>
        /// <typeparam name="TEnumeration">Type of derived class enumeration.</typeparam>
        /// <returns>List of enumerations.</returns>
        public static IEnumerable<TEnumeration> GetAll<TEnumeration>()
            where TEnumeration : EnumerationType
            => GetAll(typeof(TEnumeration)).Cast<TEnumeration>();

        /// <summary>
        /// Gets the list of all enumeration found in the instance of the specified type and base classes.
        /// The type must derived from <see cref="EnumerationType"/>.
        /// </summary>
        /// <param name="enumerationType">Type of enumeration.</param>
        /// <returns>List of enumerations.</returns>
        /// <exception cref="ArgumentException">The <paramref name="enumerationType"/> must derive from <see cref="EnumerationType"/>.
        /// </exception>
        public static IEnumerable<object> GetAll(Type enumerationType)
        {
            _ = enumerationType ?? throw new ArgumentNullException(nameof(enumerationType));
            if (!enumerationType.IsSubclassOf(typeof(EnumerationType)))
                throw new ArgumentException($"The type is not a subclass of {typeof(EnumerationType)}", nameof(enumerationType));

            return from info in enumerationType
                    .GetProperties(BindingFlags.Public | BindingFlags.Static
                    | BindingFlags.FlattenHierarchy | BindingFlags.Instance)
                   where info.PropertyType.IsSubclassOf(typeof(EnumerationType)) && info.GetGetMethod() != null
                   select info.GetValue(null);
        }

        /// <summary>
        /// Gets the enumeration matching the specified display name.
        /// </summary>
        /// <typeparam name="TEnumeration">Type of derived class enumeration.</typeparam>
        /// <param name="displayName">The display name to find.</param>
        /// <returns>An instance of <typeparamref name="TEnumeration"/> type or default value.</returns>
        /// <exception cref="InvalidOperationException">The <paramref name="displayName"/> does not exist.</exception>
        public static TEnumeration FromDisplayName<TEnumeration>(string displayName)
            where TEnumeration : EnumerationType
        {
            if (displayName is null) throw new ArgumentNullException(nameof(displayName));
            Func<TEnumeration, bool> predicateFind = new Func<TEnumeration, bool>(PredicateExecute);

            return GetAll<TEnumeration>().FirstOrDefault(predicateFind);

            bool PredicateExecute(TEnumeration enumeration)
                => enumeration.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the enumeration matching the specified type and display name.
        /// </summary>
        /// <param name="enumerationType">Type of derived class enumeration.</param>
        /// <param name="displayName">The display name to find.</param>
        /// <returns>An instance of <paramref name="enumerationType"/> type or default value.</returns>
        /// <exception cref="InvalidOperationException">The <paramref name="displayName"/> does not exist.</exception>
        public static object FromDisplayName(Type enumerationType, string displayName)
        {
            if (enumerationType is null) throw new ArgumentNullException(nameof(enumerationType));
            if (displayName is null) throw new ArgumentNullException(nameof(displayName));

            Func<EnumerationType, bool> predicateFind = new Func<EnumerationType, bool>(PredicateExecute);

            return GetAll(enumerationType).OfType<EnumerationType>().FirstOrDefault(predicateFind);

            bool PredicateExecute(EnumerationType enumeration)
                => enumeration.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the enumeration matching the specified value.
        /// </summary>
        /// <typeparam name="TEnumeration">Type of derived class enumeration.</typeparam>
        /// <param name="value">Value to find.</param>
        /// <returns>An instance of <typeparamref name="TEnumeration"/> type or default value.</returns>
        /// <exception cref="InvalidOperationException">The <paramref name="value"/> does not exist.</exception>
        public static TEnumeration FromValue<TEnumeration>(int value)
            where TEnumeration : EnumerationType
        {
            Func<TEnumeration, bool> predicateFind = new Func<TEnumeration, bool>(PredicateExecute);

            return GetAll<TEnumeration>().FirstOrDefault(predicateFind);

            bool PredicateExecute(TEnumeration enumeration) => enumeration.Value.Equals(value);
        }

        /// <summary>
        /// Gets the enumeration matching the specified type and value.
        /// </summary>
        /// <param name="enumerationType">Type of derived class enumeration.</param>
        /// <param name="value">Value to find.</param>
        /// <returns>An instance of <paramref name="enumerationType"/> type or default value.</returns>
        /// <exception cref="InvalidOperationException">The <paramref name="value"/> does not exist.</exception>
        public static object FromValue(Type enumerationType, int value)
        {
            if (enumerationType is null) throw new ArgumentNullException(nameof(enumerationType));
            Func<EnumerationType, bool> predicateFind = new Func<EnumerationType, bool>(PredicateExecute);

            return GetAll(enumerationType).OfType<EnumerationType>().FirstOrDefault(predicateFind);

            bool PredicateExecute(EnumerationType enumeration) => enumeration.Value.Equals(value);
        }

        /// <summary>
        /// Returns the absolute difference of both enumerations.
        /// </summary>
        /// <param name="first">The first instance to act on.</param>
        /// <param name="second">The second instance to act on.</param>
        /// <returns>An integer that represents an absolute comparison value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="first"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="second"/> is null.</exception>
        public static int AbsoluteDifference(EnumerationType first, EnumerationType second)
        {
            if (first is null) throw new ArgumentNullException(nameof(first));
            if (second is null) throw new ArgumentNullException(nameof(second));

            return Math.Abs(first.Value - second.Value);
        }

        /// <summary>
        /// Returns the description string attribute of the current enumeration.
        /// if not found, returns the enumeration as string.
        /// </summary>
        /// <returns>The description string. If not found, returns the enumeration as string.</returns>
        public string GetDescriptionAttributeValue()
            => GetType().GetCustomAttribute<DescriptionAttribute>()
                .AsOptional()
                .Map(attr => attr.Description)
                .WhenEmpty(() => DisplayName);

        /// <summary>
        /// Returns the comparison value of both <see cref="EnumerationType"/> objects.
        /// </summary>
        /// <param name="other">The other object for comparison.</param>
        /// <returns>An integer value.</returns>
        public virtual int CompareTo(EnumerationType other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));
            return Value.CompareTo(other.Value) & string.CompareOrdinal(DisplayName, other.DisplayName);
        }

        /// <summary>
        /// Returns the comparison value of both <see cref="EnumerationType"/> objects.
        /// </summary>
        /// <param name="other">The other object for comparison..</param>
        /// <returns>A boolean value.</returns>
        public bool Equals(EnumerationType other)
        {
            if (other is null)
            {
                return false;
            }

            return GetType() == other.GetType()
                && Value.Equals(other.Value)
                && DisplayName.Equals(other.DisplayName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the hash-code of the current type.
        /// </summary>
        /// <returns>hash-code.</returns>
        public override int GetHashCode()
            => Value.GetHashCode() + DisplayName.GetHashCode(StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Returns the comparison value of both <see cref="EnumerationType"/> objects.
        /// </summary>
        /// <param name="obj">The other object for comparison.</param>
        /// <returns>A boolean value.</returns>
        public override bool Equals(object obj)
            => Equals((EnumerationType)obj);

        /// <summary>
        /// Returns the <see cref="string"/> value matching the <see cref="DisplayName"/>.
        /// </summary>
        public override string ToString() => DisplayName;

        /// <summary>
        /// Implicit returns the <see cref="string"/> value of the display name.
        /// </summary>
        /// <param name="enumeration">current instance.</param>
        public static implicit operator string(EnumerationType enumeration)
            => enumeration?.DisplayName ?? string.Empty;

        /// <summary>
        /// Implicit returns the <see cref="int"/> value.
        /// </summary>
        /// <param name="enumeration">current instance.</param>
        [Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Les surcharges d'opérateur offrent d'autres méthodes nommées",
            Justification = "<En attente>")]
        public static implicit operator int(EnumerationType enumeration) => enumeration?.Value ?? default;

        /// <inheritdoc />
        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(EnumerationType x, EnumerationType y) => x?.Equals(y) ?? false;

        /// <inheritdoc />
        /// <summary>Returns a hash code for the specified object.</summary>
        /// <param name="obj">The object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(EnumerationType obj) => obj?.GetHashCode() ?? 0;

        /// <summary>
        /// Compares equality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(EnumerationType left, EnumerationType right) => left?.Equals(right) ?? right is null;

        /// <summary>
        ///Compares difference.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(EnumerationType left, EnumerationType right) => !(left == right);

        /// <summary>
        /// Compares lower than.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(EnumerationType left, EnumerationType right)
            => left is null ? !(right is null) : left.CompareTo(right) < 0;

        /// <summary>
        /// Compares lower or equal to.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <=(EnumerationType left, EnumerationType right) => left is null || left.CompareTo(right) <= 0;

        /// <summary>
        /// Compares greater than.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(EnumerationType left, EnumerationType right) => !(left is null) && left.CompareTo(right) > 0;

        /// <summary>
        /// Compares greater or equal to.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >=(EnumerationType left, EnumerationType right)
            => left is null ? right is null : left.CompareTo(right) >= 0;
    }
}