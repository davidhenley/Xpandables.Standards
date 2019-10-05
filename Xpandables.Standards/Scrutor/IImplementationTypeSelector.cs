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

namespace System.Design.DependencyInjection
{
    public interface IImplementationTypeSelector : IAssemblySelector
    {
        /// <summary>
        /// Adds all public, non-abstract classes from the selected assemblies to the <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>.
        /// </summary>
        IServiceTypeSelector AddClasses();

        /// <summary>
        /// Adds all non-abstract classes from the selected assemblies to the <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>.
        /// </summary>
        /// <param name="publicOnly">Specifies whether too add public types only.</param>
        IServiceTypeSelector AddClasses(bool publicOnly);

        /// <summary>
        /// Adds all public, non-abstract classes from the selected assemblies that
        /// matches the requirements specified in the <paramref name="action"/>
        /// to the <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>.
        /// </summary>
        /// <param name="action">The filtering action.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="action"/> argument is <c>null</c>.</exception>
        IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action);

        /// <summary>
        /// Adds all non-abstract classes from the selected assemblies that
        /// matches the requirements specified in the <paramref name="action"/>
        /// to the <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>.
        /// </summary>
        /// <param name="action">The filtering action.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="action"/> argument is <c>null</c>.</exception>
        /// <param name="publicOnly">Specifies whether too add public types only.</param>
        IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action, bool publicOnly);
    }
}
