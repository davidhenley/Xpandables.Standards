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

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace System.Design.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ServiceDescriptorAttribute : Attribute
    {
        public ServiceDescriptorAttribute() : this(null) { }

        public ServiceDescriptorAttribute(Type? serviceType) : this(serviceType, ServiceLifetime.Transient) { }

        public ServiceDescriptorAttribute(Type? serviceType, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
        }

        public Type? ServiceType { get; }

        public ServiceLifetime Lifetime { get; }

        public IEnumerable<Type> GetServiceTypes(Type fallbackType)
        {
            if (ServiceType is null)
            {
                yield return fallbackType;

                var fallbackTypes = fallbackType.GetBaseTypes();

                foreach (var type in fallbackTypes)
                {
                    if (type == typeof(object))
                    {
                        continue;
                    }

                    yield return type;
                }

                yield break;
            }

            if (!fallbackType.IsAssignableTo(ServiceType))
            {
                throw new InvalidOperationException($@"Type ""{fallbackType.ToFriendlyName()}"" is not assignable to ""{ServiceType.ToFriendlyName()}"".");
            }

            yield return ServiceType;
        }
    }
}
