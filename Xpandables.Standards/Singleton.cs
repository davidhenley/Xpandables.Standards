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

using System.Reflection;

namespace System
{
    /// <summary>
    /// Applies singleton pattern to the derived class.
    /// </summary>
    /// <typeparam name="T">Type of the derived class.</typeparam>
    public abstract class Singleton<T>
        where T : class
    {
        static readonly object syncRoot = new object();
        static T instance = null;

        /// <summary>
        /// Returns the unique instance for the type.
        /// <para>Throws <see cref="InvalidOperationException"/> if the type does not contain a
        /// private parameterless constructor.</para>
        /// </summary>
        public static T GetInstance()
        {
            if (instance is null)
            {
                lock (syncRoot)
                {
                    if (instance is null)
                    {
                        if (typeof(T).GetConstructor(
                                BindingFlags.NonPublic | BindingFlags.Instance,
                                null, Type.EmptyTypes, null) is ConstructorInfo constructorInfo)
                        {
                            instance = BuildInstanceWithConstructor(constructorInfo);
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                $"Building the singleton instance for type '{typeof(T).Name}' failed.",
                                new TargetParameterCountException($"{typeof(T).Name} must contains a private parameterless constructor."));
                        }

                    }
                }
            }
            return instance;
        }

        private static T BuildInstanceWithConstructor(ConstructorInfo constructorInfo)
        {
            try
            {
                return (T)constructorInfo.Invoke(null);
            }
            catch (Exception exception) when (exception is MemberAccessException || exception is MethodAccessException
                                                || exception is ArgumentException || exception is TargetInvocationException
                                                || exception is TargetParameterCountException || exception is NotSupportedException
                                                || exception is Security.SecurityException)
            {
                throw new InvalidOperationException(
                    $"Building the singleton instance for type '{typeof(T).Name}' failed. See inner exception.",
                    exception);
            }
        }
    }
}
