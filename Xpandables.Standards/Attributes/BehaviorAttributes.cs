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

using System.ComponentModel.DataAnnotations;
using System.Design;
using System.Design.Database;

namespace System
{
    /// <summary>
    /// A marker interface that allows the class implementation to be decorated with the validation behavior according to the class type : 
    /// <see cref="IQuery{TResult}"/> class implementation will be decorated with <see cref="QueryValidationBehavior{TQuery, TResult}"/> while 
    /// <see cref="ICommand"/> class implementation will be decorated with <see cref="CommandValidationBehavior{TCommand}"/>.
    /// The default validation behavior uses the data annotations validator on validation attributes. You can implement the interface 
    /// <see cref="IValidationRule{T}"/> or derive a class from <see cref="ValidationRule{TArgument}"/> to customize a validation behavior.
    /// You need to register the expected behavior to the service collections using the appropriate extension method 
    /// <see langword="AddXValidationBehavior"/> for the validation behavior and <see langword="AddXValidationRules"/> to register all your custom implementations..
    /// </summary>
    public interface IValidationBehavior { }

    /// <summary>
    /// A marker interface that allows the handling class implementation to be decorated with transaction behavior according to the decorated class type :
    /// <see cref="IQuery{TResult}"/> class implementation will be decorated with <see cref="QueryTransactionBehavior{TQuery, TResult}"/> while
    /// <see cref="ICommand"/> class implementation will be decorated with <see cref="CommandTransactionBehavior{TCommand}"/>. The class should be
    /// decorated with the <see cref="SupportTransactionAttribute"/> to customize the transaction behavior, condition without which the behavior will not be
    /// effective. You need to register the expected behavior to the service collections using the appropriate extension method :
    /// <see langword="AddXTransactionBehavior"/>.
    /// </summary>
    public interface ITransactionBehavior { }

    /// <summary>
    /// A marker interface that allows the handling class implementation to use persistence data across the control flow. The behavior makes use of an implementation
    /// of <see cref="IDataContext"/> in the handling class (EFCore DataContext) to persist data at the end of the control flow only if there is no exception.
    /// In order to control the behavior, you can add the <see cref="ICorrelationBehavior"/> to the class handling the query or command and reference the
    /// <see cref="ICorrelationTask"/> to defines actions to be applied after the control flow with <see cref="ICorrelationTask.PostEvent"/> on success and
    /// <see cref="ICorrelationTask.RollbackEvent"/> on exception.
    /// You need to register the expected behavior to the service collections using the appropriate extension method
    /// <see langword="AddXPersistenceBehavior"/>, register the data context using the <see langword="AddXDataContext"/> extension methods.
    /// <see cref="IQuery{TResult}"/> class implementation will be decorated with <see cref="QueryPersistenceBehavior{TQuery, TResult}"/> while
    /// <see cref="ICommand"/> class implementation will be decorated with <see cref="CommandPersistenceBehavior{TCommand}"/>.
    /// </summary>
    public interface IPersistenceBehavior { }

    /// <summary>
    /// A marker interface that allows the class implementation to add event after control flow. In the class handling the query or command, you should reference
    /// the <see cref="ICorrelationTask"/> and set the <see cref="ICorrelationTask.PostEvent"/> and/or <see cref="ICorrelationTask.RollbackEvent"/>.
    /// Note that <see cref="ICorrelationTask.PostEvent"/> will be executed at the end of the control only if there is no exception, giving you access to all data
    /// still alive on the control flow and the <see cref="ICorrelationTask.RollbackEvent"/> will only be executed when exception. The exception in that case in
    /// accessible through the <see cref="ICorrelationTask.RollbackEvent"/>. You need to register the expected behavior using the appropriate extension method :
    /// <see langword="AddXCorrelationBehavior"/>.
    /// <see cref="IQuery{TResult}"/> class implementation will be decorated with <see cref="QueryCorrelationBehavior{TQuery, TResult}"/> while
    /// <see cref="ICommand"/> class implementation will be decorated with <see cref="CommandCorrelationBehavior{TCommand}"/>.
    /// </summary>
    public interface ICorrelationBehavior { }

    /// <summary>
    /// A marker interface that allows the class implementation to be seeded before use. This only works for the <see cref="IDataContext"/> class
    /// implementation. You need to register the expected behavior using the appropriate extension method : <see langword="AddXDataContextSeederBehavior"/>
    /// and provide an implementation for <see cref="IDataContextSeeder"/>. The class implementation will be decorated with the 
    /// <see cref="DataContextSeederBehavior"/>.
    /// </summary>
    public interface ISeederBehavior { }
}
