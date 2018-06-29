namespace DataAccess.Datastore
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public class DbTrigger
    {
        public TriggerType TriggerType { get; set; }
        public string CreateTriggerScript { get; set; }
        public Type[] RequiredTypes { get; set; }
        public Type Type { get; set; }

        public static DbTrigger EmptyDbTrigger = new DbTrigger();

        public string TriggerName(/*ConnectionType connectionType*/)
        {
            if (this == EmptyDbTrigger)
            {
                throw new NotSupportedException("No sql string can be created for an empty db trigger.");
            }

            var tableAttribute = Type.GetCustomAttribute<TableAttribute>();

            //switch (connectionType)
            //{
            //    case ConnectionType.SqLite:
                    return $"[{TriggerType.ToString()}{tableAttribute.Name}]";
            //    case ConnectionType.MsSql:
            //    default:
            //        return $"[{Schema}].[{Name}]";
            //}
        }

        public string CreateTriggerSqlString(ConnectionType connectionType)
        {
            if (this == EmptyDbTrigger)
            {
                throw new NotSupportedException("No sql string can be created for an empty db trigger.");
            }

            foreach (var type in RequiredTypes)
            {
                var tableAttribute = type.GetCustomAttribute<TableAttribute>();

                if (tableAttribute == null)
                {
                    throw new ArgumentException("A trigger's required type must be decorated with a table attribute.");
                }

                CreateTriggerScript = CreateTriggerScript.Replace(type.Name, TableName(tableAttribute, connectionType));
            }

            CreateTriggerScript = CreateTriggerScript.Replace("(deleted/OLD)", connectionType == ConnectionType.SqLite ? "OLD" : "deleted");
            CreateTriggerScript = CreateTriggerScript.Replace("(inserted/NEW)", connectionType == ConnectionType.SqLite ? "NEW" : "inserted");

            var selfTableAttribute = Type.GetCustomAttribute<TableAttribute>();

            switch (connectionType)
            {
                case ConnectionType.SqLite:
                    return $@"CREATE TRIGGER {TriggerName()}
    {Regex.Replace(TriggerType.ToString(), "((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", " $1")} ON [{selfTableAttribute.Schema}_{selfTableAttribute.Name}]
BEGIN
    {CreateTriggerScript}
END;";
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
