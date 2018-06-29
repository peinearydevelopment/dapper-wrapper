namespace DataAccess.Datastore
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;

    public class DbView
    {
        public string Schema { get; set; }
        public string Name { get; set; }
        public string CreateViewScript { get; set; }
        public Type[] RequiredTypes { get; set; }

        public static DbView EmptyDbView = new DbView();

        public string ViewName(ConnectionType connectionType)
        {
            if (this == EmptyDbView)
            {
                throw new NotSupportedException("No sql string can be created for an empty db view.");
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

        public string CreateViewSqlString(ConnectionType connectionType)
        {
            if (this == EmptyDbView)
            {
                throw new NotSupportedException("No sql string can be created for an empty db view.");
            }

            foreach (var type in RequiredTypes)
            {
                var tableAttribute = type.GetCustomAttribute<TableAttribute>();

                if (tableAttribute == null)
                {
                    throw new ArgumentException("A view's required type must be decorated with a table attribute.");
                }

                CreateViewScript = CreateViewScript.Replace(type.Name, TableName(tableAttribute, connectionType));
            }

            switch (connectionType)
            {
                case ConnectionType.SqLite:
                    return $"CREATE VIEW {ViewName(connectionType)} AS {CreateViewScript};";
                case ConnectionType.MsSql:
                default:
                    throw new NotImplementedException();
            }
        }

        private string TableName(TableAttribute tableAttribute, ConnectionType connectionType)
        {
            switch (connectionType)
            {
                case ConnectionType.SqLite:
                    return $"[{tableAttribute.Schema}_{tableAttribute.Name}]";
                case ConnectionType.MsSql:
                default:
                    return $"[{tableAttribute.Schema}].[{tableAttribute.Name}]";
            }
        }
    }
}
