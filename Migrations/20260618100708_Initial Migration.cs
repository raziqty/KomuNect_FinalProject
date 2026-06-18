using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KomuNect_Demo.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admins",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    admin_id = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admins", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "announcement_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_announcement_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "complaint_subjects",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subject_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_complaint_subjects", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "residents",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    middle_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    last_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    birthdate = table.Column<DateOnly>(type: "date", nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_residents", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "announcements",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    author_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    event_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    event_location = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    posted_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    edited_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_announcements", x => x.id);
                    table.ForeignKey(
                        name: "FK_announcements_admins_author_id",
                        column: x => x.author_id,
                        principalTable: "admins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_announcements_announcement_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "announcement_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "complaints",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    resident_id = table.Column<int>(type: "int", nullable: false),
                    subject_id = table.Column<int>(type: "int", nullable: false),
                    details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    filed_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    admin_note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_complaints", x => x.id);
                    table.ForeignKey(
                        name: "FK_complaints_complaint_subjects_subject_id",
                        column: x => x.subject_id,
                        principalTable: "complaint_subjects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_complaints_residents_resident_id",
                        column: x => x.resident_id,
                        principalTable: "residents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "announcement_categories",
                columns: new[] { "id", "category_name" },
                values: new object[,]
                {
                    { 1, "General" },
                    { 2, "Health" },
                    { 3, "Safety" },
                    { 4, "Events" },
                    { 5, "Infrastructure" }
                });

            migrationBuilder.InsertData(
                table: "complaint_subjects",
                columns: new[] { "id", "subject_name" },
                values: new object[,]
                {
                    { 1, "Noise Complaint" },
                    { 2, "Garbage / Sanitation" },
                    { 3, "Illegal Structures" },
                    { 4, "Public Disturbance" },
                    { 5, "Others" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_admins_admin_id",
                table: "admins",
                column: "admin_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_admins_username",
                table: "admins",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_announcement_categories_category_name",
                table: "announcement_categories",
                column: "category_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_announcements_author_id",
                table: "announcements",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_announcements_category_id",
                table: "announcements",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_complaint_subjects_subject_name",
                table: "complaint_subjects",
                column: "subject_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_complaints_resident_id",
                table: "complaints",
                column: "resident_id");

            migrationBuilder.CreateIndex(
                name: "IX_complaints_subject_id",
                table: "complaints",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "IX_residents_email",
                table: "residents",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "announcements");

            migrationBuilder.DropTable(
                name: "complaints");

            migrationBuilder.DropTable(
                name: "admins");

            migrationBuilder.DropTable(
                name: "announcement_categories");

            migrationBuilder.DropTable(
                name: "complaint_subjects");

            migrationBuilder.DropTable(
                name: "residents");
        }
    }
}
