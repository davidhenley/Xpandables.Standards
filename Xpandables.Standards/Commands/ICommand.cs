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

namespace System.Design
{
    /// <summary>
    /// This interface is used as a marker for commands when using the command pattern.
    /// Class implementation is used with the <see cref="ICommandHandler{TCommand}"/> where "TCommand" is <see cref="ICommand"/> class implementation.
    /// This can also be enhanced with some useful interface decorators such as <see cref="IPersistenceBehavior"/> or <see cref="IValidationBehavior"/>.
    /// </summary>
    [Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1040:Éviter les interfaces vides", Justification = "<En attente>")]
    public interface ICommand { }
}