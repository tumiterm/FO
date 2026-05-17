// <copyright file="IHelperService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    12/31/2024 14:09:27 PM
// Purpose:         Defines the IHelperService interface.

#region Usings
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Interface for helper methods used across the application.
    /// </summary>
    public interface IHelperService
    {
        /// <summary>
        /// Validates if the user object is not null. If null, logs the incident and redirects to the PageNotFound action.
        /// </summary>
        /// <typeparam name="T">The type of user object to validate.</typeparam>
        /// <param name="controller">The controller instance to handle redirection.</param>
        /// <param name="model">The model object to validate.</param>
        /// <param name="logger">The logger to record validation events.</param>
        /// <returns>An IActionResult, either null if model is valid or a redirection to the PageNotFound action if null.</returns>
        IActionResult ValidateModel<T>(Controller controller, T model, ILogger logger = null);

        /// <summary>
        /// Encrypts the specified value using SHA256 and encodes it in base64.
        /// </summary>
        /// <param name="value">The value to encrypt.</param>
        /// <returns>The encrypted value as a base64 string.</returns>
        string ValueEncryption(string value);

        /// <summary>
        /// Generates a new GUID.
        /// </summary>
        /// <returns>A new GUID.</returns>
        Guid GenerateGuid();

        /// <summary>
        /// Initializes and returns a new HttpClient with the base address set from configuration.
        /// </summary>
        /// <returns>A new HttpClient instance.</returns>
        HttpClient InitializeHttpClient();

        /// <summary>
        /// Gets the MIME type based on the file extension.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The MIME type of the file.</returns>
        string GetContentType(string path);

        /// <summary>
        /// Generates a random string of the specified size using alphanumeric characters.
        /// </summary>
        /// <param name="size">The size of the string to generate.</param>
        /// <returns>A randomly generated string.</returns>
        string GenerateRandomString(int size);

        /// <summary>
        /// Gets the current date and time formatted as a string.
        /// </summary>
        /// <returns>The current date and time as a string.</returns>
        string GetCurrentDateTime();

        /// <summary>
        /// Generates a formatted message string with the specified parameters.
        /// </summary>
        /// <param name="name">The name to include in the message.</param>
        /// <param name="type">The type of report.</param>
        /// <param name="date">The date of the report.</param>
        /// <param name="module">The module related to the report.</param>
        /// <param name="urgency">The urgency level of the report.</param>
        /// <returns>A formatted message string.</returns>
        string GenerateMessage(string name, string type, string date, string module, string urgency);


        /// <summary>
        /// Generates a notification string with the specified parameters.
        /// </summary>
        /// <param name="reference">The reference number of the report.</param>
        /// <param name="reportType">The type of report.</param>
        /// <param name="user">The user receiving the notification.</param>
        /// <param name="date">The date of the report.</param>
        /// <returns>A formatted notification string.</returns>
        string GenerateNotification(string reference, string reportType, string user, DateTime date);
        string OnSendRejectionEmail(string name, string course, string refNumber, string rejectionReason);

        string OnSendPendingEmail(string name, string course, string refNumber, string pendingReason);

        string OnSendApprovalEmailToTradeAndSkills(string name, string course, string refNumber);

        /// <summary>
        /// Sends a generic approval email for a specified course.
        /// </summary>
        /// <param name="name">The name of the recipient to whom the email will be sent.</param>
        /// <param name="course">The name of the course for which approval is being granted.</param>
        /// <param name="refNumber">The reference number associated with the approval request.</param>
        /// <returns>A string containing the status of the email sending operation.</returns>
        string OnSendGenericApprovalEmail(string name, string course, string refNumber);    

        string OnSendAptitudeTestInvitation(string name, string course, string refNumber, string testDateTime);

        /// <summary>
        /// Sends an email notification with the specified parameters.
        /// </summary>
        /// <param name="receiver">The receiver's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The body of the email.</param>
        /// <param name="header">The header to include in the email.</param>
        Task SendMailNotificationAsync(EmailDataViewModel email);

        /// <summary>
        /// Sends an SMS message to the specified recipient.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="recipientNo">The recipient's phone number.</param>
        void SendSms(string message, string recipientNo);

        /// <summary>
        /// Generates a JWT token asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the JWT token as a string.</returns>
        Task<string> GenerateJwtToken();

        /// <summary>
        /// Shows a notification using the specified title, text, and type.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="text">The text of the notification.</param>
        /// <param name="type">The type of the notification.</param>
        /// <returns>A formatted notification string.</returns>
        string ShowNotification(string title, string text, string type);

        /// <summary>
        /// Gets the display name of the specified enum value.
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>The display name of the enum value.</returns>
        string GetDisplayName(Enum enumValue);

        /// <summary>
        /// Maps properties from the source object to a new target object of type TTarget.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TTarget">The type of the target object.</typeparam>
        /// <param name="source">The source object.</param>
        /// <returns>A new target object with mapped properties.</returns>
        TTarget MapProperties<TSource, TTarget>(TSource source) where TTarget : new();

        /// <summary>
        /// Gets a list of students asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of students.</returns>
        Task<List<Student>> GetStudentListAsync();

        /// <summary>
        /// Retrieves a student's details using their student number.
        /// </summary>
        /// <param name="studentNumber">Unique identifier for the student.</param>
        /// <returns>Returns a <see cref="Student"/> object if found; otherwise, null.</returns>
        /// <exception cref="ArgumentException">Thrown if the studentNumber is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if an error occurs during API communication.</exception>
        Task<Student> GetStudentAsync(string studentNumber);

        /// <summary>
        /// Retrieves a student's details using their student number and optional specified properties.
        /// </summary>
        /// <param name="studentNumber">Unique identifier for the student.</param>
        /// <param name="studentProperties">
        /// A colon-delimited string specifying which properties of the student to retrieve (e.g., "FullName:LastName:IDNumber").
        /// If null or empty, all student properties will be fetched.
        /// </param>
        /// <returns>Returns a dictionary containing the requested student properties and their values.</returns>
        /// <exception cref="ArgumentException">Thrown if the studentNumber is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if an error occurs during API communication.</exception>
        Task<Dictionary<string, object>> GetStudentWithPropertiesAsync(string studentNumber, string studentProperties = null);

        /// <summary>
        /// Increments the numeric part of the specified reference string by a specified amount.
        /// </summary>
        /// <param name="input">The reference string to increment.</param>
        /// <param name="incrementBy">The amount to increment by.</param>
        /// <returns>The incremented reference string.</returns>
        string IncrementReference(string input, int incrementBy);

        /// <summary>
        /// Retrieves the configuration value for the specified key, or the default value if the key is not found.
        /// </summary>
        /// <param name="key">The key of the configuration setting.</param>
        /// <param name="defaultValue">The default value to return if the key is not found.</param>
        /// <returns>The configuration value for the specified key, or the default value.</returns>
        string GetConfigurationValue(string key, string defaultValue);

        /// <summary>
        /// Generates a message to send to the applicant.
        /// </summary>
        /// <param name="name">The name of the applicant.</param>
        /// <param name="course">The name of the course applied for.</param>
        /// <param name="refNumber">The reference number of the application.</param>
        /// <returns>A formatted message for the applicant.</returns>
        string OnSendMessage(string name, string course, string refNumber);

        /// <summary>
        /// Generates a message to send to the administrator about a new application.
        /// </summary>
        /// <param name="name">The name of the applicant.</param>
        /// <param name="course">The name of the course applied for.</param>
        /// <param name="refNumber">The reference number of the application.</param>
        /// <returns>A formatted message for the administrator.</returns>
        string OnSendMailToAdmin(string name, string course, string refNumber);


        /// <summary>
        /// Creates a validation response object representing a successful operation.
        /// </summary>
        /// <param name="message">The success message to be included in the response.</param>
        /// <returns>A <see cref="ValidationResponse"/> object indicating success.</returns>
        ValidationResponse SuccessResponse(string message);

        /// <summary>
        /// Creates a validation response object representing an error.
        /// </summary>
        /// <param name="message">The error message to be included in the response.</param>
        /// <returns>A <see cref="ValidationResponse"/> object indicating an error.</returns>
        ValidationResponse ErrorResponse(string message);

        string OnSendPayslipRequestMail(User user, PayslipRequestViewModel request);

        /// <summary>
        /// Converts the date string from to a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="dateString">The date string to be converted.</param>
        /// <returns>The converted <see cref="DateTime"/> object.</returns>
        DateTime ConvertToDateTime(string dateString);

        DateTime? ConvertToDateTimeNoReference(string input);

        /// <summary>
        /// Determines the next valid business day for scheduling an aptitude test at 10:00 AM.
        /// The next day must not fall on a weekend or a South African public holiday.
        /// </summary>
        /// <returns>The next valid business day at 10:00 AM.</returns>
        DateTime GetNextBusinessDayWithTime();

        /// <summary>
        /// Checks if a given date falls on a weekend (Saturday or Sunday).
        /// </summary>
        bool IsWeekend(DateTime date);

        /// <summary>
        /// Retrieves a list of South African public holidays for a given year.
        /// </summary>
        HashSet<DateTime> GetSouthAfricanPublicHolidays(int year);

        /// <summary>
        /// Deletes a local file from the wwwroot folder based on the provided relative file path.
        /// </summary>
        /// <param name="relativeFilePath">
        /// <param name="folder">
        /// The relative path of the file to be deleted (e.g., "uploads/image.jpg").
        /// This path is combined with the web host's root path to determine the absolute file location.
        /// </param>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while attempting to delete the file.
        /// </exception>
        void DeleteLocalFile(string relativeFilePath, string folder);

        /// <summary>
        /// Gets the current date and time in the configured time zone.
        /// </summary>
        /// <returns>The current date and time.</returns>
        DateTime GetCurrentTime();

        /// <summary>
        /// Sends an invitation for a lesson using the specified lesson invite request.
        /// </summary>
        /// <param name="lessonInviteRequest">The request containing details of the lesson invitation to be sent. Cannot be null.</param>
        /// <returns>A string representing the unique identifier of the sent invitation.</returns>
        string OnSendInvitation(LessonInviteRequest lessonInviteRequest);

        /// <summary>
        /// Creates an email attachment containing an iCalendar (.ics) invitation for the specified lesson request.
        /// </summary>
        /// <param name="lessonInviteRuequest">The lesson invitation request containing the details to include in the iCalendar attachment. Cannot be null.</param>
        /// <returns>An EmailAttachmentViewModel representing the generated iCalendar (.ics) file for the lesson invitation.</returns>
        EmailAttachmentViewModel BuildInvitationIcsAttachment(LessonInviteRequest lessonInviteRuequest);

        #region Venue Workflow

        /// <summary>
        /// Builds the HOD notification email body for a new venue reservation requiring approval.
        /// </summary>
        string OnSendVenueReservationHodNotification(string hodName, string facilitatorName, string venueName, string campus, int expectedStudents, string date, string timeSlot, string approveUrl);

        /// <summary>
        /// Builds the facilitator notification email body when an HOD has approved their reservation.
        /// </summary>
        string OnSendVenueApprovalNotification(string facilitatorName, string venueName, string campus, string date, string timeSlot, string hodName, string bookAssessmentUrl);

        /// <summary>
        /// Builds the facilitator notification email body when an HOD has rejected their reservation.
        /// </summary>
        string OnSendVenueRejectionNotification(string facilitatorName, string venueName, string campus, string date, string timeSlot, string hodName, string reason);

        /// <summary>
        /// Builds the student assessment booking email body with venue details.
        /// </summary>
        string OnSendAssessmentBookingStudentNotification(string studentName, string assessmentName, string courseName, string moduleName, string venueName, string campus, string date, string timeSlot, string instructions);

        /// <summary>
        /// Builds an ICS calendar attachment for a venue assessment booking.
        /// </summary>
        EmailAttachmentViewModel BuildAssessmentBookingIcsAttachment(string assessmentName, string venueName, string campus, DateTime startUtc, DateTime endUtc);
        #endregion
    }
}
