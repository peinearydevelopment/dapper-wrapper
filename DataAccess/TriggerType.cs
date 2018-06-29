namespace DataAccess
{
    public enum TriggerType
    {
        BeforeInsert = 0, // NEW/inserted
        AfterInsert = 1, // NEW/inserted
        BeforeUpdate = 2, // NEW/inserted; OLD/deleted
        AfterUpdate = 3, // NEW/inserted; OLD/deleted
        BeforeDelete = 4, // OLD/deleted
        AfterDelete = 5 // OLD/deleted
    }
}
