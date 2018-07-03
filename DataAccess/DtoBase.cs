namespace DataAccess
{
    using DataAccess.Attributes;
    using DataAccess.Datastore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public class DtoBase
    {
        public DtoBase()
        {
            _type = GetType();
        }

        [NotMapped]
        public DbTable TableInformation
        {
            get
            {
                if (_tableInformation == null)
                {
                    var tableAttribute = _type.GetCustomAttribute<TableAttribute>();

                    _tableInformation = tableAttribute == null ? DbTable.EmptyDbTable : new DbTable { Name = tableAttribute.Name, Schema = tableAttribute.Schema, Type = _type };
                }

                return _tableInformation;
            }
        }

        [NotMapped]
        public DbView ViewInformation
        {
            get
            {
                if (_viewInformation == null)
                {
                    var viewAttribute = _type.GetCustomAttribute<ViewAttribute>();

                    _viewInformation = viewAttribute == null ? DbView.EmptyDbView : new DbView { Name = viewAttribute.Name, Schema = viewAttribute.Schema, CreateViewScript = viewAttribute.CreateViewScript, RequiredTypes = viewAttribute.RequiredTypes };
                }

                return _viewInformation;
            }
        }

        [NotMapped]
        public DbTrigger[] TriggersInformation
        {
            get
            {
                if (_triggersInformation == null)
                {
                    var triggerAttributes = _type.GetCustomAttributes<TriggerAttribute>();

                    if (triggerAttributes.Any())
                    {
                        if (TableInformation == DbTable.EmptyDbTable)
                        {
                            throw new MissingMemberException($"{_type.Name} class contains a trigger attribute which requires a table attribute, but none was found.");
                        }

                        _triggersInformation = triggerAttributes.Select(trigger => new DbTrigger
                        {
                            CreateTriggerScript = trigger.CreateTriggerScript,
                            RequiredTypes = trigger.RequiredTypes,
                            TriggerType = trigger.TriggerType,
                            Type = _type
                        }).ToArray();
                    }
                    else
                    {
                        _triggersInformation = new DbTrigger[0];
                    }
                }

                return _triggersInformation;
            }
        }

        public (string sql, object parameters) CreateInsertSqlStatement(ConnectionType connectionType)
        {
            var insertBegin = $"INSERT INTO {TableInformation.TableName(connectionType)}";
            var columnNamesStringBuilder = new StringBuilder();
            var columnValueParametersStringBuilder = new StringBuilder();
            var columnValues = new ExpandoObject() as IDictionary<string, Object>;
            foreach (var column in TableInformation.Columns)
            {
                columnNamesStringBuilder.Append(column.Name).Append(", ");
                columnValueParametersStringBuilder.Append("@").Append(column.Name).Append(", ");
                columnValues.Add(column.Name, _type.GetProperty(column.Name).GetValue(this, null));
            }

            return (
                sql: $"{insertBegin} ({columnNamesStringBuilder.ToString(0, columnNamesStringBuilder.Length - 2)}) VALUES ({columnValueParametersStringBuilder.ToString(0, columnValueParametersStringBuilder.Length - 2)})",
                parameters: columnValues
            );
        }

        private DbTable _tableInformation { get; set; }
        private DbView _viewInformation { get; set; }
        private DbTrigger[] _triggersInformation { get; set; }
        private Type _type { get; }
    }
}
