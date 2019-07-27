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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace System.Interception
{
    /// <summary>
    /// Provides the implementation of the <see cref="IInvocation" /> interface.
    /// </summary>
    internal sealed class Invocation : IInvocation
    {
        private readonly MethodInfo _method;
        private readonly object _instance;

        /// <summary>
        /// Initializes a new instance of <see cref="Invocation"/> with the arguments needed for invocation.
        /// </summary>
        /// <param name="targetMethod">The target method.</param>
        /// <param name="targetInstance">The target instance being called.</param>
        /// <param name="argsValue">Arguments for the method, if necessary.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetMethod"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetInstance"/> is null.</exception>
        internal Invocation(MethodInfo targetMethod, object targetInstance, params object[] argsValue)
        {
            _method = targetMethod ?? throw new ArgumentNullException(nameof(targetMethod));
            _instance = targetInstance ?? throw new ArgumentNullException(nameof(targetInstance));

            ReturnType = _method.ReturnType;
            Arguments = GetParametersFromMethod(_method, argsValue).ToArray();
        }

        public IEnumerable<Parameter> Arguments { get; }
        public object ReturnValue { get; private set; }
        public Type ReturnType { get; }
        public Exception Exception { get; private set; }
        public long ElapsedTime { get; private set; }

        public IInvocation WithException(Exception exception)
        {
            Exception = exception;
            return this;
        }

        public IInvocation WithReturnValue(object returnValue)
        {
            ReturnValue = returnValue;
            return this;
        }

        public void Proceed() => Proceeding();

        private Action Proceeding => () =>
                    {
                        var watch = Stopwatch.StartNew();

                        try
                        {
                            ReturnValue = _method.Invoke(
                                   _instance,
                                   Arguments.Select(arg => arg.Value).ToArray());

                            if (ReturnValue is Task task && task.Exception != null)
                                Exception = task.Exception.GetBaseException();
                        }
                        catch (Exception exception)
                        {
                            Exception = exception;
                        }
                        finally
                        {
                            watch.Stop();
                            ElapsedTime = watch.ElapsedMilliseconds;
                        }
                    };

        private static IEnumerable<Parameter> GetParametersFromMethod(MethodInfo method, params object[] argsValue)
        {
            foreach (var param in method
                .GetParameters()
                .Select((value, index) => new { Index = index, Value = value })
                .OrderBy(o => o.Value.Position).ToArray())
                yield return Parameter.BuildWith(param.Index, param.Value, argsValue[param.Index]);

        }
    }
}