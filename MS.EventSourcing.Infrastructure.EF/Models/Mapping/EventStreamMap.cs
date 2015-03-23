using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MS.EventSourcing.Infrastructure.EF.Models.Mapping
{
    public class EventStreamMap : EntityTypeConfiguration<EventStream>
    {
        public EventStreamMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Table & Column Mappings
            ToTable("EventStream");
            Property(t => t.Id).HasColumnName("Id").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.AggregateRootId).HasColumnName("AggregateRootId").IsRequired();
            Property(t => t.DateCreated).HasColumnName("DateCreated").IsRequired();
            Property(t => t.SequenceStart).HasColumnName("SequenceStart").IsRequired();
            Property(t => t.SequenceEnd).HasColumnName("SequenceEnd").IsRequired();
            Property(t => t.AggregateType).HasColumnName("AggregateType").IsOptional();
            Property(t => t.EventData).HasColumnName("EventData").IsOptional().HasMaxLength(1073741823);
        }
    }
}
