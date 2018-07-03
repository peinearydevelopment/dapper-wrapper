namespace DataAccess.Datastore
{
    using System;

    public class DbColumn
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public bool IsKey { get; set; }
        public bool IsNullable { get; set; }
        public DbForeignKey ForeignKey { get; set; }
    }
}
