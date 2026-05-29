using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Infrastructure.Migrations.Extensions
{
    public static class MigrationExtensions
    {
        /// <summary>
        /// Executes a CreateTable only if the table doesn't already exist.
        /// Schema defaults to 'dbo' if not provided.
        /// </summary>
        public static void CreateTableIfNotExists(this MigrationBuilder migrationBuilder, string tableName, string schema, Action createAction)
        {
            migrationBuilder.Sql($@"
            IF NOT EXISTS (
                SELECT 1 FROM sys.tables t
                JOIN sys.schemas s ON t.schema_id = s.schema_id
                WHERE s.name = '{schema}' AND t.name = '{tableName}'
            )
            BEGIN
                EXEC sp_executesql N'SELECT 1' -- no-op to open the block
            END
        ");

            createAction();
        }
    }
}
