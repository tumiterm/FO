// <copyright file="EnrollmentStatusTest.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

using ForekOnline.Domain.Entities;
using Xunit;

namespace Forek.Test
{
    public class EnrollmentStatusTest
    {
        [Theory]
        [InlineData("Active", EnrollmentStatus.Active)]
        [InlineData(" in progress ", EnrollmentStatus.Active)]
        [InlineData("Registered", EnrollmentStatus.Active)]
        [InlineData("complete", EnrollmentStatus.Completed)]
        [InlineData("Withdrawn", EnrollmentStatus.DroppedOut)]
        [InlineData("Cancelled", EnrollmentStatus.DroppedOut)]
        [InlineData("suspended", EnrollmentStatus.Suspended)]
        public void Normalize_KnownExternalValue_ReturnsConstraintValue(string source, string expected)
        {
            var result = EnrollmentStatus.Normalize(source, true, null);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Normalize_UnknownCompletedEnrollment_ReturnsCompleted()
        {
            var result = EnrollmentStatus.Normalize("Unexpected API value", false, DateTime.UtcNow);

            Assert.Equal(EnrollmentStatus.Completed, result);
        }

        [Theory]
        [InlineData(true, EnrollmentStatus.Active)]
        [InlineData(false, EnrollmentStatus.DroppedOut)]
        public void Normalize_UnknownValue_UsesActiveFlag(bool isActive, string expected)
        {
            var result = EnrollmentStatus.Normalize("Unexpected API value", isActive, null);

            Assert.Equal(expected, result);
        }
    }
}
