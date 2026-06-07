using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;

namespace Forek.Test;

public class FacilitatorLearnerManagementModelTest
{
    [Fact]
    public void FacilitatorStudentLink_DoesNotEnforceStudentUniquenessAcrossFacilitators()
    {
        using var db = CreateContext();
        var entity = db.Model.FindEntityType(typeof(FacilitatorStudentLink));
        var indexes = entity!.GetIndexes().ToList();

        Assert.DoesNotContain(indexes, index => index.IsUnique && index.Properties.Select(p => p.Name).SequenceEqual([nameof(FacilitatorStudentLink.StudentId)]));
        Assert.DoesNotContain(indexes, index => index.IsUnique && index.Properties.Select(p => p.Name).SequenceEqual([nameof(FacilitatorStudentLink.FacilitatorId), nameof(FacilitatorStudentLink.StudentId)]));
    }

    [Fact]
    public void GroupMembership_IsUniqueWithinAGroup()
    {
        using var db = CreateContext();
        var entity = db.Model.FindEntityType(typeof(LearningGroupStudent));

        Assert.Contains(entity!.GetIndexes(), index => index.IsUnique
            && index.Properties.Select(p => p.Name).SequenceEqual([nameof(LearningGroupStudent.LearningGroupId), nameof(LearningGroupStudent.FacilitatorStudentLinkId)]));
    }

    [Fact]
    public void Attendance_DefaultsToNotMarkedAndDraft()
    {
        Assert.Equal(LearnerAttendanceStatus.NotMarked, new LearnerAttendanceRecord().AttendanceStatus);
        Assert.Equal(AttendanceSessionStatus.Draft, new LearnerAttendanceSession().Status);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;
        return new ApplicationDbContext(options);
    }
}
