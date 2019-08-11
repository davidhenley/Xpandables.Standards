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
using System.ComponentModel;

namespace Xpandables.Database
{
    public interface IDataMapperProperty
    {
        string PropertyName { get; }
        string DataFullName { get; }
        bool IsNullable { get; }
        bool IsPrimitive { get; }
        bool IsIdentity { get; }
        bool IsEnumerable { get; }
        string DataName { get; }
        string DataPrefix { get; }
        Type Type { get; }
        IDataMapperProperty AllowIdentity(bool isIdentity);

        /// <summary>
        ///
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is null.</exception>
        void SetValue(object target, object value);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        object CreateInstance();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        object CreateStronglyTypedInstance();
    }

    public interface IDataMapperProperty<T> : IDataMapperProperty
        where T : class, new()
    {
        void SetValue(T target, object value);

        new IDataMapperProperty<T> AllowIdentity(bool isIdentity);

        [EditorBrowsable(EditorBrowsableState.Never)]
        new void SetValue(object target, object value);
    }
}