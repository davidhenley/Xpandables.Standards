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

using GraphQL;
using System.ComponentModel;

namespace System.GraphQL
{
    public static class GraphQLHelpers
    {
        public static Type GetGraphTypeFromTypeUsingConverter(this Type type, bool isNullable = false)
        {
            try
            {
                return type.GetGraphTypeFromType(isNullable);
            }
            catch (ArgumentOutOfRangeException)
            {
                if (TypeDescriptor.GetConverter(type) is TypeConverter converter
                    && converter.CanConvertFrom(typeof(string))
                    && converter.CanConvertTo(typeof(string)))
                {
                    return typeof(string).GetGraphTypeFromType(isNullable);
                }

                throw;
            }
        }
    }
}