namespace UnitTests
{
    using DataAccess;
    using DataAccess.Datastore;
    using Microsoft.Data.Sqlite;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    [SetUpFixture]
    public class Setup
    {
        private static ServiceProvider _serviceProvider;

        public static ServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    var configurationContainer = new ServiceCollection();

                    var sqliteConnection = new SqliteConnection("Data Source=:memory:");
                    sqliteConnection.Open();
                    configurationContainer.AddSingleton(sqliteConnection);

                    var msSqlConnection = new SqlConnection("Data Source=.\\SANDBOX;Initial Catalog=Tests;Integrated Security=True");
                    configurationContainer.AddSingleton(msSqlConnection);

                    _serviceProvider = configurationContainer.BuildServiceProvider();
                }

                return _serviceProvider;
            }
        }

        [OneTimeSetUp]
        public void SetupDatabase()
        {
            var connection = ServiceProvider.GetService<SqliteConnection>();

            var dtos = GetDbClasses().Select(dbClass => (DtoBase)Activator.CreateInstance(dbClass)).ToArray();
            foreach (var dto in dtos)
            {
                if (dto.TableInformation != DbTable.EmptyDbTable)
                {
                    var sql = dto.TableInformation.CreateTableSqlString(ConnectionType.SqLite);
                    new SqliteCommand(sql, connection).ExecuteNonQuery();
                }
            }

            foreach (var dto in dtos)
            {
                if (dto.ViewInformation != DbView.EmptyDbView)
                {
                    var sql = dto.ViewInformation.CreateViewSqlString(ConnectionType.SqLite);
                    new SqliteCommand(sql, connection).ExecuteNonQuery();
                }
            }

            foreach (var dto in dtos)
            {
                if (dto.TriggersInformation.Any())
                {
                    foreach (var triggerInformation in dto.TriggersInformation)
                    {
                        var sql = triggerInformation.CreateTriggerSqlString(ConnectionType.SqLite);
                        new SqliteCommand(sql, connection).ExecuteNonQuery();
                    }
                }
            }
        }

        private IEnumerable<Type> GetDbClasses()
        {
            return typeof(DtoBase)
                .Assembly
                .GetTypes()
                .Where(type => type.IsSubclassOf(typeof(DtoBase)));
        }
    }
}
