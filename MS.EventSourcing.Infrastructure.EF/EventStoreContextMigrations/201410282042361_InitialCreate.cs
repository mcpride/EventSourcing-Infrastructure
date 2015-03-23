using System;
using System.Data.Entity.Migrations;

namespace MS.EventSourcing.Infrastructure.EF.EventStoreContextMigrations
{
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EventStream",
                c => new
                {
                    Id = c.Long(false, true),
                    AggregateRootId = c.Guid(false, defaultValue: Guid.Empty),
                    DateCreated = c.DateTime(false, 0),
                    SequenceStart = c.Long(false, defaultValue: 0),
                    SequenceEnd = c.Long(false, defaultValue: 0),
                    AggregateType = c.String(maxLength: 255, unicode: false, storeType: "nvarchar"),
                    EventData = c.String(unicode: false),
                })
                .PrimaryKey(t => t.Id);
            CreateIndex("EventStream", "AggregateRootId", false, "IdxAggregateRootId");
            CreateIndex("EventStream", "AggregateType", false, "IdxAggregateType");
            CreateIndex("EventStream", "DateCreated", false, "IdxDateCreated");

            CreateTable(
                "SnapshotStream",
                c => new
                {
                    Id = c.Long(false, true),
                    AggregateRootId = c.Guid(false, defaultValue: Guid.Empty),
                    DateCreated = c.DateTime(false, 0),
                    Version = c.Long(false, defaultValue: 0),
                    SnapshotType = c.String(maxLength: 255, unicode: false, storeType: "nvarchar"),
                    SnapshotData = c.String(unicode: false),
                })
                .PrimaryKey(t => t.Id);
            CreateIndex("SnapshotStream", "AggregateRootId", false, "IdxAggregateRootId");
            CreateIndex("SnapshotStream", "SnapshotType", false, "IdxSnapshotType");
            CreateIndex("SnapshotStream", "DateCreated", false, "IdxDateCreated");
        }

        public override void Down()
        {
            DropTable("SnapshotStream");
            DropTable("EventStream");
        }
    }
}
