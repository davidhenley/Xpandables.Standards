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

namespace System.Design.Database
{
    /// <summary>
    /// This class allows us to add seed support to data context. The data context need to implement the <see cref="ISeederBehavior"/> interface.
    /// <para>This decorator will always call the <see cref="IDataContextSeeder"/> implementation to seed database.</para>
    /// </summary>
    public sealed class DataContextSeederBehavior : IDataContextAccessor, ISeederBehavior
    {
        private readonly IDataContextAccessor _decoratee;
        private readonly IDataContextSeeder _seeder;

        public DataContextSeederBehavior(
            IDataContextAccessor decoratedDataContextProducer,
            IDataContextSeeder dataContextSeeder)
        {
            _decoratee = decoratedDataContextProducer
                ?? throw new ArgumentNullException(
                    nameof(decoratedDataContextProducer),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(DataContextSeederBehavior),
                    nameof(decoratedDataContextProducer)));

            _seeder = dataContextSeeder
                ?? throw new ArgumentNullException(
                    nameof(dataContextSeeder),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(DataContextSeederBehavior),
                    nameof(dataContextSeeder)));
        }

        Optional<IDataContext> IDataContextAccessor.GetDataContext()
        {
            var context = _decoratee.GetDataContext();

            context.GetType().GetInterface(nameof(ISeederBehavior))
                .AsOptional()
                .MapOptional(_ => context)
                .Map(ctxt => _seeder.Seed(ctxt))
                .Map(ctxt => ctxt.Persist());

            return context;
        }
    }
}