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
using System.Linq;
using System.Reflection;

namespace System.Design.DependencyInjection
{
    internal class AttributeSelector : ISelector
    {
        public AttributeSelector(IEnumerable<Type> types)
        {
            Types = types;
        }

        private IEnumerable<Type> Types { get; }

        void ISelector.Populate(IServiceCollection services, RegistrationStrategy registrationStrategy)
        {
            var strategy = registrationStrategy ?? RegistrationStrategy.Append;

            foreach (var type in Types)
            {
                var typeInfo = type.GetTypeInfo();

                var attributes = typeInfo.GetCustomAttributes<ServiceDescriptorAttribute>().ToArray();

                // Check if the type has multiple attributes with same ServiceType.
                var duplicates = GetDuplicates(attributes);

                if (duplicates.Any())
                {
                    throw new InvalidOperationException($@"Type ""{type.ToFriendlyName()}"" has multiple ServiceDescriptor attributes with the same service type.");
                }

                foreach (var attribute in attributes)
                {
                    var serviceTypes = attribute.GetServiceTypes(type);

                    foreach (var serviceType in serviceTypes)
                    {
                        var descriptor = new ServiceDescriptor(serviceType, type, attribute.Lifetime);

                        strategy.Apply(services, descriptor);
                    }
                }
            }
        }

        private static IEnumerable<ServiceDescriptorAttribute> GetDuplicates(IEnumerable<ServiceDescriptorAttribute> attributes)
        {
            return attributes.GroupBy(s => s.ServiceType).SelectMany(grp => grp.Skip(1));
        }
    }
}
