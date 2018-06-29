namespace DataAccess.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ViewAttribute : Attribute
    {
        public string Name { get; }

        public string Schema { get; set; }

        public string CreateViewScript { get; set; }

        public Type[] RequiredTypes { get; set; }

        public ViewAttribute(string name)
        {
            Name = name;
        }
    }
}
