namespace UnitTests
{
    using DataAccess;
    using Microsoft.Data.Sqlite;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;

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
                    var sqLiteContext = new SqLiteDatastoreContext(sqliteConnection);
                    configurationContainer.AddSingleton(sqLiteContext);
                    configurationContainer.AddSingleton<DatastoreContext>(sqLiteContext);

                    var msSqlConnection = new SqlConnection("Data Source=.\\SANDBOX;Initial Catalog=Tests;Integrated Security=True");
                    configurationContainer.AddSingleton(msSqlConnection);
                    configurationContainer.AddSingleton(new MsSqlDatastoreContext(msSqlConnection));

                    _serviceProvider = configurationContainer.BuildServiceProvider();
                }

                return _serviceProvider;
            }
        }

        [OneTimeSetUp]
        public void SetupDatabase()
        {
            var connection = ServiceProvider.GetService<SqliteConnection>();

            foreach(var dbClass in GetDbClasses())
            {
                var tableInformation = GetTableName(dbClass);
                var columns = GetColumns(dbClass);
                CreateTable(connection, tableInformation, columns);
            }
        }

        private IEnumerable<Type> GetDbClasses()
        {
            return ServiceProvider
                .GetService<DatastoreContext>()
                .GetType()
                .GetTypeInfo()
                .Assembly
                .GetTypes()
                .Where(HasDbTable);
        }

        private bool HasDbTable(Type type)
        {
            return type
                .GetCustomAttributes()
                .Any(attribute => attribute.GetType() == typeof(TableAttribute));
        }

        private (string schema, string tableName) GetTableName(Type dbClass)
        {
            var tableAttribute = dbClass
                .GetCustomAttributes()
                .First(attribute => attribute.GetType() == typeof(TableAttribute)) as TableAttribute;
            return (tableAttribute.Schema, tableAttribute.Name);
        }

        private IEnumerable<(string Name, Type type, bool isKey)> GetColumns(Type dbClass)
        {
            return dbClass
                .GetProperties()
                .Select(property => (property.Name, property.PropertyType, property.GetCustomAttributes().Any(attribute => attribute.GetType() == typeof(KeyAttribute))));
        }

        private void CreateTable(SqliteConnection connection, (string schema, string tableName) tableInformation, IEnumerable<(string Name, Type type, bool isKey)> columnInformation)
        {
            var sql = $@"
CREATE TABLE [{tableInformation.schema}_{tableInformation.tableName}]
(
    {string.Join(",\n\t", columnInformation.Select(column =>
            {
                var type = column.type.IsGenericType && column.type.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(column.type) : column.type;

                return $"[{column.Name}] [{TypeSqliteTypeMapper[type]}] {(column.isKey ? "PRIMARY KEY" : string.Empty)}";
            }))}
);";
            new SqliteCommand(sql, connection).ExecuteNonQuery();
        }

        // https://www.sqlite.org/datatype3.html#affinity_name_examples
        private readonly Dictionary<Type, string> TypeSqliteTypeMapper = new Dictionary<Type, string>
        {
            { typeof(short), "INTEGER" },
            { typeof(int), "INTEGER" },
            { typeof(long), "INTEGER" },
            { typeof(string), "TEXT" },
            { typeof(float), "REAL" },
            { typeof(double), "REAL" },
            { typeof(decimal), "NUMERIC" },
            { typeof(bool), "NUMERIC" },
            { typeof(DateTime), "NUMERIC" },
            { typeof(DateTimeOffset), "NUMERIC" }
        };
    }
}
