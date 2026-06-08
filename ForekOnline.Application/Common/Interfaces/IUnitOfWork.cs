//     Copyright © Forek ICT.
// </copyright>
// Created By:      IF Oliphant (on IFOliphantPC)
// Created Date:    10/Aug/2024 12:31 PM
// Purpose:         Defines the Unit of work services

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for unit of work pattern, providing a way to group changes to be committed as a single transaction.
    /// </summary>
    public interface IUnitOfWork
    {
        #region Address
        /// <summary>
        /// Gets the Address repository.
        /// </summary>
        IAddress Address { get; }
        #endregion

        #region Application Cycle
        /// <summary>
        /// Gets the ApplicationCycle repository.
        /// </summary>
        IApplicationCycle ApplicationCycle { get; }
        #endregion

        #region ApplicationEvent
        /// <summary>
        /// Gets the ApplicationEvent repository.
        /// </summary>
        IApplicationEvent ApplicationEvent { get; }
        #endregion

        #region Application Submission Queue
        /// <summary>
        /// Gets the ApplicationSubmissionQueue repository.
        /// </summary>
        IApplicationSubmissionQueue ApplicationSubmissionQueue { get; }
        #endregion

        #region Assessments
        /// <summary>
        /// Gets the Assessments repository.
        /// </summary>
        IAssessments Assessments { get; }
        #endregion

        #region Assessment Attempts
        /// <summary>
        /// Gets the Assessments Attempts.
        /// </summary>
        IAssessmentAttempt AssessmentAttempts { get; }
        #endregion

        #region Assessment Attempts Answers
        /// <summary>
        /// Gets the Assessments Attempts Answers.
        /// </summary>
        IAssessmentAttemptAnswer AssessmentAttemptAnswer { get; }
        #endregion

        #region Assessments Questions
        /// <summary>
        /// Gets the Assessments Questions.
        /// </summary>
        IAssessmentQuestion AssessmentQuestions { get; }
        #endregion

        #region Assessments Questions Options
        /// <summary>
        /// Gets the Assessments Questions Options.
        /// </summary>
        IAssessmentQuestionOption AssessmentQuestionOptions { get; }
        #endregion

        #region Applications
        /// <summary>
        /// Gets the Application repository.
        /// </summary>
        IApplications Applications { get; }
        #endregion

        #region Background Job Queue
        /// <summary>
        /// Gets the BackgroundJobQueue repository.
        /// </summary>
        IBackgroundJobQueue BackgroundJobQueue { get; }
        #endregion

        #region Category
        /// <summary>
        /// Gets the Category repository.
        /// </summary>
        ICategory Category { get; }
        #endregion

        #region Company
        /// <summary>
        /// Gets the Company repository.
        /// </summary>
        ICompany Company { get; }
        #endregion

        #region Courses
        /// <summary>
        /// Gets the Course repository.
        /// </summary>
        ICourse Courses { get; }
        IRepository<CourseOption> CourseOptions { get; }
        IRepository<CourseOptionFee> CourseOptionFees { get; }
        #endregion

        #region ContactPerson
        /// <summary>
        /// Gets the ContactPerson repository.
        /// </summary>
        IContactPerson ContactPerson { get; }
        #endregion

        #region Document
        /// <summary>
        /// Gets the Document repository.
        /// </summary>
        IDocument Documents { get; }
        #endregion

        #region EmployeeContact
        /// <summary>
        /// Gets the EmployeeContact repository.
        /// </summary>
        IEmployeeContact EmployeeContact { get; }
        #endregion

        #region Embedded Assessment
        /// <summary>
        /// Gets the EmbeddedAssessment repository.
        /// </summary>
        IEmbeddedAssessment EmbeddedAssessment { get; }
        #endregion

        #region Evidence
        /// <summary>
        /// Gets the Evidence repository.
        /// </summary>
        IEvidence Evidence { get; }
        #endregion

        #region File
        /// <summary>
        /// Gets the Files repository.
        /// </summary>
        IFile Files { get; }
        #endregion

        #region File Storage
        /// <summary>
        /// Gets the File Storage repository.
        /// </summary>
        IFileStorage FileStorage { get; }
        #endregion

        #region Financial Clearance
        /// <summary>
        /// Gets the FinancialClearances repository.
        /// </summary>
        IRepository<FinancialClearance> FinancialClearances { get; }
        #endregion

        #region Guardian
        /// <summary>
        /// Gets the Guardian repository.
        /// </summary>
        IGuardian Guardian { get; }
        #endregion

        #region InAppNotification
        /// <summary>
        /// Gets the InAppNotification repository.
        /// </summary>
        IInAppNotification InAppNotifications { get; }
        #endregion

        #region WorkplaceModule
        /// <summary>
        /// Gets the Guardian repository.
        /// </summary>
        ILearnerWorkplaceModule WorkplaceModule { get; }
        #endregion

        #region Lesson Attendance
        /// <summary>
        /// Gets the LessonAttendance repository.
        /// </summary>
        ILessonAttendance LessonAttendance { get; }
        #endregion

        #region Lessons
        /// <summary>
        /// Gets the Lessons repository.
        /// </summary>
        ILessons Lessons { get; }
        #endregion

        #region LessonPlan
        /// <summary>
        /// Gets the LessonPlans repository.
        /// </summary>
        ILessonPlans LessonPlans { get; }
        #endregion

        #region License
        /// <summary>
        /// Gets the Licenses repository.
        /// </summary>
        ILicense License { get; }
        #endregion

        #region Material
        /// <summary>
        /// Gets the Material repository.
        /// </summary>
        IMaterial Material { get; }
        #endregion

        #region Medical
        /// <summary>
        /// Gets the Medical repository.
        /// </summary>
        IMedical Medical { get; }
        #endregion

        #region NotificationEvent
        /// <summary>
        /// Gets the NotificationEvent repository.
        /// </summary>
        INotification Notification { get; }
        #endregion

        #region NotificationContentBlock
        /// <summary>
        /// Gets the NotificationContentBlock repository.
        /// </summary>
        INotificationContentBlock NotificationContentBlock { get; }
        #endregion

        #region Online Applicant User
        /// <summary>
        /// Gets the OnlineApplicantUser repository.
        /// </summary>
        IOnlineApplicantUser OnlineApplicantUser { get; }
        #endregion

        #region Online Application
        /// <summary>
        /// Gets the OnlineApplication repository.
        /// </summary>
        IOnlineApplication OnlineApplication { get; }
        #endregion

        #region Payslip
        /// <summary>
        /// Gets the Payslip repository.
        /// </summary>
        IPayslip Payslip { get; }
        #endregion

        #region Placement
        /// <summary>
        /// Gets the Placement repository.
        /// </summary>
        IPlacement Placement { get; }
        #endregion

        #region ForekModel
        /// <summary>
        /// Gets the ForekBaseModels repository.
        /// </summary>
        IForekBaseModels ForekModel { get; }
        #endregion

        #region Rejections
        /// <summary>
        /// Gets the ApplicationRejection repository.
        /// </summary>
        IApplicationRejection Rejections { get; }
        #endregion

        #region Reports
        /// <summary>
        /// Gets the Reports repository.
        /// </summary>
        IReports Reports { get; }
        #endregion

        #region Report + SubReports
        /// <summary>
        /// Gets the Reports + SubReports repository.
        /// </summary>
        IReportSubReport ReportSubReport { get; }
        #endregion

        #region Resource
        /// <summary>
        /// Gets the Resource repository.
        /// </summary>
        IResource Resource { get; }
        #endregion

        #region Stored Document
        /// <summary>
        /// Gets the StoredDocument repository.
        /// </summary>
        IStoredDocument StoredDocument { get; }
        #endregion

        #region Stored Document Content
        /// <summary>
        /// Gets the StoredDocumentContent repository.
        /// </summary>
        IStoredDocumentContent StoredDocumentContent { get; } 
        #endregion

        #region StudentAttachment
        /// <summary>
        /// Gets the StudentAttachment repository.
        /// </summary>
        IStudentAttachment StudentAttachment { get; }
        #endregion

        #region Training
        /// <summary>
        /// Gets the Training repository.
        /// </summary>
        ITraining Training { get; }
        #endregion

        #region Modules
        /// <summary>
        /// Gets the Module repository.
        /// </summary>
        IModule Modules { get; }
        #endregion

        #region Users
        /// <summary>
        /// Gets the Users repository.
        /// </summary>
        IUsers Users { get; }
        #endregion

        #region Login History
        /// <summary>
        /// Gets the User Login History Repository.
        /// </summary>
        IUserLoginHistories UserLoginHistories { get; }
        #endregion

        #region Venue
        /// <summary>
        /// Gets the Venue repository.
        /// </summary>
        IVenue Venues { get; }
        #endregion

        #region Venue Reservation
        /// <summary>
        /// Gets the VenueReservation repository.
        /// </summary>
        IVenueReservation VenueReservations { get; }
        #endregion

        #region Venue Reservation Audit
        /// <summary>
        /// Gets the VenueReservationAudit repository.
        /// </summary>
        IVenueReservationAudit VenueReservationAudits { get; }
        #endregion

        #region Venue Assessment Booking
        /// <summary>
        /// Gets the VenueAssessmentBooking repository.
        /// </summary>
        IVenueAssessmentBooking VenueAssessmentBookings { get; }
        #endregion

        #region Visit
        /// <summary>
        /// Gets the Visit repository.
        /// </summary>
        IVisit Visit { get; }
        #endregion

        #region FoStudent
        /// <summary>
        /// Gets the ForekOnline-owned Student repository (SQL Server).
        /// </summary>
        IRepository<StudentEntity> Students { get; }
        #endregion

        #region FoEnrollment
        /// <summary>
        /// Gets the ForekOnline-owned Enrollment repository (SQL Server).
        /// </summary>
        IRepository<EnrollmentEntity> Enrollments { get; }
        #endregion

        #region Save
        Task<int> SaveAsync();
        #endregion
    }
}
