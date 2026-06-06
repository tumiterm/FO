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
            var sql = StudentImportSchemaService.EnsureGuardianIsOptionalSql;

            Assert.Contains("DECLARE @dropForeignKeySql nvarchar(max)", sql);
            Assert.Contains("QUOTENAME(@guardianForeignKey)", sql);
            Assert.Contains("EXEC sys.sp_executesql @dropForeignKeySql", sql);
            Assert.DoesNotContain("EXEC(N'ALTER TABLE", sql);
        }

        [Fact]
        public void CompatibilitySql_ReleasesApplicationLockOnSuccessAndFailure()
        {
            var sql = StudentImportSchemaService.EnsureGuardianIsOptionalSql;

            Assert.Contains("BEGIN TRY", sql);
            Assert.Contains("BEGIN CATCH", sql);
            Assert.Equal(2, CountOccurrences(sql, "EXEC sys.sp_releaseapplock"));
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
