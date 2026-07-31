using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MbaCrm.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentFileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "ServiceRequestDocuments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "ServiceRequestDocuments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "ServiceRequestDocuments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FileUploadedAt",
                table: "ServiceRequestDocuments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "ServiceRequestDocuments",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoredFileName",
                table: "ServiceRequestDocuments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "ServiceRequestDocuments");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "ServiceRequestDocuments");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "ServiceRequestDocuments");

            migrationBuilder.DropColumn(
                name: "FileUploadedAt",
                table: "ServiceRequestDocuments");

            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                table: "ServiceRequestDocuments");

            migrationBuilder.DropColumn(
                name: "StoredFileName",
                table: "ServiceRequestDocuments");
        }
    }
}
