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

namespace System
{
    /// <summary>
    /// A marker interface to allow validation decorator.
    /// </summary>
    public interface IValidationDecorator { }

    /// <summary>
    /// A marker interface to allow transaction decorator.
    /// </summary>
    public interface ITransactionDecorator { }

    /// <summary>
    /// A marker interface to allow persistence decorator.
    /// </summary>
    public interface IPersistenceDecorator { }

    /// <summary>
    /// A marker interface to allow logging decorator.
    /// </summary>
    public interface ILoggingDecorator { }

    /// <summary>
    /// A marker interface to allow event register decorator.
    /// </summary>
    public interface IEventRegisterDecorator { }
}