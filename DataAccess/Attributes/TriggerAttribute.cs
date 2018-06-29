namespace DataAccess.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TriggerAttribute : Attribute
    {
        public TriggerType TriggerType { get; }

        public string CreateTriggerScript { get; set; }

        public Type[] RequiredTypes { get; set; }

        public TriggerAttribute(TriggerType type)
        {
            TriggerType = type;
        }
    }
}
