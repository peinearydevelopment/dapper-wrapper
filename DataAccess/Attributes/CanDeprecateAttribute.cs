namespace DataAccess.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CanDeprecateAttribute : Attribute
    {
    }
}
