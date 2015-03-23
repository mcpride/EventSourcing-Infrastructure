using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MS.EventSourcing.Infrastructure.EF.Models.Mapping
{
    public class SnapshotStreamMap : EntityTypeConfiguration<SnapshotStream>
    {
        public SnapshotStreamMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.SnapshotType)
                .HasMaxLength(255);

            Property(t => t.SnapshotData)
                .HasMaxLength(1073741823);

            // Table & Column Mappings
            ToTable("SnapshotStream");
            Property(t => t.Id).HasColumnName("Id").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.AggregateRootId).HasColumnName("AggregateRootId").IsRequired();
            Property(t => t.DateCreated).HasColumnName("DateCreated").IsRequired();
            Property(t => t.Version).HasColumnName("Version").IsRequired();
            Property(t => t.SnapshotType).HasColumnName("SnapshotType").IsOptional();
            Property(t => t.SnapshotData).HasColumnName("SnapshotData").IsOptional();
        }
    }
}
