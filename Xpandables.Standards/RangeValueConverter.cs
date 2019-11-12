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
using System.Globalization;

namespace System
{
    /// <summary>
    /// Provides a type converter to convert <see cref="RangeValue{T}"/> objects to and from various other representations.
    /// </summary>
    public sealed class RangeValueConverter : TypeConverter
    {
        /// <summary>
        /// Gets a value indicating whether this converter can convert an object in the given source type
        /// to a range value object using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="Type"></see> that represents the type you wish to convert from.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => (sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);

        /// <summary>
        /// Gets a value indicating whether this converter can convert an object to the given destination type
        /// using the context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="Type"></see> that represents the type you wish to convert to.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => (destinationType == typeof(string)) || base.CanConvertTo(context, destinationType);

        /// <summary>
        /// Converts the specified value object to a range value object.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">An optional <see cref="CultureInfo"></see>. If not supplied, the current
        /// culture is assumed.</param>
        /// <param name="value">The <see cref="object"></see> to convert.</param>
        /// <returns>An <see cref="object"></see> that represents the converted <paramref name="value">value</paramref>
        /// .</returns>
        /// <exception cref="FormatException"><paramref name="value">value</paramref> is not a valid value
        /// for the target type.</exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is null) throw GetConvertFromException(value);

            if (value is string source)
            {
                var parts = source.Split(':');
                if (parts.Length != 2) throw GetConvertFromException(value);

                var minStr = parts[0];
                var maxStr = parts[1];

                if (int.TryParse(minStr, NumberStyles.Integer, culture, out var minInt)
                    && int.TryParse(maxStr, NumberStyles.Integer, culture, out var maxInt))
                {
                    return new RangeValue<int>(minInt, maxInt);
                }

                if (double.TryParse(minStr, NumberStyles.Float, culture, out var minDouble)
                    && int.TryParse(maxStr, NumberStyles.Float, culture, out var maxDouble))
                {
                    return new RangeValue<double>(minDouble, maxDouble);
                }

                if (DateTime.TryParse(minStr, culture, DateTimeStyles.None, out var minDateTime)
                    && DateTime.TryParse(maxStr, culture, DateTimeStyles.None, out var maxDateTime))
                {
                    return new RangeValue<DateTime>(minDateTime, maxDateTime);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the given value object to the specified destination type.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">An optional <see cref="CultureInfo"></see>. If not supplied, the current
        /// culture is assumed.</param>
        /// <param name="value">The <see cref="object"></see> to convert.</param>
        /// <param name="destinationType">The <see cref="Type"></see> to convert the value to.</param>
        /// <returns>An <see cref="object"></see> that represents the converted <paramref name="value">value</paramref>
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="destinationType">destinationType</paramref> is null.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="value">value</paramref> is not a valid value
        /// for the enumeration.</exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value?.GetType().IsSubclassOf(typeof(RangeValue<>)) == true && destinationType == typeof(string))
                return value.ToString();

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
