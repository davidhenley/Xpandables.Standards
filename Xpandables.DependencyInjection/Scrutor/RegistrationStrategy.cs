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
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace System.Design.DependencyInjection
{
    public abstract class RegistrationStrategy
    {
        /// <summary>
        /// Skips registrations for services that already exists.
        /// </summary>
        public static readonly RegistrationStrategy Skip = new SkipRegistrationStrategy();

        /// <summary>
        /// Appends a new registration for existing sevices.
        /// </summary>
        public static readonly RegistrationStrategy Append = new AppendRegistrationStrategy();

        /// <summary>
        /// Replaces existing service registrations using <see cref="ReplacementBehaviors.Default"/>.
        /// </summary>
        public static RegistrationStrategy Replace()
        {
            return Replace(ReplacementBehaviors.Default);
        }

        /// <summary>
        /// Replaces existing service registrations based on the specified <see cref="ReplacementBehaviors"/>.
        /// </summary>
        /// <param name="behavior">The behavior to use when replacing services.</param>
        public static RegistrationStrategy Replace(ReplacementBehaviors behavior)
        {
            return new ReplaceRegistrationStrategy(behavior);
        }

        /// <summary>
        /// Applies the the <see cref="ServiceDescriptor"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="descriptor">The descriptor to apply.</param>
        public abstract void Apply(IServiceCollection services, ServiceDescriptor descriptor);

        private sealed class SkipRegistrationStrategy : RegistrationStrategy
        {
            public override void Apply(IServiceCollection services, ServiceDescriptor descriptor) => services.TryAdd(descriptor);
        }

        private sealed class AppendRegistrationStrategy : RegistrationStrategy
        {
            public override void Apply(IServiceCollection services, ServiceDescriptor descriptor) => services.Add(descriptor);
        }

        private sealed class ReplaceRegistrationStrategy : RegistrationStrategy
        {
            public ReplaceRegistrationStrategy(ReplacementBehaviors behavior)
            {
                Behavior = behavior;
            }

            private ReplacementBehaviors Behavior { get; }

            public override void Apply(IServiceCollection services, ServiceDescriptor descriptor)
            {
                var behavior = Behavior;

                if (behavior == ReplacementBehaviors.Default)
                {
                    behavior = ReplacementBehaviors.ServiceType;
                }

                if (behavior.HasFlag(ReplacementBehaviors.ServiceType))
                {
                    for (var i = services.Count - 1; i >= 0; i--)
                    {
                        if (services[i].ServiceType == descriptor.ServiceType)
                        {
                            services.RemoveAt(i);
                        }
                    }
                }

                if (behavior.HasFlag(ReplacementBehaviors.ImplementationType))
                {
                    for (var i = services.Count - 1; i >= 0; i--)
                    {
                        if (services[i].ImplementationType == descriptor.ImplementationType)
                        {
                            services.RemoveAt(i);
                        }
                    }
                }

                services.Add(descriptor);
            }
        }
    }
}
