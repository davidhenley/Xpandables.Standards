/************************************************************************************************************
 * Copyright (C) 2019 Francis-Black EWANE
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System
{
#pragma warning disable CA1031 // Ne pas intercepter les types d'exception générale
    /// <summary>
    /// Implementation of <see cref="IInstanceCreator"/>.
    /// </summary>
    public class InstanceCreator : IInstanceCreator
    {
        public Optional<object> Create(Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));

            try
            {
                var lambdaConstructor = GetLambdaConstructor<Func<object>>(type, Array.Empty<Type>());
                return lambdaConstructor.Invoke();
            }
            catch (Exception exception)
            {
                return Optional<object>.Exception(exception);
            }
        }

        public Optional<object> Create<TParam>(Type type, TParam param)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (EqualityComparer<TParam>.Default.Equals(param, default)) throw new ArgumentNullException(nameof(param));

            try
            {
                var lambdaConstructor = GetLambdaConstructor<Func<TParam, object>>(type, new[] { typeof(TParam) });
                return lambdaConstructor.Invoke(param);
            }
            catch (Exception exception)
            {
                return Optional<object>.Exception(exception);
            }
        }

        public Optional<object> Create<TParam1, TParam2>(Type type, TParam1 param1, TParam2 param2)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (EqualityComparer<TParam1>.Default.Equals(param1, default)) throw new ArgumentNullException(nameof(param1));
            if (EqualityComparer<TParam2>.Default.Equals(param2, default)) throw new ArgumentNullException(nameof(param2));

            try
            {
                var lambdaConstructor = GetLambdaConstructor<Func<TParam1, TParam2, object>>(
                    type,
                    new[] { typeof(TParam1), typeof(TParam2) });

                return lambdaConstructor.Invoke(param1, param2);
            }
            catch (Exception exception)
            {
                return Optional<object>.Exception(exception);
            }
        }

        public Optional<object> Create<TParam1, TParam2, TParam3>(Type type, TParam1 param1, TParam2 param2, TParam3 param3)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (EqualityComparer<TParam1>.Default.Equals(param1, default)) throw new ArgumentNullException(nameof(param1));
            if (EqualityComparer<TParam2>.Default.Equals(param2, default)) throw new ArgumentNullException(nameof(param2));
            if (EqualityComparer<TParam3>.Default.Equals(param3, default)) throw new ArgumentNullException(nameof(param3));

            try
            {
                var lambdaConstructor = GetLambdaConstructor<Func<TParam1, TParam2, TParam3, object>>(
                    type,
                    new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3) });

                return lambdaConstructor.Invoke(param1, param2, param3);
            }
            catch (Exception exception)
            {
                return Optional<object>.Exception(exception);
            }
        }

        protected virtual TDelegate GetLambdaConstructor<TDelegate>(Type type, params Type[] parameterTypes)
            where TDelegate : Delegate
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var constructor = GetConstructorInfo(type, parameterTypes);
            var parameterExpression = GetParameterExpression(parameterTypes);
            var constructorExpression = GetConstructorExpression(constructor, parameterExpression);

            return Expression
                .Lambda<TDelegate>(constructorExpression, parameterExpression)
                .Compile();
        }

        // Get the Constructor which matches the given argument Types.
        private static ConstructorInfo GetConstructorInfo(Type type, params Type[] parameterTypes)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return type.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public,
                null,
                CallingConventions.HasThis,
                parameterTypes,
                Array.Empty<ParameterModifier>());
        }

        // Get a set of Expressions representing the parameters which will be passed to the constructor.
        private static ParameterExpression[] GetParameterExpression(params Type[] parameterTypes)
            => parameterTypes
                .Select((type, index) => Expression.Parameter(type, $"param{index + 1}"))
                .ToArray();

        // Get an Expression representing the constructor call, passing in the constructor parameters.
        private static Expression GetConstructorExpression(
           ConstructorInfo constructorInfo,
           params ParameterExpression[] parameterExpressions)
           => Expression.New(constructorInfo, parameterExpressions);
    }
}
