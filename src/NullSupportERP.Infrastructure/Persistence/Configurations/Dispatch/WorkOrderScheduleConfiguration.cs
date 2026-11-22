using NullSupportERP.Domain.Dispatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NullSupportERP.Infrastructure.Persistence.Configurations.Dispatch;

internal sealed class WorkOrderScheduleConfiguration
    : IEntityTypeConfiguration<WorkOrderSchedule>
{
    public void Configure(
        EntityTypeBuilder<WorkOrderSchedule> builder)
    {
        builder.ToTable(
            "work_order_schedules",
            "dispatch");

        builder.HasKey(
            schedule => schedule.Id);

        builder.Property(
                schedule => schedule.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(
                schedule => schedule.WorkOrderId)
            .HasColumnName("work_order_id")
            .IsRequired();

        builder.Property(
                schedule => schedule.ScheduledStartUtc)
            .HasColumnName("scheduled_start_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(
                schedule => schedule.ScheduledEndUtc)
            .HasColumnName("scheduled_end_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(
                schedule => schedule.ArrivalWindowStartUtc)
            .HasColumnName("arrival_window_start_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(
                schedule => schedule.ArrivalWindowEndUtc)
            .HasColumnName("arrival_window_end_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(
                schedule => schedule.TimeZoneId)
            .HasColumnName("time_zone_id")
            .HasMaxLength(
                WorkOrderSchedule.MaximumTimeZoneIdLength)
            .IsRequired();

        builder.Property(
                schedule => schedule.DispatchNotes)
            .HasColumnName("dispatch_notes")
            .HasMaxLength(
                WorkOrderSchedule.MaximumDispatchNotesLength);

        builder.Property(
                schedule => schedule.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(25)
            .IsRequired();

        builder.Property(
                schedule => schedule.StartedAtUtc)
            .HasColumnName("started_at_utc")
            .HasColumnType("timestamp with time zone");

        builder.Property(
                schedule => schedule.StartedByUserId)
            .HasColumnName("started_by_user_id");

        builder.Property(
                schedule => schedule.CompletedAtUtc)
            .HasColumnName("completed_at_utc")
            .HasColumnType("timestamp with time zone");

        builder.Property(
                schedule => schedule.CompletedByUserId)
            .HasColumnName("completed_by_user_id");

        builder.Property(
                schedule => schedule.CancelledAtUtc)
            .HasColumnName("cancelled_at_utc")
            .HasColumnType("timestamp with time zone");

        builder.Property(
                schedule => schedule.CancelledByUserId)
            .HasColumnName("cancelled_by_user_id");

        builder.Property(
                schedule => schedule.CancellationReason)
            .HasColumnName("cancellation_reason")
            .HasMaxLength(
                WorkOrderSchedule.MaximumCancellationReasonLength);

        builder.Property(
                schedule => schedule.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(
                schedule => schedule.CreatedByUserId)
            .HasColumnName("created_by_user_id")
            .IsRequired();

        builder.Property(
                schedule => schedule.LastModifiedAtUtc)
            .HasColumnName("last_modified_at_utc")
            .HasColumnType("timestamp with time zone");

        builder.Property(
                schedule => schedule.LastModifiedByUserId)
            .HasColumnName("last_modified_by_user_id");

        builder.Ignore(
            schedule => schedule.Technicians);

        builder.Ignore(
            schedule => schedule.PrimaryTechnicianUserId);

        builder.Ignore(
            schedule => schedule.IsScheduled);

        builder.Ignore(
            schedule => schedule.IsInProgress);

        builder.Ignore(
            schedule => schedule.IsCompleted);

        builder.Ignore(
            schedule => schedule.IsCancelled);

        builder.HasMany<WorkOrderScheduleTechnician>(
                "_technicians")
            .WithOne()
            .HasForeignKey(
                technician =>
                    technician.WorkOrderScheduleId)
            .OnDelete(
                DeleteBehavior.Cascade);

        builder.Navigation(
                "_technicians")
            .UsePropertyAccessMode(
                PropertyAccessMode.Field);

        builder.HasIndex(
                schedule => schedule.WorkOrderId)
            .IsUnique()
            .HasDatabaseName(
                "ux_work_order_schedules_work_order_id");

        builder.HasIndex(
                schedule => new
                {
                    schedule.ScheduledStartUtc,
                    schedule.ScheduledEndUtc
                })
            .HasDatabaseName(
                "ix_work_order_schedules_time_range");

        builder.HasIndex(
                schedule => schedule.Status)
            .HasDatabaseName(
                "ix_work_order_schedules_status");
    }
}
