using ElecPOE.Controllers;
using ElecPOE.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Forek.Test;

public sealed class TenantOnboardingViewModelTests
{
    [Fact]
    public void ToRequest_MapsBrandingSubscriptionAndAdminDetails()
    {
        var model = ValidModel();

        var request = model.ToRequest();

        Assert.Equal("example-college", request.Slug);
        Assert.Equal("portal.example.edu", request.HostName);
        Assert.Equal("#102030", request.PrimaryColor);
        Assert.Equal("#f08020", request.AccentColor);
        Assert.Equal("Africa/Johannesburg", request.TimeZoneId);
        Assert.Equal(75, request.MaxUsers);
        Assert.Equal(2500, request.MaxStudents);
        Assert.Equal(DateTimeOffset.Parse("2026-07-01T00:00:00+00:00"), request.StartsOn);
        Assert.Equal(DateTimeOffset.Parse("2027-07-01T00:00:00+00:00"), request.ExpiresOn);
    }

    [Fact]
    public void Validation_RejectsProtocolInHostAndInvalidSubscriptionRange()
    {
        var model = ValidModel();
        model.HostName = "https://portal.example.edu/path";
        model.ExpiresOn = model.StartsOn;

        var results = new List<ValidationResult>();
        var valid = Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);

        Assert.False(valid);
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(model.HostName)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(model.ExpiresOn)));
    }

    [Fact]
    public void Controller_IsRestrictedToPlatformAdministrators()
    {
        var attribute = Assert.Single(typeof(TenantOnboardingController)
            .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
            .Cast<AuthorizeAttribute>());

        Assert.Equal("PlatformAdmin", attribute.Policy);
    }

    private static TenantOnboardingViewModel ValidModel() => new()
    {
        LegalName = "Example College (Pty) Ltd",
        Slug = "example-college",
        AppTitle = "Example College Online",
        HostName = "portal.example.edu",
        ContactEmail = "support@example.edu",
        BillingContactEmail = "billing@example.edu",
        TimeZoneId = "Africa/Johannesburg",
        Culture = "en-ZA",
        PrimaryColor = "#102030",
        AccentColor = "#f08020",
        PlanName = "Professional",
        StartsOn = new DateTime(2026, 7, 1),
        ExpiresOn = new DateTime(2027, 7, 1),
        MaxUsers = 75,
        MaxStudents = 2500,
        AdminFirstName = "Tenant",
        AdminLastName = "Owner",
        AdminEmail = "admin@example.edu",
        AdminPassword = "A-strong-password-2026",
        ConfirmPassword = "A-strong-password-2026"
    };
}
