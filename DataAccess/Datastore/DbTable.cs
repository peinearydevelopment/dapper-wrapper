namespace DataAccess.Datastore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;

    public class DbTable
    {
        public string Schema { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }

        public static DbTable EmptyDbTable = new DbTable();

        public string TableName(ConnectionType connectionType)
        {
            if (this == EmptyDbTable)
            {
                throw new NotSupportedException("No sql string can be created for an empty db table.");
            }

            switch (connectionType)
            {
                case ConnectionType.SqLite:
                    return $"[{Schema}_{Name}]";
                case ConnectionType.MsSql:
                default:
                    return $"[{Schema}].[{Name}]";
            }
        }

        public string CreateTableSqlString(ConnectionType connectionType)
        {
            if (this == EmptyDbTable)
            {
                throw new NotSupportedException("No sql string can be created for an empty db table.");
            }

            switch (connectionType)
            {
                case ConnectionType.SqLite:
                    return $@"CREATE TABLE {TableName(connectionType)}
(
    {string.Join(
",\n\t",
        Columns.Select(column =>
        {
            column.IsNullable = column.Type.IsGenericType && column.Type.GetGenericTypeDefinition() == typeof(Nullable<>);
            var type = column.IsNullable ? Nullable.GetUnderlyingType(column.Type) : column.Type;

            return $"[{column.Name}] [{TypeSqliteTypeMapper[type]}]{(column.IsKey ? " PRIMARY KEY" : string.Empty)}";
        }))
    }
);";
                case ConnectionType.MsSql:
                default:
                    throw new NotImplementedException();
            }
        }

        public DbColumn[] Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = Type
                                .GetProperties()
                                .Where(property => property.GetCustomAttribute<NotMappedAttribute>() == null)
                                .Select(property => new DbColumn
                                {
                                    Name = property.Name,
                                    Type = property.PropertyType,
                                    IsKey = property.GetCustomAttribute<KeyAttribute>() != null
                                })
                                .ToArray();
                }

                return _columns;
            }
        }

        private DbColumn[] _columns { get; set; }

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
            { typeof(DateTime), "DATETIME" },
            { typeof(DateTimeOffset), "DATETIME" }
        };
    }
}
