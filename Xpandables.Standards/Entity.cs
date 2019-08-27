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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace System
{
    /// <summary>
    /// The domain object base implementation that provide an identifier and a key generator for derived class.
    /// This is an <see langword="abstract"/> and serializable class.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Id}")]
    public abstract class Entity
    {
        [NonSerialized]
        private string _id = string.Empty;

        /// <summary>
        /// Gets the domain object identity.
        /// The value comes from <see cref="KeyGenerator"/>.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id
        {
            get => !string.IsNullOrWhiteSpace(_id) ? _id : (_id = KeyGenerator());
            protected internal set => _id = value;
        }

        /// <summary>
        /// Determines whether or not the underlying instance is new.
        /// The default implementation just compare the <see cref="Id"/> value to its default one.
        /// You must override this property in order to match your request.
        /// </summary>
        public bool IsNew() => string.IsNullOrWhiteSpace(Id);

        /// <summary>
        /// Determines whether or not the underlying instance is deleted.
        /// </summary>
        public bool IsDeleted { get; protected set; }

        /// <summary>
        /// Gets or sets the creation date of the underlying instance.
        /// </summary>
        [DataType(DataType.DateTime)]
        public bool CreatedOn { get; protected set; }

        /// <summary>
        /// Returns the unique signature of string type for an instance.
        /// This signature value will be used as identifier for the underlying instance.
        /// <para>When overridden in the derived class, it will set or get the concrete identity for the domain object.</para>
        /// </summary>
        /// <returns>A string value as identifier.</returns>
        protected virtual string KeyGenerator()
        {
            using var rnd = RandomNumberGenerator.Create();

            var salt = new byte[32];
            var guid = Guid.NewGuid().ToString();
            rnd.GetBytes(salt);

            return $"{guid}{BitConverter.ToString(salt)}";
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Entity other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            if (string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(other.Id))
                return false;

            return Id == other.Id;
        }

        /// <summary>
        /// Applies equal operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Entity a, Entity b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        /// <summary>
        /// Applies non equal operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Entity a, Entity b) => !(a == b);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current entity.</returns>
        public override int GetHashCode() => (GetType().ToString() + Id).GetHashCode(StringComparison.InvariantCulture);
    }
}