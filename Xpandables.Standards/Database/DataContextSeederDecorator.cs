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

namespace System.Data
{
    /// <summary>
    /// This class allows us to add seed support to data context.
    /// <para>This decorator will always call the <see cref="IDataContextSeeder"/> implementation to seed database.</para>
    /// </summary>
    public sealed class DataContextSeederDecorator : IDataContextProvider
    {
        private readonly IDataContextProvider _decoratedDataContextProvider;
        private readonly IDataContextSeeder _dataContextSeeder;

        public DataContextSeederDecorator(IDataContextProvider decoratedDataContextProducer, IDataContextSeeder dataContextSeeder)
        {
            _decoratedDataContextProvider = decoratedDataContextProducer ?? throw new ArgumentNullException(nameof(decoratedDataContextProducer));
            _dataContextSeeder = dataContextSeeder ?? throw new ArgumentNullException(nameof(dataContextSeeder));
        }

        IDataContext IDataContextProvider.GetDataContext()
        {
            var dataContext = _decoratedDataContextProvider.GetDataContext();

            _dataContextSeeder.Seed(dataContext);

            dataContext.Persist();

            return dataContext;
        }
    }
}