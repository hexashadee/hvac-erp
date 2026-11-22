using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NullSupportERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialDispatchPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dispatch");

            migrationBuilder.CreateTable(
                name: "work_order_schedules",
                schema: "dispatch",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scheduled_start_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    scheduled_end_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    arrival_window_start_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    arrival_window_end_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    time_zone_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    dispatch_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    status = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    started_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    started_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    completed_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    completed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cancelled_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancelled_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cancellation_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_modified_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_order_schedules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "work_order_schedule_technicians",
                schema: "dispatch",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_order_schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    assigned_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    assigned_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_modified_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_order_schedule_technicians", x => x.id);
                    table.ForeignKey(
                        name: "FK_work_order_schedule_technicians_work_order_schedules_work_o~",
                        column: x => x.work_order_schedule_id,
                        principalSchema: "dispatch",
                        principalTable: "work_order_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_schedule_technicians_user_assigned",
                schema: "dispatch",
                table: "work_order_schedule_technicians",
                columns: new[] { "technician_user_id", "assigned_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_schedule_technicians_user_id",
                schema: "dispatch",
                table: "work_order_schedule_technicians",
                column: "technician_user_id");

            migrationBuilder.CreateIndex(
                name: "ux_schedule_technicians_schedule_user",
                schema: "dispatch",
                table: "work_order_schedule_technicians",
                columns: new[] { "work_order_schedule_id", "technician_user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_work_order_schedules_status",
                schema: "dispatch",
                table: "work_order_schedules",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_work_order_schedules_time_range",
                schema: "dispatch",
                table: "work_order_schedules",
                columns: new[] { "scheduled_start_utc", "scheduled_end_utc" });

            migrationBuilder.CreateIndex(
                name: "ux_work_order_schedules_work_order_id",
                schema: "dispatch",
                table: "work_order_schedules",
                column: "work_order_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "work_order_schedule_technicians",
                schema: "dispatch");

            migrationBuilder.DropTable(
                name: "work_order_schedules",
                schema: "dispatch");
        }
    }
}
