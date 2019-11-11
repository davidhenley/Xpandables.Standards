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

using System.Linq;
using System.Reflection;

namespace System.Design.DependencyInjection
{
    [Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1194:Implement exception constructors.", Justification = "<En attente>")]
    [Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<En attente>")]
    public class MissingTypeRegistrationException : InvalidOperationException
    {
        public MissingTypeRegistrationException(Type serviceType)
            : base($"Could not find any registered services for type '{GetFriendlyName(serviceType)}'.")
        {
            ServiceType = serviceType;
        }

        public Type ServiceType { get; }

        private static string GetFriendlyName(Type type)
        {
            if (type == typeof(int)) return "int";
            if (type == typeof(short)) return "short";
            if (type == typeof(byte)) return "byte";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(char)) return "char";
            if (type == typeof(long)) return "long";
            if (type == typeof(float)) return "float";
            if (type == typeof(double)) return "double";
            if (type == typeof(decimal)) return "decimal";
            if (type == typeof(string)) return "string";
            if (type == typeof(object)) return "object";

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType) return GetGenericFriendlyName(typeInfo);

            return type.Name;
        }

        private static string GetGenericFriendlyName(TypeInfo typeInfo)
        {
            var argumentNames = typeInfo.GenericTypeArguments.Select(GetFriendlyName).ToArray();

            var baseName = typeInfo.Name.Split('`').First();

            return $"{baseName}<{string.Join(", ", argumentNames)}>";
        }
    }
}