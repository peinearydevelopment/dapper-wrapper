namespace DataAccess.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TriggerAttribute : Attribute
    {
        public string Name { get; }
        public string TriggerStatement { get; }
        public Type[] Dependencies { get; }

        public TriggerAttribute(DatastoreContext context, string name, string triggerStatement, Type[] dependencies = null)
        {
            Name = name;
            TriggerStatement = triggerStatement;
            Dependencies = dependencies;

            foreach(var type in dependencies)
            {
                TriggerStatement = context.Uses(TriggerStatement, type);
            }
        }
    }
}
