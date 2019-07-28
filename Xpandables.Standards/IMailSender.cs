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

using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Defines a mail sender.
    /// </summary>
    /// <typeparam name="T">Type of mail content.</typeparam>
    public interface IMailSender<T>
        where T : class
    {
        /// <summary>
        /// Sends the message according to the content.
        /// </summary>
        /// <param name="content">content used for sending message.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="content"/> is null.</exception>
        Task SendAsync(T content);
    }
}