namespace DataAccess
{
    using Microsoft.Data.Sqlite;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;

    public enum ConnectionType
    {
        Unknown = 0,
        SqLite = 1,
        MsSql = 2
    }

    public abstract class DatastoreContext
    {
        public virtual DbConnection DbConnection { get; }
        protected virtual ConnectionType ConnectionType { get; set; }
        public abstract string Uses(string sql, Type type);
        public abstract string Uses<TDto>(string sql);
    }

    public class DatastoreContext<T> : DatastoreContext
        where T : DbConnection
    {
        public override DbConnection DbConnection { get; }
        protected override ConnectionType ConnectionType { get; set; }

        public DatastoreContext(T dbConnection)
        {
            DbConnection = dbConnection;
        }

        public override string Uses(string sql, Type type)
        {
            return ReplaceTypeInfo(sql, type);
        }

        public override string Uses<TDto>(string sql)
        {
            var type = typeof(TDto);
            return ReplaceTypeInfo(sql, type);
        }

        private string ReplaceTypeInfo(string sql, Type type)
        {
            var (schema, tableName) = GetTableInformation(type);

            switch (ConnectionType)
            {
                case ConnectionType.SqLite:
                    return sql.Replace(type.Name, $"{schema}_{tableName}");
                case ConnectionType.MsSql:
                default:
                    return sql.Replace(type.Name, $"[{schema}].[{tableName}]");
            }
        }

        private (string schema, string tableName) GetTableInformation(Type dbClass)
        {
            var tableAttribute = dbClass
                .GetCustomAttributes()
                .First(attribute => attribute.GetType() == typeof(TableAttribute)) as TableAttribute;
            return (tableAttribute.Schema, tableAttribute.Name);
        }
    }

    public class SqLiteDatastoreContext : DatastoreContext<SqliteConnection>
    {
        public SqLiteDatastoreContext(string connectionString)
            : base(new SqliteConnection(connectionString))
        {
        }

        public SqLiteDatastoreContext(SqliteConnection dbConnection)
            : base(dbConnection)
        {
            ConnectionType = ConnectionType.SqLite;
        }
    }

    public class MsSqlDatastoreContext : DatastoreContext<SqlConnection>
    {
        public MsSqlDatastoreContext(string connectionString)
            : base(new SqlConnection(connectionString))
        {
        }

        public MsSqlDatastoreContext(SqlConnection dbConnection)
            : base(dbConnection)
        {
            ConnectionType = ConnectionType.MsSql;
        }
    }
}
