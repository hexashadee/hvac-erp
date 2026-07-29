using HvacERP.Domain.Dispatch;

namespace HvacERP.Domain.Tests.Dispatch;

public sealed class WorkOrderScheduleTests
{
    private static readonly Guid ScheduleId =
        Guid.Parse(
            "11111111-1111-1111-1111-111111111111");

    private static readonly Guid WorkOrderId =
        Guid.Parse(
            "22222222-2222-2222-2222-222222222222");

    private static readonly Guid DispatcherUserId =
        Guid.Parse(
            "33333333-3333-3333-3333-333333333333");

    private static readonly Guid TechnicianUserId =
        Guid.Parse(
            "44444444-4444-4444-4444-444444444444");

    private static readonly Guid SecondTechnicianUserId =
        Guid.Parse(
            "55555555-5555-5555-5555-555555555555");

    private static readonly Guid AssignmentId =
        Guid.Parse(
            "66666666-6666-6666-6666-666666666666");

    private static readonly Guid SecondAssignmentId =
        Guid.Parse(
            "77777777-7777-7777-7777-777777777777");

    private static readonly DateTimeOffset CreatedAtUtc =
        new(
            2026,
            7,
            29,
            12,
            0,
            0,
            TimeSpan.Zero);

    private static readonly DateTimeOffset ScheduledStartUtc =
        new(
            2026,
            7,
            30,
            13,
            0,
            0,
            TimeSpan.Zero);

    private static readonly DateTimeOffset ScheduledEndUtc =
        new(
            2026,
            7,
            30,
            16,
            0,
            0,
            TimeSpan.Zero);

    private static readonly DateTimeOffset ArrivalWindowStartUtc =
        new(
            2026,
            7,
            30,
            13,
            0,
            0,
            TimeSpan.Zero);

    private static readonly DateTimeOffset ArrivalWindowEndUtc =
        new(
            2026,
            7,
            30,
            15,
            0,
            0,
            TimeSpan.Zero);

    [Fact]
    public void Create_WithValidValues_CreatesSchedule()
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        Assert.Equal(
            ScheduleId,
            schedule.Id);

        Assert.Equal(
            WorkOrderId,
            schedule.WorkOrderId);

        Assert.Equal(
            ScheduledStartUtc,
            schedule.ScheduledStartUtc);

        Assert.Equal(
            ScheduledEndUtc,
            schedule.ScheduledEndUtc);

        Assert.Equal(
            ArrivalWindowStartUtc,
            schedule.ArrivalWindowStartUtc);

        Assert.Equal(
            ArrivalWindowEndUtc,
            schedule.ArrivalWindowEndUtc);

        Assert.Equal(
            "America/Detroit",
            schedule.TimeZoneId);

        Assert.Equal(
            "Call when en route.",
            schedule.DispatchNotes);

        Assert.Equal(
            WorkOrderScheduleStatus.Scheduled,
            schedule.Status);

        Assert.True(
            schedule.IsScheduled);

        Assert.Empty(
            schedule.Technicians);
    }

    [Fact]
    public void Create_NormalizesText()
    {
        WorkOrderSchedule schedule =
            WorkOrderSchedule.Create(
                ScheduleId,
                WorkOrderId,
                ScheduledStartUtc,
                ScheduledEndUtc,
                ArrivalWindowStartUtc,
                ArrivalWindowEndUtc,
                "  America/Detroit  ",
                "  Call when en route.  ",
                CreatedAtUtc,
                DispatcherUserId);

        Assert.Equal(
            "America/Detroit",
            schedule.TimeZoneId);

        Assert.Equal(
            "Call when en route.",
            schedule.DispatchNotes);
    }

    [Fact]
    public void Create_WithEmptyId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderSchedule.Create(
                    Guid.Empty,
                    WorkOrderId,
                    ScheduledStartUtc,
                    ScheduledEndUtc,
                    ArrivalWindowStartUtc,
                    ArrivalWindowEndUtc,
                    "America/Detroit",
                    null,
                    CreatedAtUtc,
                    DispatcherUserId));

        Assert.Equal(
            "id",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEndBeforeStart_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderSchedule.Create(
                    ScheduleId,
                    WorkOrderId,
                    ScheduledStartUtc,
                    ScheduledStartUtc,
                    ArrivalWindowStartUtc,
                    ArrivalWindowEndUtc,
                    "America/Detroit",
                    null,
                    CreatedAtUtc,
                    DispatcherUserId));

        Assert.Equal(
            "scheduledEndUtc",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithInvalidArrivalWindow_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderSchedule.Create(
                    ScheduleId,
                    WorkOrderId,
                    ScheduledStartUtc,
                    ScheduledEndUtc,
                    ScheduledStartUtc.AddMinutes(-30),
                    ArrivalWindowEndUtc,
                    "America/Detroit",
                    null,
                    CreatedAtUtc,
                    DispatcherUserId));

        Assert.Equal(
            "arrivalWindowStartUtc",
            exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithBlankTimeZoneId_ThrowsArgumentException(
        string timeZoneId)
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderSchedule.Create(
                    ScheduleId,
                    WorkOrderId,
                    ScheduledStartUtc,
                    ScheduledEndUtc,
                    ArrivalWindowStartUtc,
                    ArrivalWindowEndUtc,
                    timeZoneId,
                    null,
                    CreatedAtUtc,
                    DispatcherUserId));

        Assert.Equal(
            "timeZoneId",
            exception.ParamName);
    }

    [Fact]
    public void AssignTechnician_AddsTechnician()
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        WorkOrderScheduleTechnician assignment =
            schedule.AssignTechnician(
                AssignmentId,
                TechnicianUserId,
                WorkOrderTechnicianRole.Primary,
                CreatedAtUtc.AddMinutes(1),
                DispatcherUserId);

        Assert.Single(
            schedule.Technicians);

        Assert.Equal(
            AssignmentId,
            assignment.Id);

        Assert.Equal(
            ScheduleId,
            assignment.WorkOrderScheduleId);

        Assert.Equal(
            TechnicianUserId,
            assignment.TechnicianUserId);

        Assert.Equal(
            WorkOrderTechnicianRole.Primary,
            assignment.Role);

        Assert.Equal(
            TechnicianUserId,
            schedule.PrimaryTechnicianUserId);
    }

    [Fact]
    public void AssignTechnician_WithDuplicateTechnician_ThrowsInvalidOperationException()
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        AssignPrimaryTechnician(
            schedule);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => schedule.AssignTechnician(
                    SecondAssignmentId,
                    TechnicianUserId,
                    WorkOrderTechnicianRole.Assistant,
                    CreatedAtUtc.AddMinutes(2),
                    DispatcherUserId));

        Assert.Equal(
            "The technician is already assigned to this schedule.",
            exception.Message);
    }

    [Fact]
    public void AssignTechnician_WithSecondPrimary_ThrowsInvalidOperationException()
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        AssignPrimaryTechnician(
            schedule);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => schedule.AssignTechnician(
                    SecondAssignmentId,
                    SecondTechnicianUserId,
                    WorkOrderTechnicianRole.Primary,
                    CreatedAtUtc.AddMinutes(2),
                    DispatcherUserId));

        Assert.Equal(
            "Only one primary technician can be assigned to a schedule.",
            exception.Message);
    }

    [Fact]
    public void AssignTechnician_AllowsMultipleNonPrimaryTechnicians()
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        schedule.AssignTechnician(
            AssignmentId,
            TechnicianUserId,
            WorkOrderTechnicianRole.Assistant,
            CreatedAtUtc.AddMinutes(1),
            DispatcherUserId);

        schedule.AssignTechnician(
            SecondAssignmentId,
            SecondTechnicianUserId,
            WorkOrderTechnicianRole.Supervisor,
            CreatedAtUtc.AddMinutes(2),
            DispatcherUserId);

        Assert.Equal(
            2,
            schedule.Technicians.Count);
    }

    [Fact]
    public void ChangeTechnicianRole_ChangesRole()
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        schedule.AssignTechnician(
            AssignmentId,
            TechnicianUserId,
            WorkOrderTechnicianRole.Assistant,
            CreatedAtUtc.AddMinutes(1),
            DispatcherUserId);

        schedule.ChangeTechnicianRole(
            TechnicianUserId,
            WorkOrderTechnicianRole.Primary,
            CreatedAtUtc.AddMinutes(2),
            DispatcherUserId);

        WorkOrderScheduleTechnician technician =
            Assert.Single(
                schedule.Technicians);

        Assert.Equal(
            WorkOrderTechnicianRole.Primary,
            technician.Role);

        Assert.Equal(
            TechnicianUserId,
            schedule.PrimaryTechnicianUserId);
    }

    [Fact]
    public void ChangeTechnicianRole_ToSecondPrimary_ThrowsInvalidOperationException()
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        AssignPrimaryTechnician(
            schedule);

        schedule.AssignTechnician(
            SecondAssignmentId,
            SecondTechnicianUserId,
            WorkOrderTechnicianRole.Assistant,
            CreatedAtUtc.AddMinutes(2),
            DispatcherUserId);

        Assert.Throws<InvalidOperationException>(
            () => schedule.ChangeTechnicianRole(
                SecondTechnicianUserId,
                WorkOrderTechnicianRole.Primary,
                CreatedAtUtc.AddMinutes(3),
                DispatcherUserId));
    }

    [Fact]
    public void RemoveTechnician_RemovesTechnician()
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        AssignPrimaryTechnician(
            schedule);

        schedule.RemoveTechnician(
            TechnicianUserId,
            CreatedAtUtc.AddMinutes(2),
            DispatcherUserId);

        Assert.Empty(
            schedule.Technicians);

        Assert.Null(
            schedule.PrimaryTechnicianUserId);
    }

    [Fact]
    public void RemoveTechnician_WhenNotAssigned_ThrowsInvalidOperationException()
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => schedule.RemoveTechnician(
                    TechnicianUserId,
                    CreatedAtUtc.AddMinutes(2),
                    DispatcherUserId));

        Assert.Equal(
            "The technician is not assigned to this schedule.",
            exception.Message);
    }

    [Fact]
    public void Reschedule_WithValidValues_UpdatesSchedule()
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        DateTimeOffset newStartUtc =
            ScheduledStartUtc.AddDays(1);

        DateTimeOffset newEndUtc =
            ScheduledEndUtc.AddDays(1);

        DateTimeOffset newArrivalStartUtc =
            ArrivalWindowStartUtc.AddDays(1);

        DateTimeOffset newArrivalEndUtc =
            ArrivalWindowEndUtc.AddDays(1);

        schedule.Reschedule(
            newStartUtc,
            newEndUtc,
            newArrivalStartUtc,
            newArrivalEndUtc,
            "America/Chicago",
            "Use rear entrance.",
            CreatedAtUtc.AddMinutes(5),
            DispatcherUserId);

        Assert.Equal(
            newStartUtc,
            schedule.ScheduledStartUtc);

        Assert.Equal(
            newEndUtc,
            schedule.ScheduledEndUtc);

        Assert.Equal(
            "America/Chicago",
            schedule.TimeZoneId);

        Assert.Equal(
            "Use rear entrance.",
            schedule.DispatchNotes);
    }

    [Fact]
    public void UpdateDispatchNotes_WithWhitespace_StoresNull()
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        schedule.UpdateDispatchNotes(
            " ",
            CreatedAtUtc.AddMinutes(5),
            DispatcherUserId);

        Assert.Null(
            schedule.DispatchNotes);
    }

    [Fact]
    public void Start_WithAssignedTechnician_StartsSchedule()
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        AssignPrimaryTechnician(
            schedule);

        DateTimeOffset startedAtUtc =
            ScheduledStartUtc.AddMinutes(5);

        schedule.Start(
            startedAtUtc,
            TechnicianUserId);

        Assert.Equal(
            WorkOrderScheduleStatus.InProgress,
            schedule.Status);

        Assert.True(
            schedule.IsInProgress);

        Assert.Equal(
            startedAtUtc,
            schedule.StartedAtUtc);

        Assert.Equal(
            TechnicianUserId,
            schedule.StartedByUserId);
    }

    [Fact]
    public void Start_WithoutTechnician_ThrowsInvalidOperationException()
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => schedule.Start(
                    ScheduledStartUtc,
                    DispatcherUserId));

        Assert.Equal(
            "At least one technician must be assigned before the schedule can be started.",
            exception.Message);
    }

    [Fact]
    public void Start_WhenAlreadyStarted_ThrowsInvalidOperationException()
    {
        WorkOrderSchedule schedule =
            CreateStartedSchedule();

        Assert.Throws<InvalidOperationException>(
            () => schedule.Start(
                ScheduledStartUtc.AddMinutes(10),
                TechnicianUserId));
    }

    [Fact]
    public void Complete_InProgressSchedule_CompletesSchedule()
    {
        WorkOrderSchedule schedule =
            CreateStartedSchedule();

        DateTimeOffset completedAtUtc =
            ScheduledEndUtc.AddMinutes(-10);

        schedule.Complete(
            completedAtUtc,
            TechnicianUserId);

        Assert.Equal(
            WorkOrderScheduleStatus.Completed,
            schedule.Status);

        Assert.True(
            schedule.IsCompleted);

        Assert.Equal(
            completedAtUtc,
            schedule.CompletedAtUtc);

        Assert.Equal(
            TechnicianUserId,
            schedule.CompletedByUserId);
    }

    [Fact]
    public void Complete_ScheduledAppointment_ThrowsInvalidOperationException()
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => schedule.Complete(
                    ScheduledEndUtc,
                    DispatcherUserId));

        Assert.Equal(
            "Only an in-progress schedule can be completed.",
            exception.Message);
    }

    [Fact]
    public void Complete_BeforeStartedTime_ThrowsArgumentException()
    {
        WorkOrderSchedule schedule =
            CreateStartedSchedule();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => schedule.Complete(
                    ScheduledStartUtc,
                    TechnicianUserId));

        Assert.Equal(
            "completedAtUtc",
            exception.ParamName);
    }

    [Fact]
    public void Cancel_ScheduledAppointment_CancelsSchedule()
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        DateTimeOffset cancelledAtUtc =
            CreatedAtUtc.AddHours(1);

        schedule.Cancel(
            "  Customer requested another date.  ",
            cancelledAtUtc,
            DispatcherUserId);

        Assert.Equal(
            WorkOrderScheduleStatus.Cancelled,
            schedule.Status);

        Assert.True(
            schedule.IsCancelled);

        Assert.Equal(
            "Customer requested another date.",
            schedule.CancellationReason);

        Assert.Equal(
            cancelledAtUtc,
            schedule.CancelledAtUtc);

        Assert.Equal(
            DispatcherUserId,
            schedule.CancelledByUserId);
    }

    [Fact]
    public void Cancel_InProgressAppointment_CancelsSchedule()
    {
        WorkOrderSchedule schedule =
            CreateStartedSchedule();

        schedule.Cancel(
            "Unsafe conditions.",
            ScheduledStartUtc.AddMinutes(30),
            TechnicianUserId);

        Assert.True(
            schedule.IsCancelled);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Cancel_WithBlankReason_ThrowsArgumentException(
        string reason)
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => schedule.Cancel(
                    reason,
                    CreatedAtUtc.AddMinutes(10),
                    DispatcherUserId));

        Assert.Equal(
            "reason",
            exception.ParamName);
    }

    [Fact]
    public void CompletedSchedule_CannotBeChanged()
    {
        WorkOrderSchedule schedule =
            CreateCompletedSchedule();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => schedule.UpdateDispatchNotes(
                    "New note.",
                    ScheduledEndUtc.AddMinutes(1),
                    DispatcherUserId));

        Assert.Equal(
            "A completed schedule cannot be changed.",
            exception.Message);
    }

    [Fact]
    public void CancelledSchedule_CannotBeChanged()
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        schedule.Cancel(
            "Customer cancelled.",
            CreatedAtUtc.AddMinutes(10),
            DispatcherUserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => schedule.UpdateDispatchNotes(
                    "New note.",
                    CreatedAtUtc.AddMinutes(20),
                    DispatcherUserId));

        Assert.Equal(
            "A cancelled schedule cannot be changed.",
            exception.Message);
    }

    private static WorkOrderSchedule CreateSchedule()
    {
        return WorkOrderSchedule.Create(
            ScheduleId,
            WorkOrderId,
            ScheduledStartUtc,
            ScheduledEndUtc,
            ArrivalWindowStartUtc,
            ArrivalWindowEndUtc,
            "America/Detroit",
            "Call when en route.",
            CreatedAtUtc,
            DispatcherUserId);
    }

    private static void AssignPrimaryTechnician(
        WorkOrderSchedule schedule)
    {
        schedule.AssignTechnician(
            AssignmentId,
            TechnicianUserId,
            WorkOrderTechnicianRole.Primary,
            CreatedAtUtc.AddMinutes(1),
            DispatcherUserId);
    }

    private static WorkOrderSchedule CreateStartedSchedule()
    {
        WorkOrderSchedule schedule =
            CreateSchedule();

        AssignPrimaryTechnician(
            schedule);

        schedule.Start(
            ScheduledStartUtc.AddMinutes(5),
            TechnicianUserId);

        return schedule;
    }

    private static WorkOrderSchedule CreateCompletedSchedule()
    {
        WorkOrderSchedule schedule =
            CreateStartedSchedule();

        schedule.Complete(
            ScheduledEndUtc.AddMinutes(-10),
            TechnicianUserId);

        return schedule;
    }
}
