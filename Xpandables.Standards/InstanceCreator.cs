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
        public ExecutionResult<object> Create(Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));

            try
            {
                var lambdaConstructor = GetLambdaConstructor<Func<object>>(type, Array.Empty<Type>());
                return new ExecutionResult<object>(lambdaConstructor.Invoke());
            }
            catch (Exception exception)
            {
                return new ExecutionResult<object>(exception);
            }
        }

        public ExecutionResult<object> Create<TParam>(Type type, TParam param)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (param is null) throw new ArgumentNullException(nameof(param));

            try
            {
                var lambdaConstructor = GetLambdaConstructor<Func<TParam, object>>(type, new[] { typeof(TParam) });
                return new ExecutionResult<object>(lambdaConstructor.Invoke(param));
            }
            catch (Exception exception)
            {
                return new ExecutionResult<object>(exception);
            }
        }

        public ExecutionResult<object> Create<TParam1, TParam2>(Type type, TParam1 param1, TParam2 param2)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (param1 is null) throw new ArgumentNullException(nameof(param1));
            if (param2 is null) throw new ArgumentNullException(nameof(param2));

            try
            {
                var lambdaConstructor = GetLambdaConstructor<Func<TParam1, TParam2, object>>(
                    type,
                    new[] { typeof(TParam1), typeof(TParam2) });

                return new ExecutionResult<object>(lambdaConstructor.Invoke(param1, param2));
            }
            catch (Exception exception)
            {
                return new ExecutionResult<object>(exception);
            }
        }

        public ExecutionResult<object> Create<TParam1, TParam2, TParam3>(Type type, TParam1 param1, TParam2 param2, TParam3 param3)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (param1 is null) throw new ArgumentNullException(nameof(param1));
            if (param2 is null) throw new ArgumentNullException(nameof(param2));
            if (param3 is null) throw new ArgumentNullException(nameof(param3));

            try
            {
                var lambdaConstructor = GetLambdaConstructor<Func<TParam1, TParam2, TParam3, object>>(
                    type,
                    new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3) });

                return new ExecutionResult<object>(lambdaConstructor.Invoke(param1, param2, param3));
            }
            catch (Exception exception)
            {
                return new ExecutionResult<object>(exception);
            }
        }

        protected virtual TDelegate GetLambdaConstructor<TDelegate>([NotNull] Type type, params Type[] parameterTypes)
            where TDelegate : Delegate
        {
            if (type is null) throw new ArgumentNullException(nameof(type));

            var constructor = GetConstructorInfo(type, parameterTypes);
            var parameterExpression = GetParameterExpression(parameterTypes);
            var constructorExpression = GetConstructorExpression(constructor, parameterExpression);

            return Expression
                .Lambda<TDelegate>(constructorExpression, parameterExpression)
                .Compile();
        }

        // Get the Constructor which matches the given argument Types.
        static ConstructorInfo GetConstructorInfo([NotNull] Type type, params Type[] parameterTypes)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));

            return type.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public,
                null,
                CallingConventions.HasThis,
                parameterTypes,
                Array.Empty<ParameterModifier>());
        }

        // Get a set of Expressions representing the parameters which will be passed to the constructor.
        static ParameterExpression[] GetParameterExpression(params Type[] parameterTypes)
            => parameterTypes
                .Select((type, index) => Expression.Parameter(type, $"param{index + 1}"))
                .ToArray();

        // Get an Expression representing the constructor call, passing in the constructor parameters.
        static Expression GetConstructorExpression(
           [NotNull] ConstructorInfo constructorInfo,
           params ParameterExpression[] parameterExpressions)
           => Expression.New(constructorInfo, parameterExpressions);
    }
}
