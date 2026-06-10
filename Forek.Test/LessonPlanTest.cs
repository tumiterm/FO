using System;
using ForekOnline.Domain.Entities;
using Xunit;

namespace Forek.Test
{
    public class LessonPlanTest
    {
        private static readonly DateTimeOffset Now = new(2026, 6, 10, 12, 0, 0, TimeSpan.Zero);

        [Fact]
        public void IsNewAt_ReturnsTrue_WhenPlanIsLessThanOneDayOld()
        {
            LessonPlan plan = new() { CreatedOn = Now.AddHours(-23).ToString("O") };

            Assert.True(plan.IsNewAt(Now));
        }

        [Fact]
        public void IsNewAt_ReturnsFalse_WhenPlanIsExactlyOneDayOld()
        {
            LessonPlan plan = new() { CreatedOn = Now.AddDays(-1).ToString("O") };

            Assert.False(plan.IsNewAt(Now));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("not-a-date")]
        public void IsNewAt_ReturnsFalse_WhenCreationDateIsUnavailable(string? createdOn)
        {
            LessonPlan plan = new() { CreatedOn = createdOn };

            Assert.False(plan.IsNewAt(Now));
        }

        [Fact]
        public void IsNewAt_ReturnsFalse_WhenCreationDateIsInTheFuture()
        {
            LessonPlan plan = new() { CreatedOn = Now.AddMinutes(1).ToString("O") };

            Assert.False(plan.IsNewAt(Now));
        }
    }
}
