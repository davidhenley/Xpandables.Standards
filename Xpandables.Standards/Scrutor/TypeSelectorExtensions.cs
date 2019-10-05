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
    public static class TypeSelectorExtensions
    {
        public static IServiceTypeSelector AddType<T>(this ITypeSelector selector)
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));

            return selector.AddTypes(typeof(T));
        }

        public static IServiceTypeSelector AddTypes<T1, T2>(this ITypeSelector selector)
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));

            return selector.AddTypes(typeof(T1), typeof(T2));
        }

        public static IServiceTypeSelector AddTypes<T1, T2, T3>(this ITypeSelector selector)
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));

            return selector.AddTypes(typeof(T1), typeof(T2), typeof(T3));
        }
    }
}
