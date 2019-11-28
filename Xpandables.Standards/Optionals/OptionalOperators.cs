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

using System.Threading.Tasks;

namespace System
{
    [Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Les identificateurs ne doivent pas correspondre à des mots clés",
        Justification = "<En attente>")]
    [Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Les identificateurs doivent avoir un suffixe correct",
        Justification = "<En attente>")]
    public partial class Optional<T>
    {
        public static bool operator ==(in Optional<T> a, in Optional<T> b) => a?.Equals(b) == true;

        public static bool operator !=(in Optional<T> a, in Optional<T> b) => !(a?.Equals(b) == true);

        public static bool operator ==(in Optional<T> a, in T b) => a?.Equals(b) == true;

        public static bool operator !=(in Optional<T> a, in T b) => !(a?.Equals(b) == true);

        public static bool operator ==(in T a, in Optional<T> b) => b?.Equals(a) == true;

        public static bool operator !=(in T a, in Optional<T> b) => !(b?.Equals(a) == true);

        [Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Les surcharges d'opérateur offrent d'autres méthodes nommées",
            Justification = "<En attente>")]
        public static implicit operator T(Optional<T> optional)
            => optional is null ? (default) : optional.IsValue() ? optional.InternalValue : default;

        [Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Les surcharges d'opérateur offrent d'autres méthodes nommées",
            Justification = "<En attente>")]
        public static implicit operator Optional<T>(T value) => value.AsOptional();

        [Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Les surcharges d'opérateur offrent d'autres méthodes nommées",
            Justification = "<En attente>")]
        public static implicit operator Task<Optional<T>>(Optional<T> optional)
            => optional is null ? Empty() : Task.FromResult(optional);

        [Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Les surcharges d'opérateur offrent d'autres méthodes nommées",
            Justification = "<En attente>")]
        public static implicit operator ValueTask<Optional<T>>(Optional<T> optional)
            => optional is null ? Empty() : new ValueTask<Optional<T>>(optional);

        [Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Les surcharges d'opérateur offrent d'autres méthodes nommées",
            Justification = "<En attente>")]
        public static implicit operator Optional<T>(Optional<Optional<T>> optional)
            => optional?.InternalValue ?? Optional<T>.Empty();

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
    }
}