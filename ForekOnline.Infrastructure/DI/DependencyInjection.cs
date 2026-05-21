// <copyright file="DependencyInjection.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    15-03-2026 10:51 AM
// Purpose:         Defines the DependencyInjection class.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
using ForekOnline.Infrastructure.Interceptors;
using ForekOnline.Infrastructure.Repository;
using ForekOnline.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
#endregion

namespace ForekOnline.Infrastructure.DI
{
    /// <summary>
    /// Provides extension methods for registering Forek infrastructure services and dependencies with the application's
    /// dependency injection container.
    /// </summary>
    /// <remarks>This static class contains methods that extend IServiceCollection to configure database
    /// contexts, repositories, services, and other infrastructure components required by the Forek application. These
    /// methods are intended to be called during application startup to ensure all necessary services are registered for
    /// dependency injection.</remarks>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds Forek infrastructure services, including database contexts, repositories, and application services, to
        /// the specified service collection.
        /// </summary>
        /// <remarks>This method registers both SQL Server and SQLite database contexts, as well as
        /// various scoped services required by the Forek application. When isAPI is false, additional services and a
        /// hosted background service are registered to support non-API scenarios. Call this method during application
        /// startup to ensure all required infrastructure components are available for dependency injection.</remarks>
        /// <param name="services">The service collection to which the infrastructure services will be added.</param>
        /// <param name="configuration">The application configuration used to retrieve connection strings and other settings required for service
        /// registration.</param>
        /// <param name="isAPI">true to configure services for an API application; false to include additional services and hosted
        /// background tasks intended for non-API scenarios.</param>
        /// <returns>The same IServiceCollection instance with Forek infrastructure services registered. This enables method
        /// chaining.</returns>
        public static IServiceCollection AddForekInfrastructure(this IServiceCollection services, IConfiguration configuration, bool isAPI)
        {
            services.AddScoped<AuditableEntityInterceptor>();

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

                options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());

#if DEBUG
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.LogTo(Console.WriteLine, LogLevel.Information);
#endif
            });

            var configuredPath = configuration.GetValue<string>("AppSettings:StudentCacheDbPath");
            var sqlitePath = string.IsNullOrWhiteSpace(configuredPath)
                ? Path.Combine(AppContext.BaseDirectory, "student_cache.db")
                : Path.IsPathRooted(configuredPath)
                    ? configuredPath
                    : Path.Combine(AppContext.BaseDirectory, configuredPath);

            services.AddDbContext<StudentCacheDbContext>(options =>
            {
                options.UseSqlite($"Data Source={sqlitePath}");
            });

            services.AddScoped<IStudentCacheRepository, StudentCacheRepository>();
            services.AddScoped<IStudentCacheStore, StudentCacheStore>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IPayslipRequestService, PayslipRequestService>();
            services.AddScoped<IHelperService, HelperService>();
            services.AddScoped<IBlobFileService, BlobFileService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IApplicationsService, ApplicationsService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<IVenueBookingService, VenueBookingService>();

            services.AddScoped<IStudentDataSource, ApiStudentDataSource>();
            services.AddScoped<IStudentDataSource, SqliteStudentDataSource>();

            services.AddScoped<IStudentImportJob, StudentImportJob>();

            services.AddScoped<EnrollmentOrchestrationService>();
            services.AddScoped<IInAppNotificationService, InAppNotificationService>();
            services.AddScoped<ILoginHistoryService, LoginHistoryService>();

            services.AddScoped<IRepository<TenantProfile>, Repository<TenantProfile>>();
            services.AddScoped<ITenantProfileService, TenantProfileService>();
            services.AddScoped<IRepository<TenantSubscription>, Repository<TenantSubscription>>();
            services.AddScoped<ITenantSubscriptionService, TenantSubscriptionService>();
            services.AddScoped<IReportComplianceService, ReportComplianceService>();
            services.AddScoped<IPlacementService, PlacementService>();
            services.AddScoped<IVisitService, VisitService>();
            services.AddMemoryCache();

            if (!isAPI)
            {
                services.AddScoped<IStudentService, StudentService>();
                services.AddHostedService<StudentListCacheWarmupService>();
            }

            services.AddHttpClient();
            services.AddHttpContextAccessor();

            return services;
        }
    }
}