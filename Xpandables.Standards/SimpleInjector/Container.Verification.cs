﻿// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using SimpleInjector.Diagnostics;
    using SimpleInjector.Internals;

    /// <summary>Methods for verifying the container.</summary>
    public partial class Container
    {
        // Flag to signal that the container's configuration is currently being verified.
        private readonly ThreadLocal<bool> isVerifying = new ThreadLocal<bool>();
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Qualité du code", "IDE0069:Les champs supprimables doivent l'être", Justification = "<En attente>")]
        private readonly ThreadLocal<Scope?> resolveScope = new ThreadLocal<Scope?>();

        private bool usingCurrentThreadResolveScope;

        // Flag to signal that the container's configuration has been verified (at least once).
        internal bool SuccesfullyVerified { get; private set; }

        internal Scope? VerificationScope { get; private set; }

        // Allows to resolve directly from a scope instead of relying on an ambient context.
        internal Scope? CurrentThreadResolveScope
        {
            get
            {
                return usingCurrentThreadResolveScope ? resolveScope.Value : null;
            }

            set
            {
                // PERF: We flag the use of the current-thread-resolve-scope to optimize getting the right
                // scope. Most application's won't resolve directly from the scope, but from the container.
                if (!usingCurrentThreadResolveScope)
                {
                    usingCurrentThreadResolveScope = true;
                }

                resolveScope.Value = value;
            }
        }

        /// <summary>
        /// Verifies and diagnoses this <b>Container</b> instance. This method will call all registered
        /// delegates, iterate registered collections and throws an exception if there was an error.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the registration of instances was
        /// invalid.</exception>
        public void Verify()
        {
            Verify(VerificationOption.VerifyAndDiagnose);
        }

        /// <summary>
        /// Verifies the <b>Container</b>. This method will call all registered delegates,
        /// iterate registered collections and throws an exception if there was an error.
        /// </summary>
        /// <param name="option">Specifies how the container should verify its configuration.</param>
        /// <exception cref="InvalidOperationException">Thrown when the registration of instances was
        /// invalid.</exception>
        /// <exception cref="DiagnosticVerificationException">Thrown in case there are diagnostic errors and
        /// the <see cref="VerificationOption.VerifyAndDiagnose"/> option is supplied.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="option"/> has an invalid value.</exception>
        public void Verify(VerificationOption option)
        {
            Requires.IsValidEnum(option, nameof(option));

            ThrowWhenDisposed();

            bool diagnose = option == VerificationOption.VerifyAndDiagnose;

            VerifyInternal(suppressLifestyleMismatchVerification: diagnose);

            if (diagnose)
            {
                ThrowOnDiagnosticWarnings();
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal Scope? GetVerificationOrResolveScopeForCurrentThread() =>
            VerificationScope != null && IsVerifying
                ? VerificationScope
                : usingCurrentThreadResolveScope
                    ? resolveScope.Value
                    : null;

        internal void UseCurrentThreadResolveScope()
        {
            usingCurrentThreadResolveScope = true;
        }

        private void VerifyInternal(bool suppressLifestyleMismatchVerification)
        {
            // Prevent multiple threads from starting verification at the same time. This could crash, because
            // the first thread could dispose the verification scope, while the other thread is still using it.
            lock (isVerifying)
            {
                LockContainer();
                bool original = Options.SuppressLifestyleMismatchVerification;
                IsVerifying = true;
                VerificationScope = new ContainerVerificationScope(this);

                try
                {
                    // Temporarily suppress lifestyle mismatch verification, because that would cause a single
                    // diagnostic warning to be displayed instead of the complete list of found warnings.
                    if (suppressLifestyleMismatchVerification)
                    {
                        Options.SuppressLifestyleMismatchVerification = true;
                    }

                    Verifying();
                    VerifyThatAllExpressionsCanBeBuilt();
                    VerifyThatAllRootObjectsCanBeCreated(VerificationScope);
                    SuccesfullyVerified = true;
                }
                finally
                {
                    Options.SuppressLifestyleMismatchVerification = original;
                    IsVerifying = false;
                    var scopeToDispose = VerificationScope;
                    VerificationScope = null;
                    scopeToDispose.Dispose();
                }
            }
        }

        private void VerifyThatAllExpressionsCanBeBuilt()
        {
            int maximumNumberOfIterations = 10;

            InstanceProducer[] producersToVerify;

            // The process of building expressions can trigger the creation/registration of new instance
            // producers. Those new producers need to be checked as well. That's why we have a loop here. But
            // since a user could accidentally trigger the creation of new registrations during verify, we
            // must set a sensible limit to the number of iterations, to prevent the process from never
            // stopping.
            do
            {
                maximumNumberOfIterations--;

                producersToVerify = GetCurrentRegistrations(includeInvalidContainerRegisteredTypes: true);

                producersToVerify = (
                    from producer in producersToVerify
                    where !producer.IsExpressionCreated
                    select producer)
                    .ToArray();

                VerifyThatAllExpressionsCanBeBuilt(producersToVerify);
            }
            while (maximumNumberOfIterations > 0 && producersToVerify.Any());
        }

        private void VerifyThatAllRootObjectsCanBeCreated(Scope verificationScope)
        {
            var rootProducers = GetRootRegistrations(includeInvalidContainerRegisteredTypes: true);

            var producersThatMustBeExplicitlyVerified = GetProducersThatNeedExplicitVerification();

            var producersToVerify =
                from producer in rootProducers.Concat(producersThatMustBeExplicitlyVerified).Distinct()
                where !producer.InstanceSuccessfullyCreated || !producer.VerifiersAreSuccessfullyCalled
                select producer;

            VerifyInstanceCreation(producersToVerify.ToArray(), verificationScope);
        }

        private IEnumerable<InstanceProducer> GetProducersThatNeedExplicitVerification()
        {
            var currentRegistrations = GetCurrentRegistrations(includeInvalidContainerRegisteredTypes: true);

            return
                from registration in currentRegistrations
                where registration.MustBeExplicitlyVerified
                select registration;
        }

        private static void VerifyThatAllExpressionsCanBeBuilt(InstanceProducer[] producersToVerify)
        {
            foreach (var producer in producersToVerify)
            {
                var expression = producer.VerifyExpressionBuilding();

                VerifyInstanceProducersOfContainerControlledCollection(expression);
            }
        }

        private static void VerifyInstanceProducersOfContainerControlledCollection(Expression expression)
        {
            var constant = expression as ConstantExpression;

            if (constant?.Value is IContainerControlledCollection collection)
            {
                collection.VerifyCreatingProducers();
            }
        }

        private void VerifyInstanceCreation(InstanceProducer[] producersToVerify, Scope verificationScope)
        {
            foreach (var producer in producersToVerify)
            {
                if (!producer.InstanceSuccessfullyCreated)
                {
                    var instance = producer.VerifyInstanceCreation();

                    VerifyContainerUncontrolledCollection(instance, producer);
                }

                if (!producer.VerifiersAreSuccessfullyCalled)
                {
                    producer.DoExtraVerfication(verificationScope);
                }
            }
        }

        private static void VerifyContainerUncontrolledCollection(object instance, InstanceProducer producer)
        {
            bool isContainerUncontrolledCollection =
                producer.Registration.IsCollection && !(instance is IContainerControlledCollection);

            if (isContainerUncontrolledCollection)
            {
                Type collectionType = producer.ServiceType;
                Type serviceType = collectionType.GetGenericArguments()[0];

                Helpers.VerifyCollection((IEnumerable)instance, serviceType);
            }
        }

        private void ThrowOnDiagnosticWarnings()
        {
            var errors = (
                from result in Analyzer.Analyze(this)
                where result.Severity > DiagnosticSeverity.Information
                select result)
                .ToArray();

            if (errors.Length > 0)
            {
                throw new DiagnosticVerificationException(errors);
            }
        }
    }
}