using MySql.Data.Entity;
using MS.EventSourcing.Infrastructure.EF.EventStoreContextMigrations;
using MS.EventSourcing.Infrastructure.EF.MySql.Models;

namespace MS.EventSourcing.Infrastructure.EF.MySql.EventStoreContextMigrations
{
    internal sealed class MySqlConfiguration : Configuration
    {
        private const string MySqlProviderInvariantName = "MySql.Data.MySqlClient";

        public MySqlConfiguration()
        // ReSharper disable RedundantBaseConstructorCall
            : base()
        // ReSharper restore RedundantBaseConstructorCall
        {
            SetSqlGenerator(MySqlProviderInvariantName, new MySqlMigrationSqlGenerator());
            SetHistoryContextFactory(MySqlProviderInvariantName, (conn, schema) => new MySqlMigrationHistoryContext(conn, schema));
        }
    }
}
