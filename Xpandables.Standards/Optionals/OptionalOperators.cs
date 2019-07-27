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
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public partial class Optional<T>
    {
#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
        public static bool operator ==(in Optional<T> a, in Optional<T> b) => a.Equals(b);

        public static bool operator !=(in Optional<T> a, in Optional<T> b) => !a.Equals(b);

        public static bool operator ==(in Optional<T> a, in T b) => a.Equals(b);

        public static bool operator !=(in Optional<T> a, in T b) => !a.Equals(b);

        public static bool operator ==(in T a, in Optional<T> b) => b.Equals(a);

        public static bool operator !=(in T a, in Optional<T> b) => !b.Equals(a);

        public static implicit operator T(Optional<T> optional) => optional.Any() ? optional.Single() : default;

        public static implicit operator Optional<T>(Optional<Optional<T>> doubleOptional)
            => doubleOptional.Any() && doubleOptional.Single().Any()
                    ? doubleOptional.Single()
                    : Empty();

        public static implicit operator Optional<T>(T value)
            => !EqualityComparer<T>.Default.Equals(value, default) ? Some(value) : Empty();

        public static implicit operator Task<Optional<T>>(Optional<T> optional) => Task.FromResult(optional);

        public static bool operator <(Optional<T> left, Optional<T> right)
        {
            return left is null ? !(right is null) : left.CompareTo(right) < 0;
        }

        public static bool operator <=(Optional<T> left, Optional<T> right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(Optional<T> left, Optional<T> right)
        {
            return !(left is null) && left.CompareTo(right) > 0;
        }

        public static bool operator >=(Optional<T> left, Optional<T> right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }
#pragma warning restore CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
    }
}