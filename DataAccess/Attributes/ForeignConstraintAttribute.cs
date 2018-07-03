namespace DataAccess.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ForeignConstraintAttribute : Attribute
    {
        public Type ForeignConstraintType { get; }

        public ForeignConstraintAttribute(Type type)
        {
            ForeignConstraintType = type;
        }
    }
}
