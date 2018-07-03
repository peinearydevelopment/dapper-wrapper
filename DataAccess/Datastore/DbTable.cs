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
        public DtoBase DtoBase { get; set; }

        public static DbTable EmptyDbTable = new DbTable();

        public string TableName(ConnectionType connectionType, Type tableType = null)
        {
            if (this == EmptyDbTable)
            {
                throw new NotSupportedException("No sql string can be created for an empty db table.");
            }

            var tableAttribute = tableType?.GetCustomAttribute<TableAttribute>();
            switch (connectionType)
            {
                case ConnectionType.SqLite:
                    return $"[{(tableAttribute == null ? Schema : tableAttribute.Schema)}_{(tableAttribute == null ? Name : tableAttribute.Name)}]";
                case ConnectionType.MsSql:
                default:
                    return $"[{(tableAttribute == null ? Schema : tableAttribute.Schema)}].[{(tableAttribute == null ? Name : tableAttribute.Name)}]";
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
                    var columns = string.Join(
",\n\t",
        DtoBase.Columns.Select(column =>
        {
            column.IsNullable = column.Type.IsGenericType && column.Type.GetGenericTypeDefinition() == typeof(Nullable<>);
            var type = column.IsNullable ? Nullable.GetUnderlyingType(column.Type) : column.Type;

            // Don't use Autoincrement as per docs https://sqlite.org/autoinc.html
            return $"[{column.Name}] [{TypeSqliteTypeMapper[type]}]{(column.IsKey ? " PRIMARY KEY" : string.Empty)}";
        }));

                    var constraints = DtoBase.ForeignKeys.Any() ? $",\n\t {string.Join(",\n\t", DtoBase.ForeignKeys.Select(foreignKey => $"FOREIGN KEY({foreignKey.PropertyName}) REFERENCES {TableName(connectionType, foreignKey.ForeignType)}({foreignKey.ForeignType.GetProperties().First(property => property.GetCustomAttribute<KeyAttribute>() != null).Name})"))}": string.Empty;

                    return $@"CREATE TABLE {TableName(connectionType)}
(
    {columns}
    {constraints}
);";
                case ConnectionType.MsSql:
                default:
                    throw new NotImplementedException();
            }
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
            { typeof(DateTime), "DATETIME" },
            { typeof(DateTimeOffset), "DATETIME" }
        };
    }
}
