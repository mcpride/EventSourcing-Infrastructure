using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using MS.EventSourcing.Infrastructure.EF.Models.Mapping;

namespace MS.EventSourcing.Infrastructure.EF.Models
{
    public class EventStoreContext : DbContext
    {
        private string _defaultSchema;

        static EventStoreContext()
        {
            //Database.SetInitializer<EventStoreContext>(new CreateDatabaseIfNotExists<EventStoreContext>());
            Database.SetInitializer<EventStoreContext>(null);
        }

        public EventStoreContext()
            : this("Name=EventStoreContext")
        {
        }

        public EventStoreContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public string DefaultSchema
        {
            set { _defaultSchema = value; }
        }

        public DbSet<EventStream> EventStreams { get; set; }
        public DbSet<SnapshotStream> SnapshotStreams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // don't pluralize table names
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            if (!string.IsNullOrWhiteSpace(_defaultSchema))
            {
                modelBuilder.HasDefaultSchema(_defaultSchema);
            }

            modelBuilder.Configurations.Add(new EventStreamMap());
            modelBuilder.Configurations.Add(new SnapshotStreamMap());
        }
    }
}
