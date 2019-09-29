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

namespace System.Design.Database.Common
{
    /// <summary>
    /// Provides with a list of data providers.
    /// You can derive from this class in order to extend type.
    /// </summary>
    public class DataProviderType : EnumerationType
    {
        /// <summary>
        /// Construct a new data provider type with an index and the invariant name.
        /// </summary>
        /// <param name="index">The index for the date provider.</param>
        /// <param name="assemblyName">The invariant assembly name to be used.</param>
        /// <param name="providerFactoryTypeName">The provider factory type name.</param>
        /// <exception cref="ArgumentException">The <paramref name="assemblyName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="providerFactoryTypeName"/> is null.</exception>
        protected DataProviderType(int index, string assemblyName, string providerFactoryTypeName)
            : base(assemblyName, index)
        {
            DisplayName = assemblyName;
            ProviderFactoryTypeName = providerFactoryTypeName ?? throw new ArgumentNullException(nameof(providerFactoryTypeName));
        }

        /// <summary>
        /// Gets the invariant (assembly) name that can be used programmatically to refer to the data provider.
        /// </summary>
        public new string DisplayName { get; }

        /// <summary>
        /// Gets the provider factory type name.
        /// </summary>
        public string ProviderFactoryTypeName { get; }

        /// <summary>
        /// Determines whether or not the provider type refers to an instance factory.
        /// </summary>
        public bool IsInstance => DisplayName.Contains("Instance", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Provides data access for Microsoft SQL Server.
        /// </summary>
        public static DataProviderType MSSQL => new DataProviderType(
            0, "System.Data.SqlClient", "System.Data.SqlClient.SqlClientFactory");

        /// <summary>
        /// For Oracle data sources version 8.1.7 and later.
        /// </summary>
        public static DataProviderType ORACLE => new DataProviderType(
            1, "System.Data.OracleClient", "Oracle.DataAccess.Client.OracleClientFactory");

#if NET48
        /// <summary>
        /// For data sources exposed by using OLE DB.
        /// </summary>
        public static DataProviderType OLEDB => new DataProviderType(2, "System.Data.OleDb", "System.Data.OleDb");
#endif

        /// <summary>
        /// For data sources exposed by using ODBC.
        /// </summary>
        public static DataProviderType ODBC => new DataProviderType(3, "System.Data.Odbc", "System.Data.Odbc");

        /// <summary>
        /// Provides data access for Entity Data Model (EDM) applications.
        /// </summary>
        public static DataProviderType ENTITY => new DataProviderType(4, "System.Data.EntityClient", "System.Data.Entity");

#if NET48
        /// <summary>
        /// Provides data access for Microsoft SQL Lite.
        /// </summary>
        public static DataProviderType SQLITE => new DataProviderType(
            5, "System.Data.SQLite", "System.Data.SQLite.SQLiteFactory");
#else
        /// <summary>
        ///  Provides data access for Microsoft SQL Lite.
        /// </summary>
        public static DataProviderType SQLITE => new DataProviderType(
            5, "Microsoft.Data.Sqlite", "Microsoft.Data.Sqlite.SqliteFactory");
#endif
        /// <summary>
        /// Provides data access for MySQL.
        /// </summary>
        public static DataProviderType MYSQL => new DataProviderType(
            6, "MySql.Data", "MySql.Data.MySqlClient.MySqlClientFactory");

        /// <summary>
        /// Provides data access for PostgreSQL.
        /// </summary>
        public static DataProviderType NPGSQL => new DataProviderType(7, "Npgsql", "Npgsql.NpgsqlFactory");
    }
}
