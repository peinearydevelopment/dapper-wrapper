namespace DataAccess
{
    using DataAccess.Attributes;
    using DataAccess.Datastore;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;

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

        private DbTable _tableInformation { get; set; }
        private DbView _viewInformation { get; set; }
        private DbTrigger[] _triggersInformation { get; set; }
        private Type _type { get; }
    }
}
