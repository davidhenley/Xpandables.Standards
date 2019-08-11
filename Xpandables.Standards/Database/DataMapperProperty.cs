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

using System;
using System.Collections;
using System.Diagnostics;

namespace Xpandables.Database
{
    public class DataMapperProperty : IFluent, IDataMapperProperty
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="dataPrefix"></param>
        /// <param name="dataName"></param>
        /// <param name="type"></param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataPrefix"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        public DataMapperProperty(string propertyName, string dataPrefix, string dataName, Type type)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            DataPrefix = dataPrefix ?? throw new ArgumentNullException(nameof(dataPrefix));
            DataName = dataName ?? throw new ArgumentNullException(nameof(dataName));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="dataName"></param>
        /// <param name="type"></param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        public DataMapperProperty(string propertyName, string dataName, Type type)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            DataName = dataName ?? throw new ArgumentNullException(nameof(dataName));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public string PropertyName { get; }
        public string DataName { get; }
        public string DataPrefix { get; }
        public string DataFullName => $"{DataPrefix}{DataName}";
        public Type Type { get; }
        public bool IsIdentity { get; protected set; }
        public bool IsEnumerable => Type.IsEnumerable();
        public bool IsNullable => Type.IsNullable();
        public bool IsPrimitive => Type.IsValueType || Type.Equals(typeof(string));
        public void SetValue(object target, object value)
        {
            var dataValue = value;
            if (dataValue is null || dataValue is DBNull)
                dataValue = null;
            else
            if (Type == typeof(DayOfWeek))
                dataValue = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), dataValue.ToString());

            try
            {
                var propertyInfo = target.GetType().GetProperty(PropertyName);
                if (IsEnumerable)
                {
                    if (propertyInfo.GetValue(target, null) is null)
                        propertyInfo.SetValue(target, CreateStronglyTypedInstance());

                    ((IList)propertyInfo.GetValue(target, null)).Add(dataValue);
                }
                else
                    propertyInfo.SetValue(target, dataValue);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

        public object CreateInstance()
        {
            var targetType = Type;

            if (IsNullable)
                targetType = Nullable.GetUnderlyingType(Type);
            else if (IsEnumerable)
                targetType = Type.GetGenericArguments()[0];

            return Activator.CreateInstance(targetType);
        }

        public object CreateStronglyTypedInstance()
        {
            var targetType = Type;

            if (IsNullable)
                targetType = Nullable.GetUnderlyingType(Type);

            return Activator.CreateInstance(targetType);
        }

        public IDataMapperProperty AllowIdentity(bool isIdentity)
        {
            IsIdentity = isIdentity;
            return this;
        }
    }

    public sealed class DataMapperProperty<T> : DataMapperProperty, IDataMapperProperty<T>
        where T : class, new()
    {
        public DataMapperProperty(string propertyName, string dataPrefix, string dataName, Type type)
            : base(propertyName, dataPrefix, dataName, type) { }

        public DataMapperProperty(string propertyName, string dataName, Type type)
            : base(propertyName, dataName, type) { }

        public void SetValue(T target, object value) => base.SetValue(target, value);

        void IDataMapperProperty.SetValue(object target, object value) => SetValue(target as T, value);

        void IDataMapperProperty<T>.SetValue(object target, object value) => SetValue(target as T, value);

        public new IDataMapperProperty<T> AllowIdentity(bool isIdentity)
        {
            IsIdentity = isIdentity;
            return this;
        }
    }
}
