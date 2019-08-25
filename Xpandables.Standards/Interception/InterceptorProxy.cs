﻿/************************************************************************************************************
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

using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace System.Interception
{
    /// <summary>
    /// The base implementation for interceptor.
    /// This implementation uses the <see cref="DispatchProxy" /> process to apply customer behaviors to a method.
    /// </summary>
    /// <typeparam name="TInstance">Type of instance to be intercepted.</typeparam>
    public class InterceptorProxy<TInstance> : DispatchProxy
        where TInstance : class
    {
        private static readonly MethodBase MethodBaseType = typeof(object).GetMethod("GetType");
        private TInstance _realInstance;
        private IInterceptor _interceptor;

#pragma warning disable CA1000 // Ne pas déclarer de membres comme étant static sur les types génériques
        /// <summary>
        /// Returns a new instance of <typeparamref name="TInstance"/> wrapped by a proxy.
        /// </summary>
        /// <param name="instance">the instance to be wrapped.</param>
        /// <param name="interceptor">The instance of the interceptor.</param>
        /// <returns>An instance that has been wrapped by a proxy.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="interceptor"/> is null.</exception>
        public static TInstance Create(TInstance instance, IInterceptor interceptor)
#pragma warning restore CA1000 // Ne pas déclarer de membres comme étant static sur les types génériques
        {
            object proxy = Create<TInstance, InterceptorProxy<TInstance>>();
            ((InterceptorProxy<TInstance>)proxy).SetParameters(instance, interceptor);
            return (TInstance)proxy;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="InterceptorProxy{TInstance}"/> with default values.
        /// </summary>
        public InterceptorProxy()
        {
            _realInstance = default!;
            _interceptor = default!;
        }

        /// <summary>
        /// Initializes the decorated instance and the interceptor with the provided arguments.
        /// </summary>
        /// <param name="instance">The instance to be intercepted.</param>
        /// <param name="interceptor">The instance of interceptor.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="interceptor"/> is null.</exception>
        private void SetParameters(TInstance instance, IInterceptor interceptor)
        {
            _realInstance = instance ?? throw new ArgumentNullException(nameof(instance));
            _interceptor = interceptor ?? throw new ArgumentNullException(nameof(interceptor));
        }

        /// <inheritdoc />
        /// <summary>
        /// Executes the method specified in the <paramref name="targetMethod" />.
        /// Applies the interceptor behavior to the called method.
        /// </summary>
        /// <param name="targetMethod">The target method.</param>
        /// <param name="args">The expected arguments.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="targetMethod" /> is null.</exception>
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (targetMethod is null)
                throw new ArgumentNullException($"The parameter {nameof(targetMethod)} is missing.");

            return ReferenceEquals(targetMethod, MethodBaseType)
                ? Bypass(targetMethod, args)
                : DoInvoke(targetMethod, args);
        }

        private object DoInvoke(MethodInfo method, params object[] args)
        {
            var invocation = new Invocation(method, _realInstance, args);

            if (_interceptor.WillExecute)
                try
                {
                    _interceptor.Intercept(invocation);
                }
#pragma warning disable CA1031 // Ne pas intercepter les types d'exception générale
                catch (Exception exception)
#pragma warning restore CA1031 // Ne pas intercepter les types d'exception générale
                {
                    invocation.WithException(
                        new InvalidOperationException(
                            $"The interceptor {_interceptor.GetType().Name} throws an exception.",
                            exception));
                }
            else
                invocation.Proceed();

            if (invocation.Exception.Any())
                ExceptionDispatchInfo.Capture(invocation.Exception).Throw();

            return invocation.ReturnValue.Cast<object>();
        }

        /// <summary>
        /// Bypass the interceptor application because the method is a system method (GetType).
        /// </summary>
        /// <param name="targetMethod">Contains all information about the method being executed</param>
        /// <param name="args">Arguments to be used.</param>
        /// <returns><see cref="object"/> instance</returns>
        [DebuggerStepThrough]
        private object Bypass(MethodInfo targetMethod, object[] args) => targetMethod.Invoke(_realInstance, args);
    }
}