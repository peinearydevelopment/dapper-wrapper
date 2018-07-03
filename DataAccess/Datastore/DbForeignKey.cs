namespace DataAccess.Datastore
{
    using System;

    public class DbForeignKey
    {
        public string PropertyName { get; set; }
        public Type ForeignType { get; set; }
    }
}
