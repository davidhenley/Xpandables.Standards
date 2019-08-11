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

using System;
using System.Data.Common;
using System.Globalization;
using System.Linq;

namespace Xpandables.Database
{
    public sealed class DataParameterBuilder : IDataParameterBuilder
    {
        public DbCommand Build(DbCommand command, params object[] parameters)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            if (parameters is null || parameters.Length <= 0)
                return command;

            var dbParameters = new DbParameter[parameters.Length];
            if (parameters.All(p => p is DbParameter))
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    dbParameters[i] = (DbParameter)parameters[i];
                }
            }
            else
            if (!parameters.Any(p => p is DbParameter))
            {
                var parameterNames = new string[parameters.Length];
                var parameterSql = new string[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                {
                    parameterNames[i] = string.Format(CultureInfo.InvariantCulture, "p{0}", i);
                    dbParameters[i] = command.CreateParameter();
                    dbParameters[i].ParameterName = parameterNames[i];
                    dbParameters[i].Value = parameters[i] ?? DBNull.Value;

                    parameterSql[i] = "@" + parameterNames[i];
                }

                command.CommandText = string.Format(CultureInfo.InvariantCulture, command.CommandText, parameterSql);
            }
            else
            {
                throw new ArgumentException("Arguments provided can not be a mix of DbParameters and values.");
            }

            command.Parameters.AddRange(dbParameters);

            return command;
        }
    }
}
