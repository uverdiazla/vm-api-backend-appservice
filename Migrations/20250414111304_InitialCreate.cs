using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace vm_api_backend_appservice.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OperatingSystems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OsType = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatingSystems", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StatusEnum = table.Column<int>(type: "int", nullable: false),
                    ColorCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VirtualMachines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cores = table.Column<int>(type: "int", nullable: false),
                    Ram = table.Column<int>(type: "int", nullable: false),
                    Disk = table.Column<int>(type: "int", nullable: false),
                    OperatingSystemId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hostname = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddress = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualMachines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VirtualMachines_OperatingSystems_OperatingSystemId",
                        column: x => x.OperatingSystemId,
                        principalTable: "OperatingSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VirtualMachines_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "OperatingSystems",
                columns: new[] { "Id", "Description", "IsActive", "Name", "OsType", "Version" },
                values: new object[,]
                {
                    { 1, "Microsoft Windows Server 2019", true, "Windows Server 2019", 0, "2019" },
                    { 2, "Microsoft Windows Server 2022", true, "Windows Server 2022", 1, "2022" },
                    { 3, "Ubuntu 18.04 Long Term Support", true, "Ubuntu 18.04 LTS", 2, "18.04" },
                    { 4, "Ubuntu 20.04 Long Term Support", true, "Ubuntu 20.04 LTS", 3, "20.04" },
                    { 5, "Ubuntu 22.04 Long Term Support", true, "Ubuntu 22.04 LTS", 4, "22.04" },
                    { 6, "CentOS Linux 7", true, "CentOS 7", 7, "7" },
                    { 7, "CentOS Linux 8", true, "CentOS 8", 8, "8" },
                    { 8, "Debian 10 (Buster)", true, "Debian 10", 5, "10" },
                    { 9, "Debian 11 (Bullseye)", true, "Debian 11", 6, "11" }
                });

            migrationBuilder.InsertData(
                table: "Statuses",
                columns: new[] { "Id", "ColorCode", "Description", "Name", "StatusEnum" },
                values: new object[,]
                {
                    { 1, "#4CAF50", "The virtual machine is running", "Running", 0 },
                    { 2, "#F44336", "The virtual machine is stopped", "Stopped", 1 },
                    { 3, "#2196F3", "The virtual machine is being provisioned", "Provisioning", 2 },
                    { 4, "#FF9800", "The virtual machine has failed", "Failed", 3 },
                    { 5, "#9C27B0", "The virtual machine is suspended", "Suspended", 4 },
                    { 6, "#607D8B", "The virtual machine is under maintenance", "Maintenance", 5 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Name", "Password", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 4, 14, 11, 13, 4, 65, DateTimeKind.Utc).AddTicks(4304), "admin@vmapi.com", "Admin User", "$2a$11$S0LmeQQVgqh0znOUk/8QjuKkAFQ06sMLcAaVFmtJo2Q.KiJXt5C6S", 0, new DateTime(2025, 4, 14, 11, 13, 4, 65, DateTimeKind.Utc).AddTicks(4311) },
                    { 2, new DateTime(2025, 4, 14, 11, 13, 4, 189, DateTimeKind.Utc).AddTicks(2275), "client@vmapi.com", "Client User", "$2a$11$M6Zby0FXF4ovJbQtlLT9kuk5dPYQ8mKhtfsouGMNW9A4L4pyQ8Epm", 1, new DateTime(2025, 4, 14, 11, 13, 4, 189, DateTimeKind.Utc).AddTicks(2280) }
                });

            migrationBuilder.InsertData(
                table: "VirtualMachines",
                columns: new[] { "Id", "Cores", "CreatedAt", "Description", "Disk", "Hostname", "IpAddress", "Name", "OperatingSystemId", "Ram", "StatusId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 4, new DateTime(2025, 4, 14, 11, 13, 4, 189, DateTimeKind.Utc).AddTicks(3496), "Main web server", 500, "web-srv-01", "10.0.0.10", "Web Server", 5, 16, 1, new DateTime(2025, 4, 14, 11, 13, 4, 189, DateTimeKind.Utc).AddTicks(3497) },
                    { 2, 8, new DateTime(2025, 4, 14, 11, 13, 4, 189, DateTimeKind.Utc).AddTicks(3501), "Main database server", 1000, "db-srv-01", "10.0.0.11", "Database Server", 2, 32, 1, new DateTime(2025, 4, 14, 11, 13, 4, 189, DateTimeKind.Utc).AddTicks(3501) },
                    { 3, 2, new DateTime(2025, 4, 14, 11, 13, 4, 189, DateTimeKind.Utc).AddTicks(3505), "Testing environment", 250, "test-srv-01", "10.0.0.12", "Test Environment", 7, 8, 2, new DateTime(2025, 4, 14, 11, 13, 4, 189, DateTimeKind.Utc).AddTicks(3505) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_VirtualMachines_OperatingSystemId",
                table: "VirtualMachines",
                column: "OperatingSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualMachines_StatusId",
                table: "VirtualMachines",
                column: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "VirtualMachines");

            migrationBuilder.DropTable(
                name: "OperatingSystems");

            migrationBuilder.DropTable(
                name: "Statuses");
        }
    }
}
