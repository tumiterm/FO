// <copyright file="StudentImportSchemaServiceTest.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

using ForekOnline.Infrastructure.Data;
using Xunit;

namespace Forek.Test
{
    public class StudentImportSchemaServiceTest
    {
        [Fact]
        public void CompatibilitySql_BuildsDynamicConstraintCommandBeforeExecution()
        {
            var sql = StudentImportSchemaService.EnsureStudentImportSchemaSql;

            Assert.Contains("DECLARE @dropForeignKeySql nvarchar(max)", sql);
            Assert.Contains("QUOTENAME(@guardianForeignKey)", sql);
            Assert.Contains("EXEC sys.sp_executesql @dropForeignKeySql", sql);
            Assert.DoesNotContain("EXEC(N'ALTER TABLE", sql);
        }

        [Fact]
        public void CompatibilitySql_ReleasesApplicationLockOnSuccessAndFailure()
        {
            var sql = StudentImportSchemaService.EnsureStudentImportSchemaSql;

            Assert.Contains("BEGIN TRY", sql);
            Assert.Contains("BEGIN CATCH", sql);
            Assert.Equal(2, CountOccurrences(sql, "EXEC sys.sp_releaseapplock"));
        }

        [Fact]
        public void CompatibilitySql_RepointsLegacyEnrollmentForeignKeyToStudentEntityKey()
        {
            var sql = StudentImportSchemaService.EnsureStudentImportSchemaSql;

            Assert.Contains("referencedColumns.name = N'StudentId'", sql);
            Assert.Contains("SET enrollmentHistory.StudentId = students.Id", sql);
            Assert.Contains("REFERENCES [Academics].[Students] ([Id])", sql);
            Assert.Contains("FK_EnrollmentHistory_Students", sql);
        }

        [Fact]
        public void CompatibilitySql_OnlyAddsEnrollmentForeignKeyWhenTablesExist()
        {
            var sql = StudentImportSchemaService.EnsureStudentImportSchemaSql;

            Assert.Contains("OBJECT_ID(N'[Academics].[EnrollmentHistory]', N'U') IS NOT NULL", sql);
            Assert.Contains("OBJECT_ID(N'[Academics].[Students]', N'U') IS NOT NULL", sql);
            Assert.Contains("referencedColumns.name = N'Id'", sql);
        }

        private static int CountOccurrences(string source, string value)
        {
            var count = 0;
            var index = 0;

            while ((index = source.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += value.Length;
            }

            return count;
        }
    }
}
