﻿// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Internals
{
    using System;
    using System.Diagnostics;

    // This class replaces the .NET's default Lazy<T> implementation. The behavior of the
    // ExecutionAndPublication mode of the default implementation is unsuited for use inside Simple Injector
    // because it will ensure that the factory is called just once, which means it caches any thrown exception.
    // That behavior is problematic because it can cause corruption of Simple Injector; for instance when, a
    // ThreadAbortException is thrown during the execution of the factory. See: #731.
    // This implementation behaves different to Lazy<T> in the following respects:
    // * It only supports reference types
    // * It does not allow the Value to be null.
    // * It only supports a single mode, which is a mix between ExecutionAndPublication and PublicationOnly.
    //   This mode has the following behavior:
    //   * It ensures the call to the factory is synchronized (like ExecutionAndPublication)
    //   * It ensures only one successful call to factory is made (like ExecutionAndPublication)
    //   * It will not cache any thrown exception and allow the factory to be called again (like PublicationOnly)
    // Simple Injector internally used Lazy<T>, especially with the ExecutionAndPublication mode, to ensure
    // that Singletons are guaranteed to be created just once. Replacing Lazy<T> will not per se cause this
    // guarantee to be broken, but now the underlying construct needs to make care that the guarantee isn't
    // broken. In the majority of cases, however, this loosening of constraints is perfectly fine.
    [DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={ValueForDebugDisplay}")]
    internal sealed class LazyEx<T> where T : class
    {
        private object valueOrFactory;

        public LazyEx(Func<T> valueFactory)
        {
            Requires.IsNotNull(valueFactory, nameof(valueFactory));

            valueOrFactory = valueFactory;
        }

        public LazyEx(T value)
        {
            Requires.IsNotNull(value, nameof(value));

            valueOrFactory = value;
        }

        public bool IsValueCreated => valueOrFactory is T;

        public T Value
        {
            get
            {
                T? value = valueOrFactory as T;

                if (value is null)
                {
                    // NOTE: Locking on 'this' is typically not adviced, but this type is internal, which
                    // means the risk is minimal. Locking on 'this' allows us to safe some bytes for the extra
                    // lock object.
                    // OPTIMIZATION: Because this is a very common code path, and very regularly part of a
                    // user's stack trace, this code is inlined here to make the call stack shorter and more
                    // readable (for user's and maintainers).
                    lock (this)
                    {
                        value = valueOrFactory as T;

                        if (value is null)
                        {
                            var factory = (Func<T>)valueOrFactory;

                            value = factory.Invoke();

                            if (value is null)
                            {
                                throw new InvalidOperationException("The valueFactory produced null.");
                            }

                            valueOrFactory = value;
                        }
                    }
                }

                return value;
            }
        }

        internal T? ValueForDebugDisplay => valueOrFactory as T;

        public override string ToString() =>
            !IsValueCreated ? "Value is not created." : Value.ToString();
    }
}