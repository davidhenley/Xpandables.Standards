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

using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace System
{
    /// <summary>
    /// Generic extension methods.
    /// </summary>
    public static class GenericExtensions
    {
        /// <summary>
        /// Sets properties via lambda expression scope.
        /// This is similar to the VB.Net key word <see lanwgord="With"/>..<see lanwgord="EndWith"/>.
        /// </summary>
        /// <typeparam name="T">Type source.</typeparam>
        /// <param name="source">The source item to act on.</param>
        /// <param name="action">The action to be applied</param>
        /// <returns>The same object after applying the action on it.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        public static T With<T>(this T source, Action<T> action)
            where T : class
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (action is null) throw new ArgumentNullException(nameof(action));

            action.Invoke(source);
            return source;
        }

        /// <summary>
        /// Sets properties via lambda expression. This is useful when dealing with <see cref="Expression{TDelegate}"/>.
        /// </summary>
        /// <typeparam name="T">Type source.</typeparam>
        /// <param name="source">The source instance to act on.</param>
        /// <param name="nameOfExpression">The expression delegate for the property.
        /// Just use <see langword="nameof"/> as expression for the delegate.</param>
        /// <param name="value">The value for the property.</param>
        /// <returns>The current instance with modified property.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="nameOfExpression"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="nameOfExpression"/> is not valid.</exception>
        public static T With<T>(this T source, Expression<Func<T, string>> nameOfExpression, object value)
            where T : class
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (nameOfExpression is null) throw new ArgumentNullException(nameof(nameOfExpression));
            if (!(nameOfExpression.Body is ConstantExpression constantExpression))
                throw new ArgumentNullException($"Constant Expression expected. {nameof(nameOfExpression)}");
            if (!(source.GetType().GetProperty(constantExpression.Value.ToString()) is PropertyInfo propertyInfo))
                throw new ArgumentException($"Property {constantExpression.Value} does not exist in the {source.GetType().Name}.");
            if (!(propertyInfo.GetSetMethod() is MethodInfo))
                throw new ArgumentException($"Property {propertyInfo.Name} is not settable.");
            if (value != null && !propertyInfo.PropertyType.IsAssignableFrom(value.GetType()))
                throw new ArgumentException($"Property type of {propertyInfo.Name} and type of the value does not match.");

            propertyInfo.SetValue(source, value);
            return source;
        }

        /// <summary>
        /// Returns the attribute of the <typeparamref name="TAttribute"/> type from the object.
        /// </summary>
        /// <typeparam name="TAttribute">The Type of attribute.</typeparam>
        /// <param name="source">enumeration instance to act on.</param>
        /// <param name="inherit"><see langword="true"/> to inspect the ancestors of element;
        /// otherwise, <see langword="false"/>.</param>
        /// <returns>The description string. If not found, returns the enumeration as string.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static Optional<TAttribute> GetAttribute<TAttribute>(this object source, bool inherit = true)
            where TAttribute : Attribute
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            try
            {
                return source.GetType().GetTypeInfo().GetCustomAttribute<TAttribute>(inherit);
            }
            catch (Exception exception) when (exception is NotSupportedException
                                            || exception is AmbiguousMatchException
                                            || exception is TypeLoadException)
            {
                return OptionalBuilder.Exception<TAttribute>(exception);
            }
        }

        /// <summary>
        /// Returns the attribute of the <typeparamref name="TAttribute"/> type from the type.
        /// </summary>
        /// <typeparam name="TAttribute">Type of attribute.</typeparam>
        /// <param name="source">enumeration instance to act on.</param>
        /// <param name="inherit"><see langword="true"/> to inspect the ancestors of element;
        /// otherwise, <see langword="false"/>.</param>
        /// <returns>The description string. If not found, returns the enumeration as string.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static Optional<TAttribute> GetAttribute<TAttribute>(this Type source, bool inherit = true)
            where TAttribute : Attribute
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            try
            {
                return source.GetType().GetTypeInfo().GetCustomAttribute<TAttribute>(inherit);
            }
            catch (Exception exception) when (exception is NotSupportedException
                                            || exception is AmbiguousMatchException
                                            || exception is TypeLoadException)
            {
                return OptionalBuilder.Exception<TAttribute>(exception);
            }
        }

        /// <summary>
        /// Returns the attribute of the <typeparamref name="TAttribute"/> type from the type.
        /// </summary>
        /// <typeparam name="TAttribute">Type of attribute.</typeparam>
        /// <param name="source">enumeration instance to act on.</param>
        /// <param name="inherit"><see langword="true"/> to inspect the ancestors of element;
        /// otherwise, <see langword="false"/>.</param>
        /// <returns>The description string. If not found, returns the enumeration as string.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static Optional<TAttribute> GetAttribute<TAttribute>(this PropertyInfo source, bool inherit = true)
            where TAttribute : Attribute
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            try
            {
                return source.GetType().GetTypeInfo().GetCustomAttribute<TAttribute>(inherit);
            }
            catch (Exception exception) when (exception is NotSupportedException
                                            || exception is AmbiguousMatchException
                                            || exception is TypeLoadException)
            {
                return OptionalBuilder.Exception<TAttribute>(exception);
            }
        }

        /// <summary>
        /// Returns the description string attribute of the current <see cref="Enum"/> value type.
        /// if not found, returns the value as string.
        /// </summary>
        /// <typeparam name="TEnum">Type of enumeration.</typeparam>
        /// <param name="value">Enumeration field value to act on.</param>
        /// <returns>The description string. If not found, returns the value as string.</returns>
        public static string GetDescriptionAttribute<TEnum>(this TEnum value)
            where TEnum : Enum
            => typeof(TEnum)
                .GetField($"{value}")
                .AsOptional()
                .MapOptional(field => field.GetAttribute<DescriptionAttribute>())
                .Map(attr => attr.Description)
                .WhenEmpty(() => $"{value}")
                .GetValueOrDefault();

        /// <summary>
        /// Converts the current enumeration value to the target one.
        /// </summary>
        /// <typeparam name="TEnum">Type of target value.</typeparam>
        /// <param name="source">The current enumeration value.</param>
        /// <returns>An new enumeration of <typeparamref name="TEnum"/> type.</returns>
        public static Optional<TEnum> ConvertTo<TEnum>(this Enum source)
            where TEnum : struct, Enum
        {
            try
            {
                return (TEnum)Enum.Parse(typeof(TEnum), $"{source}");
            }
            catch (OverflowException exception)
            {
                return OptionalBuilder.Exception<TEnum>(exception);
            }
        }

        /// <summary>
        /// Converts the current enumeration value to the target one.
        /// </summary>
        /// <typeparam name="TEnum">Type of target value.</typeparam>
        /// <param name="source">The current enumeration value.</param>
        /// <returns>An new enumeration of <typeparamref name="TEnum"/> type.</returns>
        /// <exception cref="InvalidOperationException">The conversion failed. See inner exception.</exception>
        public static Optional<TEnum> ConvertTo<TEnum>(this string source)
            where TEnum : struct, Enum
        {
            try
            {
                return (TEnum)Enum.Parse(typeof(TEnum), source);
            }
            catch (OverflowException exception)
            {
                return OptionalBuilder.Exception<TEnum>(exception);
            }
        }
    }
}