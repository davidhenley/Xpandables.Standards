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

using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Configuration;
using System.Design.Database;
using System.Diagnostics;

namespace System.Design.Logging
{
    /// <summary>
    /// Provides method to register <see cref="Serilog"/> logger.
    /// </summary>
    public static class SerilogServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the logger using the default <see cref="LogEntity"/> event.
        /// The <see cref="LogEntity"/> must be registered in the data context and the context
        /// must implement the <see cref="IAsyncDataContext"/> interface.
        /// </summary>
        /// <param name="services">The current services collection.</param>
        /// <exception cref="InvalidOperationException">Registration failed.</exception>
        public static IServiceCollection AddCustomSerilog(this IServiceCollection services)
            => services.DoAddCustomSerilog<LogEntity>();

        /// <summary>
        /// Adds the logger using the specified type of log event.
        /// The <typeparamref name="TLogEvent"/> must be registered in the data context and the context
        /// must implement the <see cref="IAsyncDataContext"/> interface.
        /// </summary>
        /// <typeparam name="TLogEvent">The type of the log entity.</typeparam>
        /// <param name="services">The current services collection.</param>
        /// <exception cref="InvalidOperationException">Registration failed.</exception>
        public static IServiceCollection AddCustomSerilog<TLogEvent>(this IServiceCollection services)
            where TLogEvent : Entity, ILogEvent<TLogEvent>, new()
            => services.DoAddCustomSerilog<TLogEvent>();

        private static IServiceCollection DoAddCustomSerilog<TLogEvent>(this IServiceCollection services)
            where TLogEvent : Entity, ILogEvent<TLogEvent>, new()
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddScoped(provider => CreateLoggerConfiguration<TLogEvent>(provider));

            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
            return services;
        }

        [DebuggerStepThrough]
        private static ILogger CreateLoggerConfiguration<TLogEvent>(IServiceProvider provider)
            where TLogEvent : Entity, ILogEvent<TLogEvent>, new()
            => new LoggerConfiguration()
                .WriteTo
                .EntityFrameworkSink<TLogEvent>(provider)
                .CreateLogger();

        [DebuggerStepThrough]
        private static LoggerConfiguration EntityFrameworkSink<TLogEvent>(
            this LoggerSinkConfiguration sinkConfiguration,
            IServiceProvider provider)
            where TLogEvent : Entity, ILogEvent<TLogEvent>, new()
            => sinkConfiguration.Sink(provider.GetRequiredService<LogEventSerilogSink<TLogEvent>>());
    }
}
