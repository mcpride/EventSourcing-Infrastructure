using System.Data.Entity.Migrations;

namespace MS.EventSourcing.Infrastructure.MySql.EventStoreContextMigrations
{
    public partial class AddCombinedAggregateIdTypeIndexesToStreams : DbMigration
    {
        public override void Up()
        {
            CreateIndex("EventStream", new[] { "AggregateRootId", "AggregateType" }, false, "IdxAggregateRootIdAggregateType");
            CreateIndex("SnapshotStream", new[] { "AggregateRootId", "SnapshotType" }, false, "IdxAggregateRootIdSnapshotType");
        }
        
        public override void Down()
        {
            DropIndex("SnapshotStream", "IdxAggregateRootIdSnapshotType");
            DropIndex("EventStream", "IdxAggregateRootIdAggregateType");
        }
    }
}
