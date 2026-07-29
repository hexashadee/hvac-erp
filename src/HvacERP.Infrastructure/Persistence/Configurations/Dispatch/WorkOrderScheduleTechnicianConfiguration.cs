using HvacERP.Domain.Dispatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HvacERP.Infrastructure.Persistence.Configurations.Dispatch;

internal sealed class WorkOrderScheduleTechnicianConfiguration
    : IEntityTypeConfiguration<WorkOrderScheduleTechnician>
{
    public void Configure(
        EntityTypeBuilder<WorkOrderScheduleTechnician> builder)
    {
        builder.ToTable(
            "work_order_schedule_technicians",
            "dispatch");

        builder.HasKey(
            technician => technician.Id);

        builder.Property(
                technician => technician.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(
                technician => technician.WorkOrderScheduleId)
            .HasColumnName("work_order_schedule_id")
            .IsRequired();

        builder.Property(
                technician => technician.TechnicianUserId)
            .HasColumnName("technician_user_id")
            .IsRequired();

        builder.Property(
                technician => technician.Role)
            .HasColumnName("role")
            .HasConversion<string>()
            .HasMaxLength(25)
            .IsRequired();

        builder.Property(
                technician => technician.AssignedAtUtc)
            .HasColumnName("assigned_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(
                technician => technician.AssignedByUserId)
            .HasColumnName("assigned_by_user_id")
            .IsRequired();

        builder.Property(
                technician => technician.LastModifiedAtUtc)
            .HasColumnName("last_modified_at_utc")
            .HasColumnType("timestamp with time zone");

        builder.Property(
                technician => technician.LastModifiedByUserId)
            .HasColumnName("last_modified_by_user_id");

        builder.HasIndex(
                technician => new
                {
                    technician.WorkOrderScheduleId,
                    technician.TechnicianUserId
                })
            .IsUnique()
            .HasDatabaseName(
                "ux_schedule_technicians_schedule_user");

        builder.HasIndex(
                technician => technician.TechnicianUserId)
            .HasDatabaseName(
                "ix_schedule_technicians_user_id");

        builder.HasIndex(
                technician => new
                {
                    technician.TechnicianUserId,
                    technician.AssignedAtUtc
                })
            .HasDatabaseName(
                "ix_schedule_technicians_user_assigned");
    }
}
