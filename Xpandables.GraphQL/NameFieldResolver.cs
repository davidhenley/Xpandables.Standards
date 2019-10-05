/************************************************************************************************************
 * Copyright (C) 2018 Francis-Black EWANE
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

using GraphQL.Resolvers;
using GraphQL.Types;

namespace System.GraphQL
{
    /// <summary>
    /// Resolves name from context.
    /// </summary>
    public sealed class NameFieldResolver : IFieldResolver
    {
        public object? Resolve(ResolveFieldContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.Source is null)
                return default;

            var source = context.Source;
            var name = char.ToUpperInvariant(context.FieldAst.Name[0]) + context.FieldAst.Name.Substring(1);
            var value = GetPropertyValue(source, name);

            if (value is null)
            {
                throw new InvalidOperationException(
                   $"Expected to find property {context.FieldAst.Name} on {context.Source.GetType().Name} but it does not exist.");
            }

            return value;
        }

        private static object? GetPropertyValue(object source, string propertyName)
            => source.GetType().GetProperty(propertyName)?.GetValue(source, null);
    }
}