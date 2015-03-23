using System.Data.Entity.Migrations;
using MS.EventSourcing.Infrastructure.EF.Models;

namespace MS.EventSourcing.Infrastructure.EF.EventStoreContextMigrations
{
    public abstract class Configuration : DbMigrationsConfiguration<EventStoreContext>
    {
        protected Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"EventStoreContextMigrations";
        }

        protected override void Seed(EventStoreContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
