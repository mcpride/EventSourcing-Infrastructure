// <auto-generated />
namespace MS.EventSourcing.Infrastructure.MySql.EventStoreContextMigrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.1.1-30610")]
    public sealed partial class AddCombinedAggregateIdTypeIndexesToStreams : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(AddCombinedAggregateIdTypeIndexesToStreams));
        
        string IMigrationMetadata.Id
        {
            get { return "201501141419541_AddCombinedAggregateIdTypeIndexesToStreams"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}
