//     Copyright © Forek ICT.
// </copyright>
// Created By:      IF Oliphant (on IFOliphantPC)
// Created Date:    09/Jan/2024 21:00 PM
// Purpose:         Defines the Generic Repo class.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a unit of work for coordinating transactions and interactions with the database.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        #region Properties

        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// Gets the application event associated with the current context.
        /// </summary>
        public IApplicationEvent ApplicationEvent { get; private set; }

        /// <summary>
        /// Gets the application lifecycle manager used to control and monitor the application's execution state.
        /// </summary>
        public IApplicationCycle ApplicationCycle { get; private set; }

        /// <summary>
        /// Gets the queue used to submit applications for processing.
        /// </summary>
        public IApplicationSubmissionQueue ApplicationSubmissionQueue { get; private set; }

        /// <summary>
        /// Gets the queue used to manage and schedule background jobs for execution.
        /// </summary>
        /// <remarks>The background job queue allows enqueuing and tracking of tasks that are processed
        /// asynchronously. This property is typically used to add jobs for background processing or to monitor the
        /// status of queued jobs. Thread safety and job execution guarantees depend on the specific implementation of
        /// the <see cref="IBackgroundJobQueue"/> interface.</remarks>
        public IBackgroundJobQueue BackgroundJobQueue { get; private set; }

        /// <summary>
        /// Gets the model repository for handling address operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IAddress Address { get; private set; }

        /// <summary>
        /// Gets the collection of assessment questions associated with the current context.
        /// </summary>
        public IAssessmentQuestion AssessmentQuestions { get; private set; }

        /// <summary>
        /// Gets the options associated with an assessment question.
        /// </summary>
        public IAssessmentQuestionOption AssessmentQuestionOptions { get; private set; }

        /// <summary>
        /// Gets the current assessment attempt.
        /// </summary>
        public IAssessmentAttempt AssessmentAttempts { get; private set; }

        /// <summary>
        /// Gets the answer associated with the current assessment attempt.
        /// </summary>
        public IAssessmentAttemptAnswer AssessmentAttemptAnswer { get; private set; }

        /// <summary>
        /// Gets the embedded assessment associated with the current context.
        /// </summary>
        public IEmbeddedAssessment EmbeddedAssessment { get; private set; }

        /// <summary>
        /// Gets the model repository for handling forek-related operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IForekBaseModels ForekModel { get; private set; }

        /// <summary>
        /// Gets the model repository for handling forek-related operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IFile Files { get; private set; }

        /// <summary>
        /// Gets the model repository for handling File Storage operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IFileStorage FileStorage { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Category operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public ICategory Category { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Course operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public ICompany Company { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Course operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public ICourse Courses { get; private set; }
        public IRepository<CourseOption> CourseOptions { get; private set; }
        public IRepository<CourseOptionFee> CourseOptionFees { get; private set; }

        /// <summary>
        /// Gets the model repository for handling ContactPerson operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IContactPerson ContactPerson { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Documents operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IDocument Documents { get; private set; }

        /// <summary>
        /// Gets the model repository for handling EmployeeContact operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IEmployeeContact EmployeeContact { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Evidence operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IEvidence Evidence { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Guardian operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IGuardian Guardian { get; private set; }

        /// <summary>
        /// Gets the model repository for handling InAppNotification operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IInAppNotification InAppNotifications { get; private set; }

        /// <summary>
        /// Gets the model repository for handling LessonPlans operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public ILessonPlans LessonPlans { get; private set; }

        /// <summary>
        /// Gets the model repository for handling License operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public ILicense License { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Material operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IMaterial Material { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Medical operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IMedical Medical { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Modules operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IModule Modules { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Notification operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public INotification Notification { get; private set; }

        /// <summary>
        /// Gets the model repository for handling NotificationContentBlock operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public INotificationContentBlock NotificationContentBlock { get; private set; }

        /// <summary>
        /// Gets the online applicant user associated with the current context.
        /// </summary>
        public IOnlineApplicantUser OnlineApplicantUser { get; private set; }

        /// <summary>
        /// Gets the online application instance associated with this object.
        /// </summary>
        public IOnlineApplication OnlineApplication { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Placement operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IPlacement Placement { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Rejections operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IApplicationRejection Rejections { get; private set; }

        /// <summary>
        /// Gets the model repository for handling StoredDocument operations.
        /// </summary>
        /// <value>
        /// The model repository.
        public IStoredDocument StoredDocument { get; private set; }

        /// <summary>
        /// Gets the model repository for handling StoredDocument Content operations.
        /// </summary>
        /// <value>
        /// The model repository.
        public IStoredDocumentContent StoredDocumentContent { get; private set; }

        /// <summary>
        /// Gets the model repository for handling WorkplaceModule operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public ILearnerWorkplaceModule WorkplaceModule { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Course operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IAssessments Assessments { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Application operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IApplications Applications { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Payslip and IRP5 operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IPayslip Payslip { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Reports operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IReports Reports { get; private set; }

        /// <summary>
        /// Gets the model repository for handling ReportSubReport operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IReportSubReport ReportSubReport { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Resource operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IResource Resource { get; private set; }

        /// <summary>
        /// Gets the model repository for handling StudentAttachment operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IStudentAttachment StudentAttachment { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Training operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public ITraining Training { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Users operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IUsers Users { get; private set; }

        /// <summary>
        /// Gets the model repository for handling user login operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IUserLoginHistories UserLoginHistories { get; private set; }

        /// <summary>
        /// Gets the model repository for handling Visits operations.
        /// </summary>
        /// <value>
        /// The model repository.
        /// </value>
        public IVisit Visit { get; private set; }

        /// <summary>
        /// Gets the lesson attendance information associated with this instance.
        /// </summary>
        public ILessonAttendance LessonAttendance { get; private set; }

        /// <summary>
        /// Gets the collection of lessons available in the current context.
        /// </summary>
        public ILessons Lessons { get; private set; }

        #region Academics
        /// <summary>
        /// Gets the ForekOnline-owned Student repository (SQL Server, Academics schema).
        /// </summary>
        public IRepository<StudentEntity> Students { get; private set; }

        /// <summary>
        /// Gets the ForekOnline-owned Enrollment repository (SQL Server, Academics schema).
        /// </summary>
        public IRepository<EnrollmentEntity> Enrollments { get; private set; }
        #endregion

        public IRepository<FinancialClearance> FinancialClearances { get; }

        #region Venues
        public IVenue Venues { get; private set; }
        public IVenueReservation VenueReservations { get; private set; }
        public IVenueAssessmentBooking VenueAssessmentBookings { get; private set; }
        public IVenueReservationAudit VenueReservationAudits { get; private set; }
        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="db">The application database context.</param>
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Address = new AddressRepository(_db);
            ForekModel = new ForekBaseRepository(_db);
            Payslip = new PayslipRepository(_db);
            Applications = new ApplicationsRepository(_db);
            Assessments = new AssessmentsRepository(_db);
            Company = new CompanyRepository(_db);
            ContactPerson = new ContactPersonRepository(_db);
            Documents = new DocumentRepository(_db);
            Guardian = new GuardianRepository(_db);
            Evidence = new EvidenceRepository(_db);
            WorkplaceModule = new WorkplaceModulesRepository(_db);
            LessonPlans = new LessonPlanRepository(_db);
            License = new LicenseRepository(_db);
            Material = new MaterialRepository(_db);
            Medical = new MedicalRepository(_db);
            Placement = new PlacementRepository(_db);
            Rejections = new RejectionRepository(_db);
            Modules = new ModuleRepository(_db);
            Visit = new VisitRepository(_db);
            Users = new UserRepository(_db);
            Courses = new CourseRepository(_db);
            CourseOptions = new Repository<CourseOption>(_db);
            CourseOptionFees = new Repository<CourseOptionFee>(_db);
            Reports = new ReportRepository(_db);
            Files = new FileRepository(_db);
            StudentAttachment = new StudentAttachmentRepository(_db);
            Training = new TrainingRepository(_db);
            EmployeeContact = new EmployeeContactRepository(_db);
            Resource = new ResourceRepository(_db);
            Category = new CategoryRepository(_db);
            Notification =  new NotificationRepository(_db);
            NotificationContentBlock = new NotificationContentBlockRepository(_db);
            EmbeddedAssessment = new EmbeddedAssessmentsRepository(_db);
            AssessmentQuestions = new AssessmentQuestionRepository(_db);    
            AssessmentQuestionOptions = new AssessmentQuestionOptionRepository(_db);
            AssessmentAttempts = new AssessmentAttemptRepository(_db);
            AssessmentAttemptAnswer = new AssessmentAttemptAnswerRepository(_db);
            ApplicationEvent = new ApplicationEventRepository(_db);
            FileStorage = new FileStorageRepository(_db);
            StoredDocument = new StoredDocumentRepository(_db);
            StoredDocumentContent = new StoredDocumentContentRepository(_db);
            UserLoginHistories = new UserLoginHistoryRepository(_db);
            LessonAttendance = new LessonAttendanceRepository(_db);
            Lessons = new LessonRepository(_db);
            OnlineApplicantUser = new OnlineApplicantUserRepository(_db);
            ApplicationCycle = new ApplicationCycleRepository(_db);
            OnlineApplication = new OnlineApplicationRepository(_db);
            ApplicationSubmissionQueue = new ApplicationSubmissionQueueRepository(_db);
            BackgroundJobQueue = new BackgroundJobQueueRepository(_db);
            ReportSubReport = new ReportSubReportRepository(_db);
            Venues = new VenueRepository(_db);
            VenueReservations = new VenueReservationRepository(_db);
            VenueAssessmentBookings = new VenueAssessmentBookingRepository(_db);
            VenueReservationAudits = new VenueReservationAuditRepository(_db);
            Students = new Repository<StudentEntity>(_db);
            Enrollments = new Repository<EnrollmentEntity>(_db);
            FinancialClearances = new Repository<FinancialClearance>(_db);
            InAppNotifications = new InAppNotificationRepository(_db);
        }

        /// <summary>
        /// Saves changes made within the unit of work to the underlying database.
        /// </summary>
        public async Task<int> SaveAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
