
#region Usings
using ElecPOE.Middleware;
using ElecPOE.Profiles;
using ElecPOE.Renderers;
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Jobs;
using ForekOnline.Application.Common.Jobs.Handlers;
using ForekOnline.Application.Common.Middleware;
using ForekOnline.Application.Common.Services;
using ForekOnline.Infrastructure.Data;
using ForekOnline.Infrastructure.DI;
using ForekOnline.Infrastructure.HostedServices;
using ForekOnline.Infrastructure.Repository;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
#endregion

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
{
    option.LoginPath = "/Auth/SignIn";

    option.Cookie.Name = "POEAppCookie";
});

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}",optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddAutoMapper(typeof(MappingProfile));

Console.WriteLine(builder.Environment.EnvironmentName);
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NMaF5cXmBCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWX5ccHRURmheUUJ0V0c=");
//Ngo9BigBOggjHTQxAR8/V1NHaF5cWWdCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH1ceHRXRmVZVkF3XERWYUo=

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PlatformAdmin", policy => policy.RequireRole("SuperAdmin").RequireClaim("PlatformAdmin", "true"));
});

builder.Services.AddSession(s =>
{
    s.IdleTimeout = TimeSpan.FromHours(10);
});

builder.Services.AddForekInfrastructure(builder.Configuration, isAPI: false);

#region Hangfire
var hangfireConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddHangfire(cfg =>
{
    cfg.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
    cfg.UseSimpleAssemblyNameTypeSerializer();
    cfg.UseRecommendedSerializerSettings();

    cfg.UseSqlServerStorage(hangfireConnection, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.FromSeconds(10),
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    });
});

var workerCount = builder.Configuration.GetValue<int?>("Hangfire:WorkerCount") ?? Math.Max(Environment.ProcessorCount, 4);
var serverName = builder.Configuration.GetValue<string>("Hangfire:ServerName") ?? "ElecPOE";
var queues = builder.Configuration.GetSection("Hangfire:Queues").Get<string[]>() ?? ["default", "onlineapps"];

builder.Services.AddHangfireServer(options =>
{
    options.ServerName = serverName;
    options.WorkerCount = workerCount;
    options.Queues = queues;
});
#endregion

#region Services
builder.Services.AddScoped<IApplicantUserService, ApplicantUserService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFileStorageSettingsService, FileStorageSettingsService>();
builder.Services.AddScoped<IPayslipRequestService, PayslipRequestService>();
builder.Services.AddScoped<IHelperService, HelperService>();
builder.Services.AddScoped<IBlobFileService, BlobFileService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IApplicationsService, ApplicationsService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IAssessmentService, AssessmentService>();
builder.Services.AddHostedService<StudentListCacheWarmupService>();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IFileStorageProviderResolver, FileStorageProviderResolver>();
builder.Services.AddScoped<IStoredDocumentLookup, StoredDocumentLookup>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IVisitService, VisitService>();
builder.Services.AddScoped<IFileStorageProvider, AzureBlobFileStorageProvider>();
builder.Services.AddScoped<IFileStorageProvider, DatabaseFileStorageProvider>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddHostedService<UserSessionCleanupHostedService>();
//builder.Services.AddScoped<IApplicationSubmissionService, ApplicationSubmissionService>();
builder.Services.AddScoped<IOnlineApplicationsService, OnlineApplicationsService>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
builder.Services.AddScoped<IPdfGenerator, SelectPdfGenerator>();
builder.Services.AddScoped<IPdfReportService, PdfReportService>();
builder.Services.AddScoped<IApplicationCycleService, ApplicationCycleService>();
builder.Services.AddScoped<IBackgroundJobHandler, OnlineApplicationSubmissionHandler>();
builder.Services.AddScoped<BackgroundJobQueueDrainJob>();
#endregion

var app = builder.Build();

if (app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
{
    using var migrationScope = app.Services.CreateScope();
    var applicationDb = migrationScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await applicationDb.Database.MigrateAsync();
}

using (var scope = app.Services.CreateScope())
{
    var cacheDb = scope.ServiceProvider.GetRequiredService<StudentCacheDbContext>();

    var connectionString = cacheDb.Database.GetConnectionString();
    var dataSource = connectionString?.Split("Data Source=").LastOrDefault()?.Trim();
    if (!string.IsNullOrEmpty(dataSource))
    {
        var directory = Path.GetDirectoryName(dataSource);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
    }

    await cacheDb.Database.EnsureCreatedAsync();
}

using (var scope = app.Services.CreateScope())
{
    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    await FileStorageSettingsSeeder.SeedAsync(unitOfWork, CancellationToken.None);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<TenantResolutionMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<TenantSubscriptionMiddleware>(); 

app.UseSession();

app.UseCookiePolicy();

#region Hangfire Dashboard
var dashboardEnabled = app.Configuration.GetValue<bool?>("Hangfire:Dashboard:Enabled") ?? true;
var dashboardRoute = app.Configuration.GetValue<string>("Hangfire:Dashboard:Route") ?? "/hangfire";

if (dashboardEnabled)
{
    app.UseHangfireDashboard(dashboardRoute, new DashboardOptions
    {
        Authorization = new[] { new HangfireDashboardAuthorizationFilter(app.Environment) }
    });
}
#endregion

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapControllerRoute(

    name: "default",

    pattern: "{controller=Home}/{action=ForekSystemsHub}/{id?}");

RecurringJob.AddOrUpdate<BackgroundJobQueueDrainJob>(
    recurringJobId: "background-queue-onlineapps-drain",
    methodCall: job => job.DrainAsync("onlineapps", CancellationToken.None),
    cronExpression: "*/1 * * * *",
    options: new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

app.Run();

internal sealed class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly IWebHostEnvironment _env;

    public HangfireDashboardAuthorizationFilter(IWebHostEnvironment env)
    {
        _env = env;
    }

    public bool Authorize(DashboardContext context)
    {
        var http = context.GetHttpContext();

        if (_env.IsDevelopment() && http.Connection.RemoteIpAddress != null)
        {
            return true;
        }

        return http.User?.Identity?.IsAuthenticated == true
               && (http.User.IsInRole("Admin") || http.User.IsInRole("SuperAdmin"));
    }
}



