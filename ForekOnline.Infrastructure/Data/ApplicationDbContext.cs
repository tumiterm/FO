// <copyright file="ApplicationDbContext.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    07/01/2024 20:44:27 PM
// Purpose:         Defines the ApplicationDbContext class

#region Usings
using ForekOnline.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static ForekOnline.Domain.Enums.EnumRegistry;
using File = ForekOnline.Domain.Entities.File;
#endregion

namespace ForekOnline.Infrastructure.Data
{
    /// <summary>
    /// Represents the database context for the application.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class with specified options.
        /// </summary>
        /// <param name="options">The options to configure the database context.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        #region DbSets

        #region Venues
        public DbSet<Venue> Venues { get; set; }
        public DbSet<VenueReservation> VenueReservations { get; set; }
        public DbSet<VenueReservationAudit> VenueReservationAudits { get; set; }
        public DbSet<VenueAssessmentBooking> VenueAssessmentBookings { get; set; }
        #endregion
        public DbSet<TenantProfile> TenantProfiles { get; set; }
        public DbSet<TenantSubscription> TenantSubscriptions { get; set; }
        public DbSet<InAppNotification> InAppNotifications { get; set; }
        public DbSet<FinancialClearance> FinancialClearance { get; set; }
        public DbSet<EnrollmentEntity> Enrollments { get; set; }
        public DbSet<StudentEntity> Students { get; set; }
        public DbSet<ReportSubReport> ReportSubReport { get; set; }
        public DbSet<BackgroundJobQueueItem> BackgroundJobQueueItems { get; set; }
        public DbSet<ApplicationSubmissionQueue> ApplicationSubmissionQueue { get; set; }
        public DbSet<ApplicationRating> ApplicationRatings { get; set; }
        public DbSet<ResourceRoleAudience> ResourceRoleAudiences { get; set; }
        public DbSet<ResourceUserAudience> ResourceUserAudiences { get; set; }

        /// <summary>
        /// Gets or sets the collection of application cycles in the database.
        /// </summary>
        public DbSet<ApplicationCycle> ApplicationCycles { get; set; }

        /// <summary>
        /// Gets or sets the collection of online application entities for the context.
        /// </summary>
        /// <remarks>This property is used to query and save instances of <see cref="OnlineApplication"/>
        /// in the database. Use LINQ queries to retrieve or manipulate online application records.</remarks>
        public DbSet<OnlineApplication> OnlineApplications { get; set; }

        /// <summary>
        /// Gets or sets the collection of online application users in the database.
        /// </summary>
        public DbSet<OnlineApplicationUser> OnlineApplicationUsers { get; set; }

        /// <summary>
        /// Gets or sets the collection of lesson attendance records in the context.
        /// </summary>
        public DbSet<LessonAttendance> LessonAttendances { get; set; }

        /// <summary>
        /// Gets or sets the collection of lessons in the database context.
        /// </summary>
        public DbSet<Lesson> Lessons { get; set; }

        /// <summary>
        /// Gets or sets the collection of user login history records in the database.
        /// </summary>
        public DbSet<UserLoginHistory> UserLoginHistory { get; set; }

        /// <summary>
        /// Gets or sets the StorageSettings table.
        /// </summary>
        public DbSet<FileStorageSetting> StorageSettings { get; set; }

        /// <summary>
        /// Gets or sets the StoredDocuments table.
        /// </summary>
        public DbSet<StoredDocument> StoredDocuments { get; set; }

        /// <summary>
        /// Gets or sets the database set for <see cref="ApplicationEvent"/> entities.
        /// </summary>
        public DbSet<ApplicationEvent> ApplicationEvent { get; set; }

        /// <summary>
        /// Gets or sets the collection of assessment questions in the database context.
        /// </summary>
        public DbSet<AssessmentQuestion> AssessmentQuestions { get; set; }

        /// <summary>
        /// Gets or sets the collection of assessment question options.
        /// </summary>
        public DbSet<AssessmentQuestionOption> AssessmentQuestionOptions { get; set; }

        /// <summary>
        /// Gets or sets the collection of assessment attempts.
        /// </summary>
        public DbSet<AssessmentAttempt> AssessmentAttempts { get; set; }

        /// <summary>
        /// Gets or sets the collection of assessment attempt answers.
        /// </summary>
        public DbSet<AssessmentAttemptAnswer> AssessmentAttemptAnswers { get; set; }

        /// <summary>
        /// Gets or sets the collection of the StoredDocumentContents.
        /// </summary>
        public DbSet<StoredDocumentContent> StoredDocumentContents { get; set; }

        /// <summary>
        /// Gets or sets the Employee Contacts table.
        /// </summary>
        public DbSet<EmployeeContact> EmployeeContact { get; set; }

        /// <summary>
        /// Gets or sets the Student Attachments table.
        /// </summary>
        public DbSet<StudentAttachment> StudentAttachments { get; set; }

        /// <summary>
        /// Gets or sets the Assessment Attachments table.
        /// </summary>
        public DbSet<AssessmentAttachment> AssessmentAttachments { get; set; }

        /// <summary>
        /// Gets or sets the Material table.
        /// </summary>
        public DbSet<Material> Material { get; set; }

        /// <summary>
        /// Gets or sets the Trainings table.
        /// </summary>
        public DbSet<Training> Training { get; set; }

        /// <summary>
        /// Gets or sets the Companies table.
        /// </summary>
        public DbSet<Company> Company { get; set; }

        /// <summary>
        /// Gets or sets the Resources table.
        /// </summary>
        public DbSet<Resource> Resource { get; set; }

        /// <summary>
        /// Gets or sets the ResourceCategory table.
        /// </summary>
        public DbSet<ResourceCategory> ResourceCategory { get; set; }

        /// <summary>
        /// Gets or sets the Evidences table.
        /// </summary>
        public DbSet<Evidence> Evidence { get; set; }

        /// <summary>
        /// Gets or sets the Users table.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the Progress Reports table.
        /// </summary>
        public DbSet<ProgressReport> Results { get; set; }

        /// <summary>
        /// Gets or sets the Reports table.
        /// </summary>
        public DbSet<Report> Reports { get; set; }

        /// <summary>
        /// Gets or sets the Learner Finances table.
        /// </summary>
        public DbSet<LearnerFinance> Finance { get; set; }

        /// <summary>
        /// Gets or sets the Courses table.
        /// </summary>
        public DbSet<Course> Course { get; set; }

        /// <summary>
        /// Gets or sets the Modules table.
        /// </summary>
        public DbSet<Module> Module { get; set; }

        /// <summary>
        /// Gets or sets the Lesson Plans table.
        /// </summary>
        public DbSet<LessonPlan> LessonPlan { get; set; }

        /// <summary>
        /// Gets or sets the Placements table.
        /// </summary>
        public DbSet<Placement> Placements { get; set; }

        /// <summary>
        /// Gets or sets the Contact Persons table.
        /// </summary>
        public DbSet<ContactPerson> ContactPerson { get; set; }

        /// <summary>
        /// Gets or sets the Addresses table.
        /// </summary>
        public DbSet<Address> Address { get; set; }

        /// <summary>
        /// Gets or sets the Assessments table.
        /// </summary>
        public DbSet<Assessment> Assessments { get; set; }

        /// <summary>
        /// Gets or sets the Visits table.
        /// </summary>
        public DbSet<Visit> Visits { get; set; }

        /// <summary>
        /// Gets or sets the WeeklyTimesheets table.
        /// </summary>
        public DbSet<WeeklyTimesheet> WeeklyTimesheets { get; set; }

        /// <summary>
        /// Gets or sets the WorkplaceModules table.
        /// </summary>
        public DbSet<LearnerWorkplaceModules> WorkplaceModules { get; set; }

        /// <summary>
        /// Gets or sets the Application table.
        /// </summary>
        public DbSet<Domain.Entities.Application> Applications { get; set; }


        /// <summary>
        /// Gets or sets the ForekBase table.
        /// </summary>
        public DbSet<ForekBaseModel> ForekBase { get; set; }

        /// <summary>
        /// Gets or sets the Guardians table.
        /// </summary>
        public DbSet<Guardian> Guardians { get; set; }

        /// <summary>
        /// Gets or sets the Medicals table.
        /// </summary>
        public DbSet<Medical> Medicals { get; set; }

        /// <summary>
        /// Gets or sets the Files table.
        /// </summary>
        public DbSet<File> Files { get; set; }

        /// <summary>
        /// Gets or sets the Documents table.
        /// </summary>
        public DbSet<Document> Documents { get; set; }

        /// <summary>
        /// Gets or sets the Rejections table.
        /// </summary>
        public DbSet<ApplicationRejection> RejectionTBL { get; set; }

        /// <summary>
        /// Gets or sets the Licenses table.
        /// </summary>
        public DbSet<License> Licenses { get; set; }

        /// <summary>
        /// Gets or sets the Payslips table.
        /// </summary>
        public DbSet<PayslipRequest> PayslipRequest { get; set; }

        /// <summary>
        /// Gets the database set of notification events.
        /// </summary>
        /// <remarks>Use this property to perform CRUD operations on the <see cref="NotificationEvent"/>
        /// entities. Changes to the entities in this set are tracked by the context and can be persisted to the
        /// database when <see cref="DbContext.SaveChanges"/> is called.</remarks>
        public DbSet<NotificationEvent> NotificationEvents => Set<NotificationEvent>();

        /// <summary>
        /// Gets the database set of <see cref="NotificationContentBlock"/> entities.
        /// </summary>
        /// <remarks>This property provides access to the <see cref="NotificationContentBlock"/> entities
        /// for querying and saving changes to the database. Use LINQ queries to interact with the data.</remarks>
        public DbSet<NotificationContentBlock> NotificationContentBlocks => Set<NotificationContentBlock>();

        #endregion
      
        /// <summary>
        /// Configures the model for the database context by defining entity relationships, table mappings, and other
        /// model-level configurations.
        /// </summary>
        /// <remarks>This method is called during the initialization of the database context to customize
        /// the model.  It defines relationships between entities, such as the one-to-many relationship between <see
        /// cref="Course"/> and <see cref="Module"/>,  and applies additional configurations, such as associating the
        /// <see cref="Report"/> entity with a database trigger.</remarks>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/> used to configure the entity framework model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationRating>(entity =>
            {
                entity.ToTable(table =>
                    table.HasCheckConstraint("CK_ApplicationRating_Rating", "[Rating] BETWEEN 1 AND 5"));

                entity.HasIndex(rating => rating.ApplicationId)
                    .IsUnique();

                entity.HasOne(rating => rating.Application)
                    .WithMany()
                    .HasForeignKey(rating => rating.ApplicationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            #region Academics
            modelBuilder.Entity<StudentEntity>(entity =>
            {
                // ── Enum columns: live DB uses TINYINT, EF defaults to INT. ──────
                // HasConversion<byte>() makes EF call GetByte() instead of GetInt32()
                // so the InvalidCastException on read is eliminated without a migration.
                entity.Property(e => e.AdmissionCategory)
                    .HasConversion<byte>()
                    .HasColumnType("tinyint");

                entity.Property(e => e.Gender)
                    .HasConversion<byte>()
                    .HasColumnType("tinyint");

                // Province is nullable — use the nullable byte converter
                entity.Property(e => e.Province)
                    .HasConversion<byte?>()
                    .HasColumnType("tinyint");

                //entity.HasIndex(e => e.StudentNumber)
                //    .IsUnique()
                //    .HasDatabaseName("UX_Student_StudentNumber");

                //entity.HasIndex(e => e.IDNumber)
                //    .HasDatabaseName("IX_Student_IDNumber");

                //entity.HasIndex(e => e.PassportNumber)
                //    .HasDatabaseName("IX_Student_PassportNumber");

                //entity.HasIndex(e => e.Email)
                //    .HasDatabaseName("IX_Student_Email");

                entity.HasMany(e => e.Enrollments)
                    .WithOne(e => e.Student)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Documents)
                    .WithOne(d => d.Student)
                    .HasForeignKey("StudentEntityId");
            });

            modelBuilder.Entity<EnrollmentEntity>(entity =>
            {
                entity.HasIndex(e => new { e.StudentId, e.CourseId })
                    .IsUnique()
                    .HasDatabaseName("UX_Enrollment_Student_Course");

                entity.HasIndex(e => e.IsActive)
                    .HasDatabaseName("IX_Enrollment_IsActive");
            });
            #endregion

            modelBuilder.Entity<Course>()
            .HasMany(c => c.Module)
            .WithOne(m => m.Course)
            .HasForeignKey(m => m.CourseIdFK);

            modelBuilder.Entity<Report>()
                .ToTable(tb => tb.HasTrigger("trg_LogDeletedReports"));

            modelBuilder.Entity<UserLoginHistory>()
                .HasIndex(x => x.SessionKey)
                .IsUnique();

            modelBuilder.Entity<UserLoginHistory>()
                .HasIndex(x => x.LastActivityUtc);

            modelBuilder.Entity<UserLoginHistory>()
                .HasIndex(x => x.LogoutTimeUtc);

            ConfigureNotifications(modelBuilder);

            modelBuilder.Entity<Lesson>()
                 .HasIndex(x => x.RoomName);

            modelBuilder.Entity<LessonAttendance>()
                .HasIndex(x => new { x.LessonId, x.StudentId })
                .IsUnique();

            modelBuilder.Entity<WeeklyTimesheet>(entity =>
            {
                entity.HasIndex(e => new { e.PlacementId, e.WeekStartDate, e.WeekEndDate })
                    .HasDatabaseName("IX_WeeklyTimesheets_Placement_Week");

                entity.Property(e => e.TotalHours)
                    .HasColumnType("decimal(6,2)");

                entity.HasOne(e => e.Placement)
                    .WithMany(p => p.WeeklyTimesheets)
                    .HasForeignKey(e => e.PlacementId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Visit>(entity =>
            {
                entity.HasIndex(e => e.PlacementId)
                    .HasDatabaseName("IX_Visits_PlacementId");

                entity.HasOne(e => e.Placement)
                    .WithMany(p => p.Visits)
                    .HasForeignKey(e => e.PlacementId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ApplicationCycle>(entity =>
            {
                entity.Property(x => x.RowVersion)
                    .IsRowVersion();

                entity.HasIndex(x => x.AcademicYear);

                entity.HasIndex(x => new { x.AcademicYear, x.ApplicationPeriod })
                    .HasDatabaseName("IX_ApplicationCycle_Year_Period");

                entity.HasIndex(x => new { x.AcademicYear, x.ApplicationPeriod, x.IsActive })
                    .IsUnique()
                    .HasDatabaseName("UX_ApplicationCycle_Year_Period_Active")
                    .HasFilter("[IsActive] = 1 AND [IsDeleted] = 0");
            });

            modelBuilder.Entity<ResourceRoleAudience>()
                    .HasIndex(x => new { x.ResourceId, x.Role })
                    .IsUnique();

            modelBuilder.Entity<ResourceRoleAudience>()
                .HasOne(x => x.Resource)
                .WithMany(x => x.RoleAudiences)
                .HasForeignKey(x => x.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResourceUserAudience>()
                .HasIndex(x => new { x.ResourceId, x.UserId })
                .IsUnique();

            modelBuilder.Entity<ResourceUserAudience>()
                .HasOne(x => x.Resource)
                .WithMany(x => x.UserAudiences)
                .HasForeignKey(x => x.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResourceUserAudience>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BackgroundJobQueueItem>(entity =>
            {
                entity.Property(x => x.RowVersion).IsRowVersion();

                entity.HasIndex(x => new { x.Queue, x.Status, x.LockedUntilUtc })
                    .HasDatabaseName("IX_BackgroundJobQueueItem_Queue_Status_Lock");

                entity.HasIndex(x => x.DateCreated)
                    .HasDatabaseName("IX_BackgroundJobQueueItem_DateCreated");
            });

            modelBuilder.Entity<BackgroundJobQueueItem>(entity =>
            {
                entity.Property(x => x.RowVersion).IsRowVersion();

                entity.HasIndex(x => new { x.Queue, x.Status, x.LockedUntilUtc, x.DateCreated })
                    .HasDatabaseName("IX_BackgroundJobQueueItem_Q_Status_Lock_Created");

                entity.HasIndex(x => x.JobType)
                    .HasDatabaseName("IX_BackgroundJobQueueItem_JobType");
            });

            modelBuilder.Entity<ReportSubReport>(entity =>
            {
                entity.HasIndex(x => x.ReportId);

                entity.Property(x => x.FileName)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(x => x.FileId)
                    .IsRequired();
            });

            #region Venue
            modelBuilder.Entity<Venue>(entity =>
            {
                entity.HasIndex(e => new { e.Name, e.Campus })
                    .IsUnique()
                    .HasDatabaseName("UX_Venue_Name_Campus");

                entity.HasIndex(e => e.Status);
            });

            modelBuilder.Entity<VenueReservation>(entity =>
            {
                entity.Property(e => e.RowVersion).IsRowVersion();

                entity.HasIndex(e => new { e.VenueId, e.StartTimeUtc, e.EndTimeUtc })
                    .HasDatabaseName("IX_VenueReservation_Venue_TimeSlot");

                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.FacilitatorId);
                entity.HasIndex(e => e.ExpiresOnUtc);

                entity.HasOne(e => e.Venue)
                    .WithMany(v => v.Reservations)
                    .HasForeignKey(e => e.VenueId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<VenueReservationAudit>(entity =>
            {
                entity.HasOne(e => e.Reservation)
                    .WithMany(r => r.AuditLog)
                    .HasForeignKey(e => e.ReservationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<VenueAssessmentBooking>(entity =>
            {
                entity.HasIndex(e => e.ReservationId).IsUnique();

                entity.HasOne(e => e.Reservation)
                    .WithMany()
                    .HasForeignKey(e => e.ReservationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            modelBuilder.Entity<InAppNotification>(entity =>
            {
                entity.HasIndex(e => new { e.RecipientUserId, e.IsRead, e.DateCreated })
                    .HasDatabaseName("IX_InAppNotification_Recipient_Read_Created");
            });

            modelBuilder.Entity<TenantProfile>().HasData(new TenantProfile
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                AppTitle = "Forek Online",
                Tagline = "Powered By Forek ICT Services",
                LogoUrl = "/Theme/images/logob-removebg-preview.png",
                FaviconUrl = "/favicon.png",
                PrimaryColor = "#e65100",
                PrimaryColorLight = "#ff8a50",
                PrimaryColorDark = "#ac1900",
                AccentColor = "#ff9100",
                BackgroundColor = "#fafafa",
                TextColor = "#1a1a2e",
                PhysicalAddress = "Factory 107 Kabokweni, Industrial",
                ContactEmail = "info@forek.co.za",
                TwitterUrl = "https://twitter.com/forekinstitute",
                FacebookUrl = "https://www.facebook.com/ForekInstitute",
                InstagramUrl = "https://www.instagram.com/forekinstituteoftechnology/",
                YouTubeUrl = "https://www.youtube.com/@forekinstituteoftechnology2257",
                WebsiteUrl = "https://www.forek.co.za",
                CopyrightHolder = "Forek ICT Services",
                Name = "Default Tenant Profile",
                Code = "DEFAULT",
            });

            modelBuilder.Entity<TenantSubscription>(entity =>
            {
                entity.HasOne(e => e.TenantProfile)
                    .WithMany()
                    .HasForeignKey(e => e.TenantProfileId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.TenantProfileId, e.Status })
                    .HasDatabaseName("IX_TenantSubscription_Tenant_Status");

                entity.HasIndex(e => e.ExpiresOn)
                    .HasDatabaseName("IX_TenantSubscription_ExpiresOn");
            });

            modelBuilder.ApplyEntityConventions();

            // The deployed Academics.Students table predates the shared audit convention and
            // stores its audit timestamps as datetime2. Convert only this legacy entity so EF
            // reads DateTime values without trying to cast them directly to DateTimeOffset.
            var sastOffset = TimeSpan.FromHours(2);
            modelBuilder.Entity<StudentEntity>(entity =>
            {
                entity.Property(e => e.DateCreated)
                    .HasConversion(
                        value => value.ToOffset(sastOffset).DateTime,
                        value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Unspecified), sastOffset))
                    .HasColumnType("datetime2")
                    .HasDefaultValueSql("SYSDATETIME()");

                entity.Property(e => e.DateModified)
                    .HasConversion(
                        value => value.ToOffset(sastOffset).DateTime,
                        value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Unspecified), sastOffset))
                    .HasColumnType("datetime2")
                    .HasDefaultValueSql("SYSDATETIME()");

                entity.Property(e => e.DateDeleted)
                    .HasConversion(
                        value => value.HasValue ? value.Value.ToOffset(sastOffset).DateTime : (DateTime?)null,
                        value => value.HasValue
                            ? new DateTimeOffset(DateTime.SpecifyKind(value.Value, DateTimeKind.Unspecified), sastOffset)
                            : (DateTimeOffset?)null)
                    .HasColumnType("datetime2");
            });
        }

        /// <summary>
        /// Configures the entity relationships, property conversions, and seed data for the notification-related
        /// entities.
        /// </summary>
        /// <remarks>This method sets up the following configurations: <list type="bullet"> <item>
        /// <description>Defines a one-to-many relationship between <see cref="NotificationEvent"/> and <see
        /// cref="NotificationContentBlock"/> entities, with cascade delete behavior.</description> </item> <item>
        /// <description>Configures the <c>ListItems</c> property of <see cref="NotificationContentBlock"/> to use a
        /// custom string conversion for storage.</description> </item> <item> <description>Seeds initial data for <see
        /// cref="NotificationEvent"/> and <see cref="NotificationContentBlock"/> entities, including a sample
        /// notification event and its associated content blocks.</description> </item> </list></remarks>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/> instance used to configure the entity framework model.</param>
        protected void ConfigureNotifications(ModelBuilder modelBuilder)
        {
            var eventId = Guid.NewGuid();
            modelBuilder.Entity<NotificationEvent>().HasData(new NotificationEvent
            {
                Id = eventId,
                Title = "Forek Online Version 2 Highlights",
                HeaderIconCss = "fa fa-rocket",
                HeaderGradientCss = "var(--ap-grad)",
                HeaderTextColor = "#fff",
                ModalSize = eNotificationModalSize.Large,
                ImageUrl = "/Images/dancing.jpg",
                StartUtc = DateTime.Now.AddDays(-1),
                EndUtc = DateTime.Now.AddDays(7),
                DisplayOrder = 1,
                IsActive = true
            });

            modelBuilder.Entity<NotificationContentBlock>().HasData(
                new NotificationContentBlock
                {
                    Id = Guid.NewGuid(),
                    NotificationEventId = eventId,
                    Type = eNotificationContentType.Paragraph,
                    Text = "A sleeker, faster platform..."
                }
            );

            modelBuilder.Entity<NotificationEvent>()
                .HasMany(e => e.Blocks)
                .WithOne(b => b.NotificationEvent!)
                .HasForeignKey(b => b.NotificationEventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationContentBlock>()
                .Property(b => b.ListItems)
                .HasConversion(
                    v => string.Join("||", v ?? Array.Empty<string?>()),
                    v => v.Split("||", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

            var now = DateTime.UtcNow;
            modelBuilder.Entity<NotificationEvent>().HasData(new NotificationEvent
            {
                Id = Guid.Parse("59ebc1e4-31dc-4b87-91c8-39d49c298b64"),
                Title = "Forek Online Version 2 Highlights",
                HeaderIconCss = "fa fa-rocket",
                HeaderGradientCss = "var(--ap-grad)",
                HeaderTextColor = "#fff",
                ModalSize = eNotificationModalSize.Large,
                ImageUrl = "/Images/dancing.jpg",
                StartUtc = now.AddDays(-1),
                EndUtc = now.AddDays(7),
                DisplayOrder = 1,
                IsActive = true
            });

            modelBuilder.Entity<NotificationContentBlock>().HasData(
                new NotificationContentBlock
                {
                    Id = Guid.NewGuid(),
                    NotificationEventId = Guid.Parse("59ebc1e4-31dc-4b87-91c8-39d49c298b64"),
                    Type = eNotificationContentType.Paragraph,
                    Text = "A sleeker, faster platform with cohesive red → orange design language and improved UX consistency."
                },
                new NotificationContentBlock
                {
                    Id = Guid.NewGuid(),
                    NotificationEventId = Guid.Parse("59ebc1e4-31dc-4b87-91c8-39d49c298b64"),
                    Type = eNotificationContentType.UnorderedList,
                    ListItems = new[] {
                    "Unified visual system (Dark Red / Red / Orange)",
                    "Enhanced statistics & quick scan KPIs",
                    "Optimized markup & reduced layout shift",
                    "Modular section architecture for future widgets",
                    "Accessibility-focused contrast & semantics"
                    }
                },
                new NotificationContentBlock
                {
                    Id = Guid.NewGuid(),
                    NotificationEventId = Guid.Parse("59ebc1e4-31dc-4b87-91c8-39d49c298b64"),
                    Type = eNotificationContentType.Paragraph,
                    Text = "<div class='alert alert-warning'><i class='fa fa-triangle-exclamation me-2'></i>Please report anomalies to ICT so iterative refinements can ship rapidly.</div>"
                }
            );
        }
    }
}
