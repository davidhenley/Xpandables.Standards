using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Xpandables.Database;

namespace Xpandables.Tests
{
    [TestClass]
    public class UnitTest1
    {
        const string connection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Xpandables;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        [TestMethod]
        public void Testmethod2()
        {
            var dbProviderFactoryProvider = new DataProviderFactoryProvider();
            DbProviderFactory dbProviderfactory = dbProviderFactoryProvider.GetProviderFactory(DataProviderType.MSSQL);

            using var dbConnection = CreateDbConnection(dbProviderfactory, connection);
            var command = dbConnection.CreateCommand();
            command.CommandText = "SELECT p.FirstName AS P_FName, p.LastName AS P_LName, p.BirthDate as P_BirthDate, addr.* FROM Person AS p INNER JOIN Address AS addr ON p.ID  = addr.PersonID";
            command.CommandType = CommandType.Text;

            var adapter = dbProviderfactory.CreateDataAdapter();
            adapter.SelectCommand = command;

            var dataset = new DataSet();
            adapter.Fill(dataset);
            var table = dataset.Tables[0];

            IDataMapperEntityBuilder propertyBuilder = new DataMapperEntityBuilder();
            var properties = propertyBuilder.Build<Person>();
            IDataMapperRow dataMapper = new DataMapperRow(propertyBuilder);

            var persons = new ConcurrentDictionary<string, IDataMapperEntity<Person>>();

            foreach (DataRow dataRow in table.Rows)
            {
                var person = dataMapper.MapTo<Person>(dataRow);
                person.Map(p => persons.AddOrUpdate(p.Signature, p, (_, __) => p));
            }

            var result = persons.Values.Select(p => p.Entity).ToList();
            Assert.IsTrue(result.Count > 3);
        }

        [TestMethod]
        public void TestMethod1()
        {
            var dbProviderFactoryProvider = new DataProviderFactoryProvider();
            DbProviderFactory dbProviderfactory = dbProviderFactoryProvider.GetProviderFactory(DataProviderType.MSSQL);
            using var dbConnection = CreateDbConnection(dbProviderfactory, connection);

            dbConnection.Open();
            var command = dbConnection.CreateCommand();
            command.CommandText = "SELECT p.FirstName AS FName, p.LastName AS LName, addr.* FROM Person AS p INNER JOIN Address AS addr ON p.ID  = addr.PersonID";
            command.CommandType = CommandType.Text;

            IDataMapperEntityBuilder propertyBuilder = new DataMapperEntityBuilder();
            var properties = propertyBuilder.Build<Person>();
            var person = new Person();

            var result = command.ExecuteReader(CommandBehavior.Default | CommandBehavior.KeyInfo);
            var count = 0;
            var table = result.GetSchemaTable().Rows.Cast<DataRow>().ToList();
            while (result.Read())
            {
                count++;
                var dataRecord = result.Cast<DbDataRecord>().ToList();
            }

            var countLast = count;
            //SqlClientFactory.Instance.CreateParameter().DbType=DbType.
            // Retrieve the installed providers and factories.
            //DataTable table = DbProviderFactories.GetFactoryClasses();

            //// Display each row and column value.
            //foreach (DataRow row in table.Rows)
            //{
            //    foreach (DataColumn column in table.Columns)
            //    {
            //        Debug.WriteLine(row[column]);
            //    }
            //}
        }

        private DbConnection CreateDbConnection(DbProviderFactory dbProviderFactory, string connectionString)
        {
            var dbConnection = dbProviderFactory.CreateConnection();
            dbConnection.ConnectionString = connectionString;
            return dbConnection;
        }
    }

    [DataMapperPrefix("P_")]
    [DataMapperUniqueKey(nameof(FirstName), nameof(LastName))]
    public class Person
    {
        public DateTime BirthDate { get; set; }
        [DataMapperName("FName")]
        public string FirstName { get; set; }
        [DataMapperName("LName")]
        public string LastName { get; set; }
        public List<Address> Addresses { get; set; }
    }

    //[DataMapperPrefix("A_")]
    public class Address
    {
        public string Street { get; set; }
        //[DataMapperPrefix("P_")]
        public string City { get; set; }
        [DataMapperName("Zip")]
        public string PostalCode { get; set; }
    }
}
