/************************************************************************************************************
 * The MIT License (MIT)
 *
 * Copyright (c) 2015 Kristian Hellang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
************************************************************************************************************/

using System.Collections.Generic;
using System.Linq;

namespace System.Design.DependencyInjection
{
    internal class ImplementationTypeFilter : IImplementationTypeFilter
    {
        public ImplementationTypeFilter(IEnumerable<Type> types)
        {
            Types = types;
        }

        internal IEnumerable<Type> Types { get; private set; }

        public IImplementationTypeFilter AssignableTo<T>()
        {
            return AssignableTo(typeof(T));
        }

        public IImplementationTypeFilter AssignableTo(Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));

            return AssignableToAny(type);
        }

        public IImplementationTypeFilter AssignableToAny(params Type[] types)
        {
            if (types is null) throw new ArgumentNullException(nameof(types));
            if (types.Length == 0) throw new ArgumentOutOfRangeException(nameof(types));

            return AssignableToAny(types.AsEnumerable());
        }

        public IImplementationTypeFilter AssignableToAny(IEnumerable<Type> types)
        {
            if (types is null) throw new ArgumentNullException(nameof(types));
            if (!types.Any()) throw new ArgumentOutOfRangeException(nameof(types));

            return Where(t => types.Any(t.IsAssignableTo));
        }

        public IImplementationTypeFilter WithAttribute<T>() where T : Attribute
        {
            return WithAttribute(typeof(T));
        }

        public IImplementationTypeFilter WithAttribute(Type attributeType)
        {
            if (attributeType is null) throw new ArgumentNullException(nameof(attributeType));

            return Where(t => t.HasAttribute(attributeType));
        }

        public IImplementationTypeFilter WithAttribute<T>(Func<T, bool> predicate) where T : Attribute
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            return Where(t => t.HasAttribute(predicate));
        }

        public IImplementationTypeFilter WithoutAttribute<T>() where T : Attribute
        {
            return WithoutAttribute(typeof(T));
        }

        public IImplementationTypeFilter WithoutAttribute(Type attributeType)
        {
            if (attributeType is null) throw new ArgumentNullException(nameof(attributeType));

            return Where(t => !t.HasAttribute(attributeType));
        }

        public IImplementationTypeFilter WithoutAttribute<T>(Func<T, bool> predicate) where T : Attribute
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            return Where(t => !t.HasAttribute(predicate));
        }

        public IImplementationTypeFilter InNamespaceOf<T>()
        {
            return InNamespaceOf(typeof(T));
        }

        public IImplementationTypeFilter InNamespaceOf(params Type[] types)
        {
            if (types is null) throw new ArgumentNullException(nameof(types));
            if (types.Length == 0) throw new ArgumentOutOfRangeException(nameof(types));

            return InNamespaces(types.Select(t => t.Namespace));
        }

        public IImplementationTypeFilter InNamespaces(params string[] namespaces)
        {
            if (namespaces is null) throw new ArgumentNullException(nameof(namespaces));
            if (namespaces.Length == 0) throw new ArgumentOutOfRangeException(nameof(namespaces));

            return InNamespaces(namespaces.AsEnumerable());
        }

        public IImplementationTypeFilter InExactNamespaceOf<T>()
        {
            return InExactNamespaceOf(typeof(T));
        }

        public IImplementationTypeFilter InExactNamespaceOf(params Type[] types)
        {
            if (types is null) throw new ArgumentNullException(nameof(types));
            if (types.Length == 0) throw new ArgumentOutOfRangeException(nameof(types));

            return Where(t => types.Any(x => t.IsInExactNamespace(x.Namespace)));
        }

        public IImplementationTypeFilter InExactNamespaces(params string[] namespaces)
        {
            if (namespaces is null) throw new ArgumentNullException(nameof(namespaces));
            if (namespaces.Length == 0) throw new ArgumentOutOfRangeException(nameof(namespaces));

            return Where(t => namespaces.Any(t.IsInExactNamespace));
        }

        public IImplementationTypeFilter InNamespaces(IEnumerable<string> namespaces)
        {
            if (namespaces is null) throw new ArgumentNullException(nameof(namespaces));
            if (!namespaces.Any()) throw new ArgumentOutOfRangeException(nameof(namespaces));

            return Where(t => namespaces.Any(t.IsInNamespace));
        }

        public IImplementationTypeFilter NotInNamespaceOf<T>()
        {
            return NotInNamespaceOf(typeof(T));
        }

        public IImplementationTypeFilter NotInNamespaceOf(params Type[] types)
        {
            if (types is null) throw new ArgumentNullException(nameof(types));
            if (types.Length == 0) throw new ArgumentOutOfRangeException(nameof(types));

            return NotInNamespaces(types.Select(t => t.Namespace));
        }

        public IImplementationTypeFilter NotInNamespaces(params string[] namespaces)
        {
            if (namespaces is null) throw new ArgumentNullException(nameof(namespaces));
            if (namespaces.Length == 0) throw new ArgumentOutOfRangeException(nameof(namespaces));

            return NotInNamespaces(namespaces.AsEnumerable());
        }

        public IImplementationTypeFilter NotInNamespaces(IEnumerable<string> namespaces)
        {
            if (namespaces is null) throw new ArgumentNullException(nameof(namespaces));
            if (!namespaces.Any()) throw new ArgumentOutOfRangeException(nameof(namespaces));

            return Where(t => namespaces.All(ns => !t.IsInNamespace(ns)));
        }

        public IImplementationTypeFilter Where(Func<Type, bool> predicate)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            Types = Types.Where(predicate);
            return this;
        }
    }
}
