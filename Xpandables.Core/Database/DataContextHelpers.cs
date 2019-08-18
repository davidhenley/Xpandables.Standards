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
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq;
using System.Reflection;

namespace System.Design.Database
{
#pragma warning disable HAA0401 // Possible allocation of reference type enumerator
    /// <summary>
    /// Extends the <see cref="ModelBuilder"/> class to apply conversion to <see cref="EnumerationType"/> types.
    /// </summary>
    /// <remarks>From https://github.com/aspnet/EntityFrameworkCore/issues/10784</remarks>
    public static class ModelBuilderHelpers
    {
        /// <summary>
        /// Specifies the converter to be used for the generic property type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The generic target <typeparamref name="T"/> to act on.</typeparam>
        /// <param name="this">The current model builder instance.</param>
        /// <param name="valueConverter">The converter to be applied.</param>
        /// <returns>The model builder with converter application.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="this"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueConverter"/> is null.</exception>
        public static ModelBuilder UseEnumerationValueConverterForType<T>(
            this ModelBuilder @this,
            ValueConverter<T, string> valueConverter)
            where T : EnumerationType
            => @this.UseEnumerationValueConverterForType(typeof(T), valueConverter);

        /// <summary>
        /// Specifies the converter to be used for the property type (<see cref="EnumerationType"/>).
        /// </summary>
        /// <param name="this">The current model builder instance.</param>
        /// <param name="type">The target type to act on.</param>
        /// <param name="valueConverter">The converter to be applied.</param>
        /// <returns>The model builder with converter application.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="this"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueConverter"/> is null.</exception>
        public static ModelBuilder UseEnumerationValueConverterForType(
            this ModelBuilder @this,
            Type type,
            ValueConverter valueConverter)
        {
            var isTypeEnumerationFunc = new Func<IMutableEntityType, bool>(IsTypeEnumeration);
            var isPropertyTypeFunc = new Func<PropertyInfo, bool>(IsPropertyType);

            foreach (var entityType in @this.Model.GetEntityTypes().Where(isTypeEnumerationFunc))
                foreach (var property in entityType.ClrType.GetProperties().Where(isPropertyTypeFunc))
                    @this.Entity(entityType.Name).Property(property.Name).HasConversion(valueConverter);

            return @this;

            static bool IsTypeEnumeration(IMutableEntityType entityType)
                => !entityType.ClrType.IsSubclassOf(typeof(EnumerationType));
            bool IsPropertyType(PropertyInfo propertyInfo) => propertyInfo.PropertyType == type;
        }
    }
}