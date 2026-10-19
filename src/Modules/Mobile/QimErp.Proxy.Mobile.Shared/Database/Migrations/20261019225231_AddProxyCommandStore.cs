using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QimErp.Proxy.Mobile.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddProxyCommandStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: false),
                    CategoryDescription = table.Column<string>(type: "text", nullable: true),
                    DataType = table.Column<string>(type: "text", nullable: false),
                    Scope = table.Column<string>(type: "text", nullable: false, defaultValue: "CompanyOverridable"),
                    ValidationRules = table.Column<string>(type: "jsonb", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    CreatedByEmail = table.Column<string>(type: "text", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedByUserId = table.Column<string>(type: "text", nullable: false),
                    LastModifiedByEmail = table.Column<string>(type: "text", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataStatus = table.Column<string>(type: "text", nullable: true),
                    PreviousDataStatus = table.Column<string>(type: "text", nullable: true),
                    CustomFields = table.Column<string>(type: "jsonb", nullable: true),
                    EntityStatusCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntityStatusCreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    EntityStatusCreatedByEmail = table.Column<string>(type: "text", nullable: false),
                    EntityStatusCreatedByName = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntityStatusLastModifiedByUserId = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModifiedByEmail = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModifiedByName = table.Column<string>(type: "text", nullable: false),
                    OtherProperty = table.Column<string>(type: "text", nullable: false),
                    OtherProperty1 = table.Column<string>(type: "text", nullable: false),
                    IsGlobal = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityCodeConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    Prefix = table.Column<string>(type: "text", nullable: false),
                    Separator = table.Column<string>(type: "text", nullable: false),
                    IncludeYear = table.Column<bool>(type: "boolean", nullable: false),
                    PaddingWidth = table.Column<int>(type: "integer", nullable: false),
                    LastSequence = table.Column<long>(type: "bigint", nullable: false),
                    ManualHighWaterMark = table.Column<long>(type: "bigint", nullable: false),
                    Mode = table.Column<string>(type: "text", nullable: false),
                    ResetPeriod = table.Column<string>(type: "text", nullable: false),
                    LastResetPeriodKey = table.Column<string>(type: "text", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    CreatedByEmail = table.Column<string>(type: "text", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedByUserId = table.Column<string>(type: "text", nullable: false),
                    LastModifiedByEmail = table.Column<string>(type: "text", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataStatus = table.Column<string>(type: "text", nullable: true),
                    PreviousDataStatus = table.Column<string>(type: "text", nullable: true),
                    CustomFields = table.Column<string>(type: "jsonb", nullable: true),
                    EntityStatusCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntityStatusCreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    EntityStatusCreatedByEmail = table.Column<string>(type: "text", nullable: false),
                    EntityStatusCreatedByName = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntityStatusLastModifiedByUserId = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModifiedByEmail = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModifiedByName = table.Column<string>(type: "text", nullable: false),
                    OtherProperty = table.Column<string>(type: "text", nullable: false),
                    OtherProperty1 = table.Column<string>(type: "text", nullable: false),
                    IsGlobal = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityCodeConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityCodeReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    ReservationToken = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Metadata = table.Column<string>(type: "text", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    CreatedByEmail = table.Column<string>(type: "text", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedByUserId = table.Column<string>(type: "text", nullable: false),
                    LastModifiedByEmail = table.Column<string>(type: "text", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataStatus = table.Column<string>(type: "text", nullable: true),
                    PreviousDataStatus = table.Column<string>(type: "text", nullable: true),
                    CustomFields = table.Column<string>(type: "jsonb", nullable: true),
                    EntityStatusCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntityStatusCreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    EntityStatusCreatedByEmail = table.Column<string>(type: "text", nullable: false),
                    EntityStatusCreatedByName = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntityStatusLastModifiedByUserId = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModifiedByEmail = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModifiedByName = table.Column<string>(type: "text", nullable: false),
                    OtherProperty = table.Column<string>(type: "text", nullable: false),
                    OtherProperty1 = table.Column<string>(type: "text", nullable: false),
                    IsGlobal = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityCodeReservations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityWorkflowSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowCode = table.Column<string>(type: "text", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DefinitionVersion = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    CreatedByEmail = table.Column<string>(type: "text", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedByUserId = table.Column<string>(type: "text", nullable: false),
                    LastModifiedByEmail = table.Column<string>(type: "text", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataStatus = table.Column<string>(type: "text", nullable: true),
                    PreviousDataStatus = table.Column<string>(type: "text", nullable: true),
                    CustomFields = table.Column<string>(type: "jsonb", nullable: true),
                    EntityStatusCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntityStatusCreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    EntityStatusCreatedByEmail = table.Column<string>(type: "text", nullable: false),
                    EntityStatusCreatedByName = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntityStatusLastModifiedByUserId = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModifiedByEmail = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModifiedByName = table.Column<string>(type: "text", nullable: false),
                    OtherProperty = table.Column<string>(type: "text", nullable: false),
                    OtherProperty1 = table.Column<string>(type: "text", nullable: false),
                    IsGlobal = table.Column<bool>(type: "boolean", nullable: false),
                    WorkflowDefinition = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityWorkflowSteps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Imports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ImportType = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    ContentType = table.Column<string>(type: "text", nullable: true),
                    TotalRows = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ProcessedRows = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    SuccessfulImports = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    FailedImports = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Percentage = table.Column<double>(type: "numeric(5,2)", nullable: false, defaultValue: 0.0),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    BatchesQueued = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    BatchesSaved = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    BatchesFailed = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    SyncSucceededCount = table.Column<int>(type: "integer", nullable: true),
                    SyncFailedCount = table.Column<int>(type: "integer", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    CreatedByEmail = table.Column<string>(type: "text", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedByUserId = table.Column<string>(type: "text", nullable: false),
                    LastModifiedByEmail = table.Column<string>(type: "text", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataStatus = table.Column<string>(type: "text", nullable: true),
                    PreviousDataStatus = table.Column<string>(type: "text", nullable: true),
                    CustomFields = table.Column<string>(type: "jsonb", nullable: true),
                    EntityStatusCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntityStatusCreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    EntityStatusCreatedByEmail = table.Column<string>(type: "text", nullable: false),
                    EntityStatusCreatedByName = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntityStatusLastModifiedByUserId = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModifiedByEmail = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModifiedByName = table.Column<string>(type: "text", nullable: false),
                    OtherProperty = table.Column<string>(type: "text", nullable: false),
                    OtherProperty1 = table.Column<string>(type: "text", nullable: false),
                    IsGlobal = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    EventType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Transport = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityVersion = table.Column<long>(type: "bigint", nullable: false),
                    Payload = table.Column<string>(type: "jsonb", nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    NextAttemptAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LockedUntil = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Attempts = table.Column<int>(type: "integer", nullable: false),
                    LastError = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ProcessedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeadAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProxyCommands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetService = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Route = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Method = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PayloadCiphertext = table.Column<string>(type: "text", nullable: false),
                    RequestedByUserId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AcceptedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailureCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    CreatedByEmail = table.Column<string>(type: "text", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedByUserId = table.Column<string>(type: "text", nullable: false),
                    LastModifiedByEmail = table.Column<string>(type: "text", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataStatus = table.Column<string>(type: "text", nullable: true),
                    PreviousDataStatus = table.Column<string>(type: "text", nullable: true),
                    CustomFields = table.Column<Dictionary<string, string>>(type: "hstore", nullable: true),
                    EntityStatusCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntityStatusCreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    EntityStatusCreatedByEmail = table.Column<string>(type: "text", nullable: false),
                    EntityStatusCreatedByName = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntityStatusLastModifiedByUserId = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModifiedByEmail = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModifiedByName = table.Column<string>(type: "text", nullable: false),
                    OtherProperty = table.Column<string>(type: "text", nullable: false),
                    OtherProperty1 = table.Column<string>(type: "text", nullable: false),
                    IsGlobal = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProxyCommands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantPluginFlags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PluginKey = table.Column<string>(type: "text", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    EnabledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DisabledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    CreatedByEmail = table.Column<string>(type: "text", nullable: false),
                    CreatedByName = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedByUserId = table.Column<string>(type: "text", nullable: false),
                    LastModifiedByEmail = table.Column<string>(type: "text", nullable: false),
                    LastModifiedByName = table.Column<string>(type: "text", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataStatus = table.Column<string>(type: "text", nullable: true),
                    PreviousDataStatus = table.Column<string>(type: "text", nullable: true),
                    CustomFields = table.Column<string>(type: "jsonb", nullable: true),
                    EntityStatusCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntityStatusCreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    EntityStatusCreatedByEmail = table.Column<string>(type: "text", nullable: false),
                    EntityStatusCreatedByName = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntityStatusLastModifiedByUserId = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModifiedByEmail = table.Column<string>(type: "text", nullable: false),
                    EntityStatusLastModifiedByName = table.Column<string>(type: "text", nullable: false),
                    OtherProperty = table.Column<string>(type: "text", nullable: false),
                    OtherProperty1 = table.Column<string>(type: "text", nullable: false),
                    IsGlobal = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantPluginFlags", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_Category",
                table: "AppSettings",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_Created",
                table: "AppSettings",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_DataStatus",
                table: "AppSettings",
                column: "DataStatus");

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_LastModified",
                table: "AppSettings",
                column: "LastModified");

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_TenantId_CompanyId_DataStatus",
                table: "AppSettings",
                columns: new[] { "TenantId", "CompanyId", "DataStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_TenantId_CompanyId_Key",
                table: "AppSettings",
                columns: new[] { "TenantId", "CompanyId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityCodeConfigs_Created",
                table: "EntityCodeConfigs",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_EntityCodeConfigs_DataStatus",
                table: "EntityCodeConfigs",
                column: "DataStatus");

            migrationBuilder.CreateIndex(
                name: "IX_EntityCodeConfigs_LastModified",
                table: "EntityCodeConfigs",
                column: "LastModified");

            migrationBuilder.CreateIndex(
                name: "IX_EntityCodeConfigs_TenantId_CompanyId_DataStatus",
                table: "EntityCodeConfigs",
                columns: new[] { "TenantId", "CompanyId", "DataStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityCodeConfigs_TenantId_CompanyId_EntityType",
                table: "EntityCodeConfigs",
                columns: new[] { "TenantId", "CompanyId", "EntityType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityCodeReservations_Created",
                table: "EntityCodeReservations",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_EntityCodeReservations_DataStatus",
                table: "EntityCodeReservations",
                column: "DataStatus");

            migrationBuilder.CreateIndex(
                name: "IX_EntityCodeReservations_LastModified",
                table: "EntityCodeReservations",
                column: "LastModified");

            migrationBuilder.CreateIndex(
                name: "IX_EntityCodeReservations_TenantId_CompanyId_DataStatus",
                table: "EntityCodeReservations",
                columns: new[] { "TenantId", "CompanyId", "DataStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityCodeReservations_TenantId_CompanyId_EntityType_Code",
                table: "EntityCodeReservations",
                columns: new[] { "TenantId", "CompanyId", "EntityType", "Code" },
                unique: true,
                filter: "\"DataStatus\" = 'Active'");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkflowSteps_Category",
                table: "EntityWorkflowSteps",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkflowSteps_Created",
                table: "EntityWorkflowSteps",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkflowSteps_DataStatus",
                table: "EntityWorkflowSteps",
                column: "DataStatus");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkflowSteps_DataStatus_IsActive",
                table: "EntityWorkflowSteps",
                columns: new[] { "DataStatus", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkflowSteps_EntityType",
                table: "EntityWorkflowSteps",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkflowSteps_EntityType_IsActive",
                table: "EntityWorkflowSteps",
                columns: new[] { "EntityType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkflowSteps_IsActive",
                table: "EntityWorkflowSteps",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkflowSteps_LastModified",
                table: "EntityWorkflowSteps",
                column: "LastModified");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkflowSteps_TenantId_CompanyId_DataStatus",
                table: "EntityWorkflowSteps",
                columns: new[] { "TenantId", "CompanyId", "DataStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkflowSteps_WorkflowCode",
                table: "EntityWorkflowSteps",
                column: "WorkflowCode");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkflowSteps_WorkflowCode_EntityType",
                table: "EntityWorkflowSteps",
                columns: new[] { "WorkflowCode", "EntityType" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkflowSteps_WorkflowCode_EntityType_IsActive",
                table: "EntityWorkflowSteps",
                columns: new[] { "WorkflowCode", "EntityType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkflowSteps_WorkflowCode_IsActive",
                table: "EntityWorkflowSteps",
                columns: new[] { "WorkflowCode", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Imports_Created",
                table: "Imports",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Imports_DataStatus",
                table: "Imports",
                column: "DataStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Imports_LastModified",
                table: "Imports",
                column: "LastModified");

            migrationBuilder.CreateIndex(
                name: "IX_Imports_TenantId_CompanyId_DataStatus",
                table: "Imports",
                columns: new[] { "TenantId", "CompanyId", "DataStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Imports_TenantId_Created",
                table: "Imports",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Imports_TenantId_ImportType",
                table: "Imports",
                columns: new[] { "TenantId", "ImportType" });

            migrationBuilder.CreateIndex(
                name: "IX_Imports_TenantId_Status",
                table: "Imports",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "ix_outbox_entity_order",
                table: "OutboxMessages",
                columns: new[] { "EntityId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "ix_outbox_ready",
                table: "OutboxMessages",
                column: "NextAttemptAt",
                filter: "\"ProcessedAt\" IS NULL AND \"DeadAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProxyCommands_TenantId_Status_AcceptedAtUtc",
                table: "ProxyCommands",
                columns: new[] { "TenantId", "Status", "AcceptedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantPluginFlags_Created",
                table: "TenantPluginFlags",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_TenantPluginFlags_DataStatus",
                table: "TenantPluginFlags",
                column: "DataStatus");

            migrationBuilder.CreateIndex(
                name: "IX_TenantPluginFlags_LastModified",
                table: "TenantPluginFlags",
                column: "LastModified");

            migrationBuilder.CreateIndex(
                name: "IX_TenantPluginFlags_TenantId_CompanyId_DataStatus",
                table: "TenantPluginFlags",
                columns: new[] { "TenantId", "CompanyId", "DataStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantPluginFlags_TenantId_PluginKey",
                table: "TenantPluginFlags",
                columns: new[] { "TenantId", "PluginKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "EntityCodeConfigs");

            migrationBuilder.DropTable(
                name: "EntityCodeReservations");

            migrationBuilder.DropTable(
                name: "EntityWorkflowSteps");

            migrationBuilder.DropTable(
                name: "Imports");

            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "ProxyCommands");

            migrationBuilder.DropTable(
                name: "TenantPluginFlags");
        }
    }
}
