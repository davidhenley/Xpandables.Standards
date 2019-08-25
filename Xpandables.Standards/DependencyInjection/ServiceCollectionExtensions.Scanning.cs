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

using System;
using System.Design.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds registrations to the <paramref name="services"/> collection using
        /// conventions specified using the <paramref name="action"/>.
        /// </summary>
        /// <param name="services">The services to add to.</param>
        /// <param name="action">The configuration action.</param>
        /// <exception cref="ArgumentNullException">If either the <paramref name="services"/>
        /// or <paramref name="action"/> arguments are <c>null</c>.</exception>
        public static IServiceCollection Scan(this IServiceCollection services, Action<ITypeSourceSelector> action)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (action is null) throw new ArgumentNullException(nameof(action));

            var selector = new TypeSourceSelector();

            action(selector);

            return services.Populate(selector, RegistrationStrategy.Append);
        }

        private static IServiceCollection Populate(this IServiceCollection services, ISelector selector, RegistrationStrategy registrationStrategy)
        {
            selector.Populate(services, registrationStrategy);
            return services;
        }
    }
}
