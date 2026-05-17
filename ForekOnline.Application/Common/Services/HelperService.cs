// <copyright file="HelperService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    12/31/2024 14:09:27 PM
// Purpose:         Defines the HelperService class

#region Usings
using Azure.Core;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
#endregion

namespace ForekOnline.Application.Common.Services
{
    public class HelperService : IHelperService
    {
        #region Private ReadOnly Variables

        private readonly IConfiguration _configuration;
        private readonly string _username;
        private readonly string _password;
        private readonly string _smtpHost;
        private readonly string _sendSMS;
        private readonly int _smtpPort;
        private readonly string _smsApiKey;
        private readonly string _jwtUrl;
        private readonly string _jwtUsername;
        private readonly string _jwtPassword;
        private readonly string _apiBaseAddress;
        private static readonly Random rand = new Random();
        private readonly HashSet<DateTime> _publicHolidays;
        private readonly TimeZoneInfo _saTimeZone;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Helper"/> class.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        public HelperService(IConfiguration configuration)
        {
            _configuration = configuration;
            _username = GetConfigurationValue("AppSettings:Username", "defaultUsername");
            _password = GetConfigurationValue("AppSettings:Password", "defaultPassword");
            _smtpHost = GetConfigurationValue("AppSettings:SmtpHost", "defaultSmtpHost");
            _smtpPort = int.Parse(GetConfigurationValue("AppSettings:SmtpPort", "25"));
            _smsApiKey = GetConfigurationValue("AppSettings:SmsApiKey", "defaultSmsApiKey");
            _jwtUrl = GetConfigurationValue("AppSettings:JwtUrl", "defaultJwtUrl");
            _jwtUsername = GetConfigurationValue("AppSettings:JwtUsername", "defaultJwtUsername");
            _jwtPassword = GetConfigurationValue("AppSettings:JwtPassword", "defaultJwtPassword");
            _apiBaseAddress = GetConfigurationValue("AppSettings:ApiBaseAddress", "defaultApiBaseAddress");
            _sendSMS = GetConfigurationValue("AppSettings:SendSMS", "defaultString");
            _publicHolidays = GetSouthAfricanPublicHolidays(DateTime.Now.Year);
            _saTimeZone = TimeZoneInfo.FindSystemTimeZoneById(GetTimeZoneId());
        }

        #region Public Constants

        public const string Alphabet = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public static string LoggedInUser { get; set; } = string.Empty;

        #endregion

        /// <summary>
        /// Generates a new GUID.
        /// </summary>
        /// <returns>A new GUID.</returns>
        public Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// Generates a JWT token asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the JWT token as a string.</returns>
        public async Task<string> GenerateJwtToken()
        {
            var client = new RestClient($"{_jwtUrl}?Username={_jwtUsername}&Password={_jwtPassword}");

            var request = new RestRequest();

            var body = new User { Username = _jwtUsername, Password = _jwtPassword };

            request.AddJsonBody(body);

            var response = await client.PostAsync(request);

            var content = response.Content;

            if (!response.IsSuccessful)
            {
                // Handle error
            }

            return content.Substring(31, 280);
        }

        /// <summary>
        /// Generates a formatted message string with the specified parameters.
        /// </summary>
        /// <param name="name">The name to include in the message.</param>
        /// <param name="type">The type of report.</param>
        /// <param name="date">The date of the report.</param>
        /// <param name="module">The module related to the report.</param>
        /// <param name="urgency">The urgency level of the report.</param>
        /// <returns>A formatted message string.</returns>
        public string GenerateMessage(string name, string type, string date, string module, string urgency)
        {
            return $"Good day Sir<br/><hr/> This message serves to confirm that {name} has successfully compiled & submitted their report<br/>" +
                   $" Report details are recorded as follows:<br/><hr/> Report Type: {type}<br/>" +
                   $"Date: {date}<br/> Module: {module}<br/> Urgency: {urgency}<br/><br/>Warm Regards";
        }

        /// <summary>
        /// Generates a notification string with the specified parameters.
        /// </summary>
        /// <param name="reference">The reference number of the report.</param>
        /// <param name="reportType">The type of report.</param>
        /// <param name="user">The user receiving the notification.</param>
        /// <param name="date">The date of the report.</param>
        /// <returns>A formatted notification string.</returns>
        public string GenerateNotification(string reference, string reportType, string user, DateTime date)
        {
            return $"Good day {user} this notification serves as confirmation that you've successfully submitted your report<br/>" +
                  $"1) Ref: {reference}<br/>" +
                  $"2) Report Type: {reportType}<br/>" +
                  $"3) Date: {date}<br/>";
        }

        /// <summary>
        /// Generates a random string of the specified size using alphanumeric characters.
        /// </summary>
        /// <param name="size">The size of the string to generate.</param>
        /// <returns>A randomly generated string.</returns>
        public string GenerateRandomString(int size)
        {
            var chars = new char[size];

            for (int i = 0; i < size; i++)
            {
                chars[i] = Alphabet[rand.Next(Alphabet.Length)];
            }

            return new string(chars);
        }

        /// <summary>
        /// Gets the MIME type based on the file extension.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The MIME type of the file.</returns>
        public string GetContentType(string path)
        {
            var types = GetMimeTypes();

            var ext = Path.GetExtension(path).ToLowerInvariant();

            return types[ext];
        }

        /// <summary>
        /// Gets the current date and time formatted as a string.
        /// </summary>
        /// <returns>The current date and time as a string.</returns>
        public string GetCurrentDateTime()
        {
            return DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt");
        }

        /// <summary>
        /// Gets the display name of the specified enum value.
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>The display name of the enum value.</returns>
        public string GetDisplayName(Enum enumValue)
        {
            if (enumValue == null)
            {
                LogError("GetDisplayName was called with a null enum value. Returning default 'Unknown'.");

                return "Unknown";

            }

            Type enumType = enumValue.GetType();

            MemberInfo[] memberInfo = enumType.GetMember(enumValue.ToString());

            if (memberInfo.Length == 0)
            {
                LogError($"No member information found for enum value {enumValue}.");

                return enumValue.ToString();
            }

            DisplayAttribute? displayAttribute = memberInfo.FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>();

            string result = displayAttribute?.Name ?? enumValue.ToString();

            LogError($"Resolved display name: {result} for enum");

            return result;
        }

        /// <summary>
        /// Gets a list of students asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of students.</returns>
        public async Task<List<Student>> GetStudentListAsync()
        {
            List<Student> students = new();

            try
            {
                var token = await GenerateJwtToken();

                using var client = InitializeHttpClient();

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var response = await client.GetAsync($"{_apiBaseAddress}Students/");

                if (response.IsSuccessStatusCode)
                {
                    var results = await response.Content.ReadAsStringAsync();

                    students = JsonConvert.DeserializeObject<List<Student>>(results);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    LogError($"Error fetching students. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}, Content: {errorContent}");
                }
            }
            catch (HttpRequestException httpRequestException)
            {
                LogError($"HttpRequestException occurred: {httpRequestException.Message}");
            }
            catch (Exception ex)
            {
                LogError($"An unexpected error occurred: {ex.Message}");
            }

            if (students == null || students.Count == 0)
            {
                students = GetFallbackStudentList();
            }

            return students;
        }

        /// <summary>
        /// Retrieves a student's details using their student number.
        /// </summary>
        /// <param name="studentNumber">Unique identifier for the student.</param>
        /// <returns>Returns a <see cref="Student"/> object if found; otherwise, null.</returns>
        /// <exception cref="ArgumentException">Thrown if the studentNumber is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if an error occurs during API communication.</exception>
        public async Task<Student> GetStudentAsync(string studentNumber)
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
            {
                LogError("Student number is null or empty.");
                throw new ArgumentException("Student number cannot be null or empty.", nameof(studentNumber));
            }

            try
            {
                var client = new RestClient($"{_apiBaseAddress}Student");

                var request = new RestRequest();

                request.Method = Method.Get;

                request.AddParameter("StudentNumber", studentNumber);

                request.AddHeader("Authorization", $"Bearer {Helper.GenerateJWTToken()}");

                var response = await client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                {
                    LogError($"Failed to retrieve student details for StudentNumber: {studentNumber}. StatusCode: {response.StatusCode}");
                    throw new InvalidOperationException("Failed to retrieve student details. Please check the API response.");
                }

                var student = JsonConvert.DeserializeObject<Student>(response.Content);
                if (student == null)
                {
                    LogError($"The response did not contain a valid Student object for StudentNumber: {studentNumber}");
                    throw new InvalidOperationException("The API response did not contain valid student details.");
                }

                LogError($"Successfully retrieved student details for StudentNumber: {studentNumber}");
                return student;
            }
            catch (Exception ex)
            {
                LogError($"An error occurred while retrieving student details for StudentNumber: {studentNumber}");
                throw;
            }
        }

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
        public async Task<Dictionary<string, object>> GetStudentWithPropertiesAsync(string studentNumber, string studentProperties = null)
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
            {
                LogError("Student number is null or empty.");

                throw new ArgumentException("Student number cannot be null or empty.", nameof(studentNumber));
            }

            try
            {
                var student = await GetStudentAsync(studentNumber);

                if (string.IsNullOrWhiteSpace(studentProperties))
                {
                    LogError($"Returning all properties for StudentNumber: {studentNumber}");
                    return student.GetType()
                                  .GetProperties()
                                  .ToDictionary(prop => prop.Name, prop => prop.GetValue(student));
                }

                var requestedProperties = studentProperties.Split(':');

                var result = new Dictionary<string, object>();

                foreach (var propertyName in requestedProperties)
                {
                    var property = student.GetType().GetProperty(propertyName);

                    if (property == null)
                    {
                        LogError($"Property '{propertyName}' does not exist on Student for StudentNumber: {studentNumber}");
                        throw new InvalidOperationException($"Property '{propertyName}' does not exist on the Student object.");
                    }

                    result[propertyName] = property.GetValue(student);
                }

                LogError($"Successfully retrieved specified properties for StudentNumber: {studentNumber}");

                return result;
            }
            catch (Exception ex)
            {
                LogError($"An error occurred while retrieving student properties for StudentNumber: {studentNumber}");
                throw;
            }
        }

        /// <summary>
        /// Increments the numeric part of the specified reference string by a specified amount.
        /// </summary>
        /// <param name="input">The reference string to increment.</param>
        /// <param name="incrementBy">The amount to increment by.</param>
        /// <returns>The incremented reference string.</returns>
        /// <exception cref="ArgumentException">Thrown when the input format is invalid or the numeric part is invalid.</exception>
        public string IncrementReference(string input, int incrementBy)
        {
            int slashIndex = input.IndexOf('/');

            if (slashIndex == -1 || slashIndex == input.Length - 1)
            {
                throw new ArgumentException("Invalid input format");
            }

            string prefix = input.Substring(0, slashIndex + 1);

            string numericPart = input.Substring(slashIndex + 1);

            if (!int.TryParse(numericPart, out int number))
            {
                throw new ArgumentException("Invalid numeric part");
            }

            number += incrementBy;

            string newNumericPart = number.ToString("D" + numericPart.Length);

            return prefix + newNumericPart;
        }

        /// <summary>
        /// Initializes and returns a new HttpClient with the base address set from configuration.
        /// </summary>
        /// <returns>A new HttpClient instance.</returns>
        public HttpClient InitializeHttpClient()
        {
            return new HttpClient { BaseAddress = new Uri(_apiBaseAddress) };
        }

        /// <summary>
        /// Maps properties from the source object to a new target object of type TTarget.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TTarget">The type of the target object.</typeparam>
        /// <param name="source">The source object.</param>
        /// <returns>A new target object with mapped properties.</returns>
        public TTarget MapProperties<TSource, TTarget>(TSource source) where TTarget : new()
        {
            var target = new TTarget();

            foreach (var sourceProperty in typeof(TSource).GetProperties())
            {
                var targetProperty = typeof(TTarget).GetProperty(sourceProperty.Name);

                if (targetProperty != null && targetProperty.CanWrite)
                {
                    targetProperty.SetValue(target, sourceProperty.GetValue(source));
                }
            }

            return target;
        }

        /// <summary>
        /// Asynchronously sends an email notification using the specified email data.
        /// </summary>
        /// <param name="mail">The email data containing recipient, subject, body, and header details.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendMailNotificationAsync(EmailDataViewModel mail)
        {
            var senderMail = new MailAddress(_username, mail.From ?? "Forek Online");

            var displayHeader = string.IsNullOrWhiteSpace(mail.Header) ? "Notification" : mail.Header;
            var receiverMail = new MailAddress(mail.Recipient, displayHeader);

            using var smtp = new SmtpClient(_smtpHost)
            {
                Port = _smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderMail.Address, _password)
            };

            using var mailMessage = new MailMessage(senderMail, receiverMail)
            {
                Subject = mail.Subject,
                Body = mail.Body,
                IsBodyHtml = true
            };

            if (mail.Attachments is { Count: > 0 })
            {
                foreach (var a in mail.Attachments)
                {
                    if (a is null || a.Content is null || a.Content.Length == 0 || string.IsNullOrWhiteSpace(a.FileName))
                    {
                        continue;
                    }

                    var stream = new MemoryStream(a.Content, writable: false);
                    var attachment = new Attachment(stream, a.FileName, a.ContentType);

                    mailMessage.Attachments.Add(attachment);
                }
            }

            await smtp.SendMailAsync(mailMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends an SMS message to the specified recipient.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="recipientNo">The recipient's phone number.</param>
        public void SendSms(string message, string recipientNo)
        {

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be null or empty.", nameof(message));

            if (string.IsNullOrWhiteSpace(recipientNo))
                throw new ArgumentException("RecipientViewModel number cannot be null or empty.", nameof(recipientNo));

            var client = new RestClient(_sendSMS);

            var sms = new SMSViewModel
            {
                message = message,

                recipients = new List<RecipientViewModel>
                {
                    new RecipientViewModel { mobileNumber = recipientNo }
                }
            };

            var request = new RestRequest().AddHeader("Authorization", _smsApiKey).AddJsonBody(sms);

            var response = client.Post(request);

            if (response == null || !response.IsSuccessful)
            {
                var errorContent = response?.Content ?? "No response content available.";

                throw new HttpRequestException("Failed to send SMS. Check the logs for more details.");
            }
        }

        /// <summary>
        /// Shows a notification using the specified title, text, and type.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="text">The text of the notification.</param>
        /// <param name="type">The type of the notification.</param>
        /// <returns>A formatted notification string.</returns>
        public string ShowNotification(string title, string text, string type)
        {
            return $"Swal.fire('{title}', '{text}', '{type}')";
        }

        /// <summary>
        /// Encrypts the specified value using SHA256 and encodes it in base64.
        /// </summary>
        /// <param name="value">The value to encrypt.</param>
        /// <returns>The encrypted value as a base64 string.</returns>
        public string ValueEncryption(string value)
        {
            using var sha256 = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(value);

            return Convert.ToBase64String(sha256.ComputeHash(bytes));
        }

        /// <summary>
        /// Retrieves a configuration value from the appsettings.json file.
        /// </summary>
        /// <param name="key">The key of the configuration value to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the key is not found.</param>
        /// <returns>The configuration value associated with the specified key, or the default value if the key is not found.</returns>
        public string GetConfigurationValue(string key, string defaultValue)
        {
            return _configuration[key] ?? defaultValue;
        }

        private string BuildEmail(
            string title,
            string preheader,
            string heading,
            string bodyHtml,
            string accentColor = "#8B0000",
            string buttonText = null,
            string buttonUrl = null,
            string referenceBlockHtml = null,
            string secondaryNote = null,
            string legalFooter = null)
        {
            title = Sanitize(title);
            heading = Sanitize(heading);
            preheader = Sanitize(preheader);

            string year = DateTime.UtcNow.Year.ToString();

            var buttonMarkup = string.IsNullOrWhiteSpace(buttonText) || string.IsNullOrWhiteSpace(buttonUrl)
                ? ""
                : $@"
                    <tr>
                        <td align='center' style='padding:10px 0 5px 0;'>
                            <a href='{buttonUrl}' target='_blank'
                               style='background:{accentColor};color:#ffffff;text-decoration:none;
                                      font-size:16px;font-weight:600;padding:14px 28px;
                                      display:inline-block;border-radius:6px;'>
                                {Sanitize(buttonText)}
                            </a>
                        </td>
                    </tr>";

            var referenceMarkup = string.IsNullOrWhiteSpace(referenceBlockHtml)
                ? ""
                : $@"
                    <tr>
                        <td style='padding:6px 18px 12px 18px;'>
                            <table width='100%' cellpadding='0' cellspacing='0' role='presentation'
                                   style='border-collapse:collapse;border:1px solid #e2e5e9;border-radius:6px;background:#fafbfc;'>
                                <tr>
                                    <td style='padding:12px 16px;font:14px/20px Arial,Helvetica,sans-serif;color:#374151;'>
                                        {referenceBlockHtml}
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>";

            var secondaryNoteMarkup = string.IsNullOrWhiteSpace(secondaryNote)
                ? ""
                : $@"
                    <tr>
                        <td style='padding:4px 18px 0 18px;font:13px/19px Arial,Helvetica,sans-serif;color:#6b7280;'>
                            {secondaryNote}
                        </td>
                    </tr>";

            var legal = legalFooter ??
                        "This message (and any attachments) is intended only for the individual or entity to which it is addressed. If you are not the intended recipient, please delete it.";

            return $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                <meta charset='utf-8' />
                <title>{title}</title>
                <meta name='viewport' content='width=device-width,initial-scale=1' />
                <meta http-equiv='x-ua-compatible' content='ie=edge' />
                <style>
                    /* Client-specific resets */
                    body,table,td,a {{ -webkit-text-size-adjust:100%; -ms-text-size-adjust:100%; }}
                    table,td {{ mso-table-lspace:0pt; mso-table-rspace:0pt; }}
                    img {{ -ms-interpolation-mode:bicubic; border:0; outline:none; text-decoration:none; }}
                    table {{ border-collapse:collapse !important; }}
                    body {{ margin:0 !important; padding:0 !important; background:#f3f4f6; }}
                    /* Dark mode preference (supported clients only) */
                    @media (prefers-color-scheme: dark) {{
                        body {{ background:#111827 !important; }}
                        .email-container {{ background:#1f2937 !important; }}
                        .heading {{ color:#f9fafb !important; }}
                        .body-text {{ color:#d1d5db !important; }}
                        .muted {{ color:#9ca3af !important; }}
                        .reference-box {{ background:#374151 !important; border-color:#4b5563 !important; }}
                    }}
                    @media screen and (max-width:600px) {{
                        .fluid {{ width:100% !important; max-width:100% !important; }}
                        .stack {{ display:block !important; width:100% !important; }}
                        .p-mobile {{ padding-left:16px !important; padding-right:16px !important; }}
                    }}
                </style>
                <!--[if mso]>
                <style type='text/css'>
                    body, table, td {{ font-family: Arial, Helvetica, sans-serif !important; }}
                </style>
                <![endif]-->
                </head>
                <body style='margin:0;padding:0;'>
                <span style='display:none !important;visibility:hidden;opacity:0;color:transparent;height:0;width:0;
                             overflow:hidden;mso-hide:all;font-size:1px;line-height:1px;'>{preheader}</span>

                <table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='background:#f3f4f6;padding:20px 0;'>
                <tr>
                  <td align='center'>
                    <table role='presentation' width='600' cellpadding='0' cellspacing='0' class='email-container' style='width:600px;max-width:600px;background:#ffffff;border-radius:10px;overflow:hidden;border:1px solid #e5e7eb;'>
                        <!-- Header Bar -->
                        <tr>
                            <td style='background:{accentColor};padding:14px 18px;'>
                                <table width='100%' role='presentation'>
                                    <tr>
                                        <td style='font:700 18px/22px Arial,Helvetica,sans-serif;color:#ffffff;letter-spacing:.5px;'>{heading}</td>
                                        <td align='right' style='font:12px/16px Arial,Helvetica,sans-serif;color:#fde68a;'>{DateTime.UtcNow:yyyy-MM-dd}</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>

                        <!-- Body -->
                        <tr>
                            <td style='padding:0;'>
                                <table role='presentation' width='100%' cellpadding='0' cellspacing='0'>
                                    <tr>
                                        <td style='padding:22px 18px 6px 18px;font:15px/23px Arial,Helvetica,sans-serif;color:#374151;'
                                            class='body-text'>
                                            {bodyHtml}
                                        </td>
                                    </tr>
                                    {referenceMarkup}
                                    {buttonMarkup}
                                    {secondaryNoteMarkup}
                                    <tr>
                                        <td style='padding:22px 18px 8px 18px;'>
                                            <hr style='border:none;border-top:1px solid #e5e7eb;margin:0;' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='padding:0 18px 24px 18px;font:12px/18px Arial,Helvetica,sans-serif;color:#6b7280;' class='muted'>
                                            <strong>Contact:</strong> WhatsApp 060 728 6757 • Tel 068 048 6967<br/>
                                            &copy; {year} Forek Institute of Technology. All rights reserved.<br/><br/>
                                            <em>{legal}</em>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>

                    </table>
                  </td>
                </tr>
                </table>
                </body>
                </html>";
        }

        private string Sanitize(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public string OnSendMessage(string name, string course, string refNumber)
        {
            string heading = "Application Received";
            string preheader = $"Your application for {course} has been received.";
            string body = $@"
                <p style='margin:0 0 14px 0;'>Dear {Sanitize(name)},</p>
                <p style='margin:0 0 14px 0;'>Thank you for applying to <strong>{Sanitize(course)}</strong>. We have successfully received your application and it is now queued for processing.</p>
                <p style='margin:0 0 14px 0;'>Please keep your reference number safe – you will need it for any correspondence with us.</p>
                <p style='margin:0 0 14px 0;'>We will notify you as soon as there is an update on the status of your application.</p>
                <p style='margin:0;'>Warm regards,<br/>Admissions Office</p>";

            string referenceBlock = $"<strong>Reference Number:</strong> {Sanitize(refNumber)}";

            return BuildEmail(
                title: "Application Acknowledgement",
                preheader: preheader,
                heading: heading,
                bodyHtml: body,
                referenceBlockHtml: referenceBlock,
                accentColor: "#8B0000",
                secondaryNote: "Keep this email for your records.");
        }

        public string OnSendInvitation(LessonInviteRequest lessonInviteRequest)
        {
            if (lessonInviteRequest is null)
            {
                throw new ArgumentNullException(nameof(lessonInviteRequest));
            }

            Validator.ValidateObject(lessonInviteRequest, new ValidationContext(lessonInviteRequest), validateAllProperties: true);

            if (lessonInviteRequest.EndUtc <= lessonInviteRequest.StartUtc)
            {
                throw new ArgumentException("EndUtc must be after StartUtc.", nameof(lessonInviteRequest));
            }

            var topic = (lessonInviteRequest.Topic ?? string.Empty).Trim();
            var roomName = (lessonInviteRequest.RoomName ?? string.Empty).Trim();
            var password = (lessonInviteRequest.Password ?? string.Empty).Trim();

            var startSa = TimeZoneInfo.ConvertTimeFromUtc(lessonInviteRequest.StartUtc, _saTimeZone);
            var endSa = TimeZoneInfo.ConvertTimeFromUtc(lessonInviteRequest.EndUtc, _saTimeZone);

            var joinUrl = roomName.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? roomName
                : null;

            var heading = "Lesson Invitation";
            var preheader = $"{topic} • {startSa:ddd, dd MMM yyyy HH:mm} (SAST)";

            var detailsBox = $@"
<table width='100%' cellpadding='0' cellspacing='0' role='presentation' style='border-collapse:collapse;margin:0 0 12px 0;'>
  <tr>
    <td style='padding:12px 14px;border:1px solid #e5e7eb;border-radius:8px;background:#fafafa;font:14px/20px Arial,Helvetica,sans-serif;color:#374151;'>
      <div style='font:800 16px/22px Arial,Helvetica,sans-serif;color:#111827;margin:0 0 8px 0;'>{Sanitize(topic)}</div>

      <div style='margin:0 0 10px 0;'>
        <strong>Start:</strong> {Sanitize(startSa.ToString("dddd, dd MMMM yyyy HH:mm"))} (SAST)<br/>
        <strong>End:</strong> {Sanitize(endSa.ToString("dddd, dd MMMM yyyy HH:mm"))} (SAST)
      </div>

      <div style='margin:0 0 10px 0;'>
        <strong>Room:</strong> {Sanitize(roomName)}<br/>
        {(string.IsNullOrWhiteSpace(password) ? "" : $"<strong>Password:</strong> {Sanitize(password)}<br/>")}
        {(string.IsNullOrWhiteSpace(joinUrl) ? "" : $"<strong>Join link:</strong> <a href='{joinUrl}' target='_blank' style='color:#0B3D91;text-decoration:underline;'>{Sanitize(joinUrl)}</a>")}
      </div>

      <div style='margin:0;color:#6b7280;font-size:13px;'>
        A calendar invite is attached. Please accept it to add this lesson to your schedule.
      </div>
    </td>
  </tr>
</table>";

            var bodyHtml = $@"
<p style='margin:0 0 14px 0;'>Dear Student,</p>
<p style='margin:0 0 14px 0;'>
You have been invited to attend the lesson below. Please review the details and accept the attached calendar invite.
</p>
{detailsBox}
<p style='margin:0 0 14px 0;'>
If you cannot attend, please notify your facilitator as soon as possible.
</p>
<p style='margin:0;'>Kind regards,<br/>Forek Learning Portal</p>";

            var referenceBlock = $@"
<strong>Topic:</strong> {Sanitize(topic)}<br/>
<strong>Start (SAST):</strong> {Sanitize(startSa.ToString("yyyy-MM-dd HH:mm"))}<br/>
<strong>End (SAST):</strong> {Sanitize(endSa.ToString("yyyy-MM-dd HH:mm"))}";

            return BuildEmail(
                title: "Lesson Invitation",
                preheader: preheader,
                heading: heading,
                bodyHtml: bodyHtml,
                accentColor: "#0B3D91",
                buttonText: string.IsNullOrWhiteSpace(joinUrl) ? null : "Open Lesson Room",
                buttonUrl: joinUrl,
                referenceBlockHtml: referenceBlock,
                secondaryNote: "Automated notification • Please do not reply to this email.");
        }

        public EmailAttachmentViewModel BuildInvitationIcsAttachment(LessonInviteRequest lessonInviteRequest)
        {
            if (lessonInviteRequest is null)
            {
                throw new ArgumentNullException(nameof(lessonInviteRequest));
            }

            Validator.ValidateObject(lessonInviteRequest, new ValidationContext(lessonInviteRequest), validateAllProperties: true);

            if (lessonInviteRequest.EndUtc <= lessonInviteRequest.StartUtc)
            {
                throw new ArgumentException("EndUtc must be after StartUtc.", nameof(lessonInviteRequest));
            }

            var topic = (lessonInviteRequest.Topic ?? string.Empty).Trim();
            var roomName = (lessonInviteRequest.RoomName ?? string.Empty).Trim();

            var joinUrl = roomName.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? roomName
                : roomName;

            var ics = BuildIcsInvite(topic, joinUrl, lessonInviteRequest.StartUtc, lessonInviteRequest.EndUtc);

            return new EmailAttachmentViewModel
            {
                FileName = "invite.ics",
                ContentType = "text/calendar; method=REQUEST; charset=UTF-8",
                Content = Encoding.UTF8.GetBytes(ics)
            };
        }

        public string OnSendRejectionEmail(string name, string course, string refNumber, string rejectionReason)
        {
            string heading = "Application Outcome";
            string preheader = "Your application result is now available.";
            string body = $@"
                <p style='margin:0 0 14px 0;'>Dear {Sanitize(name)},</p>
                <p style='margin:0 0 14px 0;'>Thank you for your interest in the <strong>{Sanitize(course)}</strong> program at Forek Institute of Technology.</p>
                <p style='margin:0 0 14px 0;'>After careful review, we regret to inform you that we are unable to offer you a place at this time.</p>
                <p style='margin:0 0 14px 0;'><strong>Reason Provided:</strong><br/>{Sanitize(rejectionReason)}</p>
                <p style='margin:0 0 14px 0;'>We value the effort you put into your application and encourage you to consider applying again in the future or exploring other programs we offer.</p>
                <p style='margin:0;'>Kind regards,<br/>Admissions Team</p>";

            string referenceBlock = $"<strong>Reference Number:</strong> {Sanitize(refNumber)}";

            return BuildEmail(
                title: "Application Result",
                preheader: preheader,
                heading: heading,
                bodyHtml: body,
                referenceBlockHtml: referenceBlock,
                accentColor: "#9CA3AF",
                secondaryNote: "This decision does not reflect your potential. You are welcome to reapply.");
        }


        public string OnSendPendingEmail(string name, string course, string refNumber, string pendingReason)
        {
            string heading = "Application Pending";
            string preheader = "Your application is currently under review.";
            string body = $@"
                <p style='margin:0 0 14px 0;'>Dear {Sanitize(name)},</p>
                <p style='margin:0 0 14px 0;'>Thank you for applying to the <strong>{Sanitize(course)}</strong> program at Forek Institute of Technology.</p>
                <p style='margin:0 0 14px 0;'>Your application is currently <strong>pending</strong> and requires additional attention before a final decision can be made.</p>
                <p style='margin:0 0 14px 0;'><strong>Reason for Pending Status:</strong><br/>{Sanitize(pendingReason)}</p>
                <p style='margin:0 0 14px 0;'>Please address the above at your earliest convenience. Once resolved, your application will continue through the review process and you will be notified of the outcome.</p>
                <p style='margin:0;'>Kind regards,<br/>Admissions Team</p>";
            string referenceBlock = $"<strong>Reference Number:</strong> {Sanitize(refNumber)}";
            return BuildEmail(
                title: "Application Pending",
                preheader: preheader,
                heading: heading,
                bodyHtml: body,
                referenceBlockHtml: referenceBlock,
                accentColor: "#F59E0B",
                secondaryNote: "If you have any questions regarding your pending status, please contact our admissions office.");
        }

        public string OnSendMailToAdmin(string name, string course, string refNumber)
        {
            string heading = "New Applicant Alert";
            string preheader = $"New application submitted: {course}.";
            string body = $@"
                <p style='margin:0 0 14px 0;'>Hi General Manager,</p>
                <p style='margin:0 0 14px 0;'>A new application has been received and awaits your review:</p>
                <ul style='margin:0 0 14px 18px;padding:0;font:14px/20px Arial,Helvetica,sans-serif;color:#374151;'>
                    <li><strong>Student:</strong> {Sanitize(name)}</li>
                    <li><strong>Course:</strong> {Sanitize(course)}</li>
                    <li><strong>Submitted:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm} (UTC)</li>
                </ul>
                <p style='margin:0 0 14px 0;'>Please proceed to the admin portal to review documents and proceed with the next workflow step.</p>
                <p style='margin:0;'>Regards,<br/>Forek Online System</p>";

            string referenceBlock = $"<strong>Reference:</strong> {Sanitize(refNumber)}";

            return BuildEmail(
                title: "New Application Submitted",
                preheader: preheader,
                heading: heading,
                bodyHtml: body,
                referenceBlockHtml: referenceBlock,
                accentColor: "#8B0000",
                secondaryNote: "Automated notification • No reply required.");
        }

        public string OnSendPayslipRequestMail(User user, PayslipRequestViewModel request)
        {
            string heading = "Payslip Request Submitted";
            string preheader = $"Payslip request from {user.Name} {user.LastName}";
            string period = Sanitize($"From {request.StartMonth} To {request.EndMonth}");
            string body = $@"
                <p style='margin:0 0 14px 0;'>Hi General Manager,</p>
                <p style='margin:0 0 14px 0;'>A new payslip/IRP5 document request has been submitted and requires your attention.</p>
                <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='border-collapse:collapse;margin:0 0 16px 0;'>
                    <tr>
                        <td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Employee:</strong> {Sanitize(user.Name)} {Sanitize(user.LastName)}</td>
                    </tr>
                    <tr>
                        <td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Department:</strong> {Sanitize(user.Department?.ToString() ?? "N/A")}</td>
                    </tr>
                    <tr>
                        <td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Requested Period:</strong> {period}</td>
                    </tr>
                    <tr>
                        <td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Document Type:</strong> {Sanitize(request.DocumentType.ToString())}</td>
                    </tr>
                    {(string.IsNullOrWhiteSpace(request.Reason) ? "" :
                        $"<tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Reason:</strong> {Sanitize(request.Reason)}</td></tr>")}
                </table>
                <p style='margin:0;'>Regards,<br/>HR Portal</p>";

            string referenceBlock = "<strong>Action:</strong> Please approve or fulfil the request in the HR system.";

            return BuildEmail(
                title: "Payslip Request",
                preheader: preheader,
                heading: heading,
                bodyHtml: body,
                referenceBlockHtml: referenceBlock,
                accentColor: "#0B7285",
                secondaryNote: "Automated HR workflow notification.");
        }

        public string OnSendApprovalEmailToTradeAndSkills(string name, string course, string refNumber)
        {
            string heading = "Application Approved";
            string preheader = "Your application has been approved. Complete enrollment now.";
            string enrollUrl = "https://na4.documents.adobe.com/public/esignWidget?wid=CBFCIBAA3AAABLblqZhA37o8lb-pwaQGF604e4jiEvp1cD8pmAH9u6QWO4bzHMVu-3HHDaQqfUoDtTsnp8hw*";

            string body = $@"
                <p style='margin:0 0 14px 0;'>Dear {Sanitize(name)},</p>
                <p style='margin:0 0 14px 0;'>Great news! Your application for the <strong>{Sanitize(course)}</strong> program has been <strong>approved</strong>.</p>
                <p style='margin:0 0 14px 0;'>To finalize your admission, please complete the mandatory enrollment document online and arrange payment of the applicable tuition fees.</p>
                <p style='margin:0 0 14px 0;'>If you have already completed this step, you may ignore this reminder.</p>
                <p style='margin:0 0 14px 0;'>We look forward to welcoming you to campus.</p>
                <p style='margin:0;'>Warm regards,<br/>Admissions Team</p>";

            string referenceBlock = $"<strong>Reference Number:</strong> {Sanitize(refNumber)}";

            return BuildEmail(
                title: "Enrollment Required",
                preheader: preheader,
                heading: heading,
                bodyHtml: body,
                referenceBlockHtml: referenceBlock,
                buttonText: "Complete Enrollment",
                buttonUrl: enrollUrl,
                accentColor: "#046A38",
                secondaryNote: "Enrollment link is unique to this application. Do not share.");
        }

        public string OnSendAptitudeTestInvitation(string name, string course, string refNumber, string testDateTime)
        {
            string heading = "Aptitude Test Invitation";
            string preheader = "Your aptitude test has been scheduled.";
            string venueUrl = "https://www.google.co.za/maps/place/Forek+Institute+of+Technology/@-25.3498321,31.108492,17z";
            string body = $@"
                <p style='margin:0 0 14px 0;'>Dear {Sanitize(name)},</p>
                <p style='margin:0 0 14px 0;'>Your application for <strong>{Sanitize(course)}</strong> has progressed to the next stage. You are invited to sit for the required aptitude test.</p>
                <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='border-collapse:collapse;margin:0 0 16px 0;'>
                    <tr>
                        <td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Date & Time:</strong> {Sanitize(testDateTime)}</td>
                    </tr>
                    <tr>
                        <td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Venue:</strong> Forek Institute of Technology (See map link below)</td>
                    </tr>
                </table>
                <p style='margin:0 0 14px 0;'><strong>Bring:</strong> Certified ID Copy, Qualifications, Guardian ID, Proof of Residence, Black Pen.</p>
                <p style='margin:0 0 14px 0;'>Please arrive 15 minutes early for registration.</p>
                <p style='margin:0;'>Good luck!<br/>Admissions Assessments</p>";

            string referenceBlock = $"<strong>Reference:</strong> {Sanitize(refNumber)}";

            return BuildEmail(
                title: "Aptitude Test Invitation",
                preheader: preheader,
                heading: heading,
                bodyHtml: body,
                referenceBlockHtml: referenceBlock,
                buttonText: "Open Map Location",
                buttonUrl: venueUrl,
                accentColor: "#0B3D91",
                secondaryNote: "If you are unable to attend, please notify us at least 48 hours before the test.");
        }

        public string OnSendGenericApprovalEmail(string name, string course, string referenceNumber)
        {
            string heading = "Application Approved";
            string preheader = "Your application has been approved. Complete enrollment now.";
            string enrollUrl = "https://na4.documents.adobe.com/public/esignWidget?wid=CBFCIBAA3AAABLblqZhA37o8lb-pwaQGF604e4jiEvp1cD8pmAH9u6QWO4bzHMVu-3HHDaQqfUoDtTsnp8hw*";

            string body = $@"
                <p style='margin:0 0 14px 0;'>Dear {Sanitize(name)},</p>
                <p style='margin:0 0 14px 0;'>Great news! Your application for the <strong>{Sanitize(course)}</strong> program has been <strong>approved</strong>.</p>
                <p style='margin:0 0 14px 0;'>To finalize your admission, please complete the mandatory enrollment document online and arrange payment of the applicable tuition fees.</p>
                <p style='margin:0 0 14px 0;'>If you have already completed this step, you may ignore this reminder.</p>
                <p style='margin:0 0 14px 0;'>We look forward to welcoming you to campus.</p>
                <p style='margin:0;'>Warm regards,<br/>Admissions Team</p>";

            string referenceBlock = $"<strong>Reference Number:</strong> {Sanitize(referenceNumber)}";

            return BuildEmail(
                title: "Enrollment Required",
                preheader: preheader,
                heading: heading,
                bodyHtml: body,
                referenceBlockHtml: referenceBlock,
                buttonText: "Complete Enrollment",
                buttonUrl: enrollUrl,
                accentColor: "#046A38",
                secondaryNote: "Enrollment link is unique to this application. Do not share.");
        }

        /// <summary>
        /// Checks if a given date falls on a weekend (Saturday or Sunday).
        /// </summary>
        public bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        /// <summary>
        /// Retrieves a list of South African public holidays for a given year.
        /// </summary>
        public HashSet<DateTime> GetSouthAfricanPublicHolidays(int year)
        {
            return new HashSet<DateTime>
        {
            new DateTime(year, 1, 1),   // New Year's Day
            new DateTime(year, 3, 21),  // Human Rights Day
            new DateTime(year, 4, 27),  // Freedom Day
            new DateTime(year, 5, 1),   // Workers' Day
            new DateTime(year, 6, 16),  // Youth Day
            new DateTime(year, 8, 9),   // National Women's Day
            new DateTime(year, 9, 24),  // Heritage Day
            new DateTime(year, 12, 16), // Day of Reconciliation
            new DateTime(year, 12, 25), // Christmas Day
            new DateTime(year, 12, 26)  // Day of Goodwill
        };
        }

        /// <summary>
        /// Determines the next valid business day for scheduling an aptitude test at 10:00 AM.
        /// The next day must not fall on a weekend or a South African public holiday.
        /// </summary>
        /// <returns>The next valid business day at 10:00 AM.</returns>
        public DateTime GetNextBusinessDayWithTime()
        {
            DateTime nextDay = DateTime.Now.AddDays(1);

            while (IsWeekend(nextDay) || _publicHolidays.Contains(nextDay.Date))
            {
                nextDay = nextDay.AddDays(1);
            }

            DateTime scheduledTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 10, 0, 0);

            return scheduledTime;
        }

        /// <summary>
        /// Converts the date string from to a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="dateString">The date string to be converted.</param>
        /// <returns>The converted <see cref="DateTime"/> object.</returns>
        public DateTime ConvertToDateTime(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                throw new ArgumentException("Input string cannot be null or empty.", nameof(dateString));
            }

            string dateFormat = "dd/MM/yyyy";


            if (DateTime.TryParseExact(dateString, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            else
            {
                throw new FormatException("The input string is not in the correct format.");
            }

        }

        /// <summary>
        /// Converts the date string from to a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="input">The date string to be converted.</param>
        /// <returns>The converted <see cref="DateTime"/> object.</returns>
        public DateTime? ConvertToDateTimeNoReference(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                LogError("Input string is null or empty");
                return null;
            }

            if (TryParseReportDate(input, out var dt))
                return dt;

            LogError($"Failed to parse Input string: {input}");
            return null;
        }

        /// <summary>
        /// Gets the current date and time in South Africa Standard Time (SAST).
        /// </summary>
        /// <returns>The current date and time in SAST.</returns>
        public DateTime GetCurrentTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _saTimeZone);
        }

        #region Venue WorkFlow

        /// <inheritdoc/>
        public string OnSendVenueReservationHodNotification(string hodName, string facilitatorName, string venueName, string campus, int expectedStudents, string date, string timeSlot, string approveUrl)
        {
            var body = $@"
                <p style='margin:0 0 14px 0;'>Dear {Sanitize(hodName)},</p>
                <p style='margin:0 0 14px 0;'>A new venue reservation requires your attention. <strong>{Sanitize(facilitatorName)}</strong> has requested the following slot:</p>
                <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='border-collapse:collapse;margin:0 0 16px 0;'>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Venue:</strong> {Sanitize(venueName)}</td></tr>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Campus:</strong> {Sanitize(campus)}</td></tr>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Expected Students:</strong> {expectedStudents}</td></tr>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Date:</strong> {Sanitize(date)}</td></tr>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Time:</strong> {Sanitize(timeSlot)}</td></tr>
                </table>
                <p style='margin:0 0 14px 0;'>This reservation will expire automatically in <strong>24 hours</strong> if no action is taken. Please approve or reject at your earliest convenience.</p>
                <p style='margin:0;'>Kind regards,<br/>Forek Online System</p>";

            var referenceBlock = $"<strong>Facilitator:</strong> {Sanitize(facilitatorName)}<br/><strong>Venue:</strong> {Sanitize(venueName)} ({Sanitize(campus)})";

            return BuildEmail(
                title: "Venue Reservation – Approval Required",
                preheader: $"{facilitatorName} has reserved {venueName} for {date} — action required.",
                heading: "🏫 Venue Reservation – HOD Approval",
                bodyHtml: body,
                accentColor: "#b31217",
                buttonText: "Review Pending Reservations",
                buttonUrl: approveUrl,
                referenceBlockHtml: referenceBlock,
                secondaryNote: "Automated notification from Forek Online. Please do not reply to this email.");
        }

        /// <inheritdoc/>
        public string OnSendVenueApprovalNotification(string facilitatorName, string venueName, string campus, string date, string timeSlot, string hodName, string bookAssessmentUrl)
        {
            var body = $@"
                <p style='margin:0 0 14px 0;'>Dear {Sanitize(facilitatorName)},</p>
                <p style='margin:0 0 14px 0;'>Great news! Your venue reservation has been <strong style='color:#059669;'>approved</strong> by <strong>{Sanitize(hodName)}</strong>.</p>
                <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='border-collapse:collapse;margin:0 0 16px 0;'>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Venue:</strong> {Sanitize(venueName)}</td></tr>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Campus:</strong> {Sanitize(campus)}</td></tr>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Date:</strong> {Sanitize(date)}</td></tr>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Time:</strong> {Sanitize(timeSlot)}</td></tr>
                </table>
                <p style='margin:0 0 14px 0;'>You may now proceed to <strong>Stage 2</strong> to create and send your assessment on this approved venue.</p>
                <p style='margin:0;'>Best regards,<br/>Forek Online System</p>";

            var referenceBlock = $"<strong>Approved by:</strong> {Sanitize(hodName)}<br/><strong>Venue:</strong> {Sanitize(venueName)} ({Sanitize(campus)})";

            return BuildEmail(
                title: "Venue Reservation Approved ✓",
                preheader: $"Your reservation for {venueName} on {date} has been approved.",
                heading: "✅ Venue Reservation Approved",
                bodyHtml: body,
                accentColor: "#059669",
                buttonText: "Book Assessment Now",
                buttonUrl: bookAssessmentUrl,
                referenceBlockHtml: referenceBlock,
                secondaryNote: "Automated notification from Forek Online.");
        }

        /// <inheritdoc/>
        public string OnSendVenueRejectionNotification(string facilitatorName, string venueName, string campus, string date, string timeSlot, string hodName, string reason)
        {
            var body = $@"
                <p style='margin:0 0 14px 0;'>Dear {Sanitize(facilitatorName)},</p>
                <p style='margin:0 0 14px 0;'>Unfortunately, your venue reservation has been <strong style='color:#dc2626;'>rejected</strong> by <strong>{Sanitize(hodName)}</strong>.</p>
                <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='border-collapse:collapse;margin:0 0 16px 0;'>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Venue:</strong> {Sanitize(venueName)}</td></tr>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Campus:</strong> {Sanitize(campus)}</td></tr>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Date:</strong> {Sanitize(date)}</td></tr>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Time:</strong> {Sanitize(timeSlot)}</td></tr>
                </table>
                <p style='margin:0 0 14px 0;'><strong>Reason:</strong><br/>{Sanitize(reason)}</p>
                <p style='margin:0 0 14px 0;'>The slot has been released. You may create a new reservation with a different venue or time.</p>
                <p style='margin:0;'>Kind regards,<br/>Forek Online System</p>";

            var referenceBlock = $"<strong>Rejected by:</strong> {Sanitize(hodName)}<br/><strong>Reason:</strong> {Sanitize(reason)}";

            return BuildEmail(
                title: "Venue Reservation Rejected",
                preheader: $"Your reservation for {venueName} on {date} has been rejected.",
                heading: "❌ Venue Reservation Rejected",
                bodyHtml: body,
                accentColor: "#dc2626",
                referenceBlockHtml: referenceBlock,
                secondaryNote: "Automated notification from Forek Online.");
        }

        /// <inheritdoc/>
        public string OnSendAssessmentBookingStudentNotification(string studentName, string assessmentName, string courseName, string moduleName, string venueName, string campus, string date, string timeSlot, string instructions)
        {
            var instructionBlock = string.IsNullOrWhiteSpace(instructions)
                ? ""
                : $@"<tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'>
                        <strong>Instructions:</strong><br/>{Sanitize(instructions)}
                    </td></tr>";

            var body = $@"
                <p style='margin:0 0 14px 0;'>Dear {Sanitize(studentName)},</p>
                <p style='margin:0 0 14px 0;'>You have been scheduled for an upcoming assessment. Please review the details below carefully and arrive on time.</p>
                <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='border-collapse:collapse;margin:0 0 16px 0;'>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Assessment:</strong> {Sanitize(assessmentName)}</td></tr>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Course:</strong> {Sanitize(courseName)}</td></tr>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Module:</strong> {Sanitize(moduleName)}</td></tr>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Venue:</strong> {Sanitize(venueName)}</td></tr>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Campus:</strong> {Sanitize(campus)}</td></tr>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Date:</strong> {Sanitize(date)}</td></tr>
                    <tr><td style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;padding:4px 0;'><strong>Time:</strong> {Sanitize(timeSlot)}</td></tr>
                    {instructionBlock}
                </table>
                <p style='margin:0 0 14px 0;'>A calendar invite is attached. Please accept it to add this assessment to your schedule.</p>
                <p style='margin:0 0 14px 0;'>Bring your student card and any required materials. If you cannot attend, notify your facilitator immediately.</p>
                <p style='margin:0;'>Good luck!<br/>Forek Assessment Office</p>";

            var referenceBlock = $@"<strong>Assessment:</strong> {Sanitize(assessmentName)}<br/>
                <strong>Venue:</strong> {Sanitize(venueName)} ({Sanitize(campus)})<br/>
                <strong>Date:</strong> {Sanitize(date)} • {Sanitize(timeSlot)}";

            return BuildEmail(
                title: "Assessment Scheduled – Venue Details",
                preheader: $"{assessmentName} at {venueName} on {date}.",
                heading: "📝 Assessment Booking Confirmation",
                bodyHtml: body,
                accentColor: "#0B3D91",
                referenceBlockHtml: referenceBlock,
                secondaryNote: "Automated notification • Please do not reply to this email.");
        }

        /// <inheritdoc/>
        public EmailAttachmentViewModel BuildAssessmentBookingIcsAttachment(string assessmentName, string venueName, string campus, DateTime startUtc, DateTime endUtc)
        {
            var location = $"{venueName}, {campus}";
            var ics = BuildIcsInvite(assessmentName, location, startUtc, endUtc);

            return new EmailAttachmentViewModel
            {
                FileName = "assessment-invite.ics",
                ContentType = "text/calendar; method=REQUEST; charset=UTF-8",
                Content = Encoding.UTF8.GetBytes(ics)
            };
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Determines the appropriate time zone ID based on the runtime environment.
        /// </summary>
        /// <returns>The Windows or IANA time zone ID for South Africa.</returns>
        private string GetTimeZoneId()
        {
            return OperatingSystem.IsWindows() ? "South Africa Standard Time" : "Africa/Johannesburg";
        }

        private static string BuildIcsInvite(string topic, string locationOrUrl, DateTime startUtc, DateTime endUtc)
        {
            var uid = Guid.NewGuid().ToString("N");

            return $"""
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//Forek//Learning Portal//EN
            CALSCALE:GREGORIAN
            METHOD:REQUEST
            BEGIN:VEVENT
            UID:{uid}
            DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}
            DTSTART:{startUtc:yyyyMMddTHHmmssZ}
            DTEND:{endUtc:yyyyMMddTHHmmssZ}
            SUMMARY:{EscapeIcs(topic)}
            DESCRIPTION:{EscapeIcs("Lesson invitation. Location/Room: " + locationOrUrl)}
            LOCATION:{EscapeIcs(locationOrUrl)}
            END:VEVENT
            END:VCALENDAR
            """;
        }

        private static string EscapeIcs(string value)
            => (value ?? string.Empty)
                .Replace(@"\", @"\\")
                .Replace(";", @"\;")
                .Replace(",", @"\,")
                .Replace("\r\n", @"\n")
                .Replace("\n", @"\n");

        /// <summary>
        /// Gets the MIME types for common file extensions.
        /// </summary>
        /// <returns>A dictionary mapping file extensions to MIME types.</returns>
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                { ".txt", "text/plain" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                { ".docx", "application/vnd.ms-word" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" },
                { ".csv", "text/csv" }
            };
        }

        /// <summary>
        /// Provides a fallback list of students with predefined dummy data.
        /// This can be used when the actual student data is unavailable from the API.
        /// </summary>
        /// <returns>A list of dummy students with basic details and enrollment history.</returns>
        private List<Student> GetFallbackStudentList()
        {
            return new List<Student>
            {
                new Student
                {
                    StudentId = Guid.NewGuid(),
                    StudentNumber = "FIT-DUMMY",
                    AdmissionDate = DateTime.Now,
                    FirstName = "Girl",
                    MiddleName = "Mr Itu",
                    LastName = "Mom",
                    IDNumber = "ID1234567899",
                    StudyPermitNumber = "SP123456",
                    PassportNumber = "P123456",
                    DateofBirth = DateTime.Parse("2000-01-01"),
                    Gender = "Male",
                    PlaceofBirth = "City A",
                    Nationality = "South Africa",
                    Language = "English",
                    AdmissionCategory = "Category A",
                    StreetAddressLine1 = "123 Main St",
                    StreetAddressLine2 = "Apt 4B",
                    Cellphone = "1234567890",
                    Email = "ifoliphant@forekisntitute.co.za",
                    HighestGrade = "Grade 12",
                    NameofSchool = "Dummy School",
                    IsActive = true,
                    Deregistered = false,
                    EnrollmentHistory = new List<EnrollmentHistory>
                    {
                        new EnrollmentHistory
                        {
                            EnrollmentId = Guid.NewGuid(),
                            StudentId = Guid.NewGuid(),
                            CourseId = Guid.NewGuid(),
                            CourseTitle = "IT",
                            CourseType = "Software Dev",
                            EnrollmentStatus = "Completed",
                            StartDate = DateTime.Parse("2020-09-01"),
                            IsActive = true
                        }
                    }
                },
                new Student
                {
                    StudentId = Guid.NewGuid(),
                    StudentNumber = "FITDUMMY2",
                    AdmissionDate = DateTime.Now,
                    FirstName = "Boy",
                    MiddleName = "G",
                    LastName = "Dad",
                    IDNumber = "ID6543215556",
                    StudyPermitNumber = "SP654321",
                    PassportNumber = "P654321",
                    DateofBirth = DateTime.Parse("1999-05-15"),
                    Gender = "Female",
                    PlaceofBirth = "Nkandla",
                    Nationality = "South African",
                    Language = "Zulu",
                    AdmissionCategory = "Category B",
                    StreetAddressLine1 = "456 Elm St",
                    StreetAddressLine2 = "Suite 2A",
                    Cellphone = "098-765-4321",
                    Email = "jacob.zuma@gmail.com",
                    HighestGrade = "Grade 11",
                    NameofSchool = "School B",
                    IsActive = true,
                    Deregistered = false,
                    EnrollmentHistory = new List<EnrollmentHistory>
                    {
                        new EnrollmentHistory
                        {
                            EnrollmentId = Guid.NewGuid(),
                            StudentId = Guid.NewGuid(),
                            CourseId = Guid.NewGuid(),
                            CourseTitle = "Course 202",
                            CourseType = "Type B",
                            EnrollmentStatus = "In Progress",
                            StartDate = DateTime.Parse("2021-01-15"),
                            IsActive = true
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Logs an error message to the console.
        /// This method can be enhanced to log errors to a file or monitoring system.
        /// </summary>
        /// <param name="message">The error message to be logged.</param>
        private void LogError(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Validates if the model object is not null. If null, logs the incident and redirects to the PageNotFound action.
        /// </summary>
        /// <typeparam name="T">The type of model object to validate.</typeparam>
        /// <param name="controller">The controller instance to handle redirection.</param>
        /// <param name="model">The model object to validate.</param>
        /// <param name="logger">The logger to record validation events.</param>
        /// <returns>An IActionResult, either null if model is valid or a redirection to the PageNotFound action if null.</returns>
        public IActionResult ValidateModel<T>(Controller controller, T model, ILogger logger = null)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller), "Controller instance cannot be null.");
            }

            if (model == null)
            {
                logger?.LogWarning("Model validation failed: model instance is null.");
            }

            return null;
        }

        /// <summary>
        /// Creates a validation response object representing a successful operation.
        /// </summary>
        /// <param name="message">The success message to be included in the response.</param>
        /// <returns>A <see cref="ValidationResponse"/> object indicating success.</returns>
        public ValidationResponse SuccessResponse(string message) => new ValidationResponse
        {
            IsError = false,
            Message = message
        };

        /// <summary>
        /// Creates a validation response object representing an error.
        /// </summary>
        /// <param name="message">The error message to be included in the response.</param>
        /// <returns>A <see cref="ValidationResponse"/> object indicating an error.</returns>
        public ValidationResponse ErrorResponse(string message) => new ValidationResponse
        {
            IsError = true,
            Message = message
        };
        /// <summary>
        /// Deletes a local file from the wwwroot folder based on the provided relative file path.
        /// </summary>
        /// <param name="relativeFilePath">
        /// The relative path of the file to be deleted (e.g., "uploads/image.jpg").
        /// This path is combined with the web host's root path to determine the absolute file location.
        /// </param>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while attempting to delete the file.
        /// </exception>
        public void DeleteLocalFile(string relativeFilePath, string folderPath)
        {
            var path1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            string folder = path1 + @$"\{folderPath}\" + relativeFilePath;

            if (System.IO.File.Exists(folder))
            {
                try
                {
                    System.IO.File.Delete(folder);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error deleting file at {folder}: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Attempts to parse a report date string in multiple expected formats.
        /// </summary>
        private bool TryParseReportDate(string input, out DateTime result)
        {
            input = input.Trim();

            var formats = new[]
            {
                "dddd, dd MMMM yyyy hh:mm tt",
                "dddd, dd MMMM yyyy h:mm tt",

                "M/d/yyyy h:mm:ss tt",
                "M/d/yyyy hh:mm:ss tt",
                "MM/dd/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",

                "M/d/yyyy h:mm tt",
                "M/d/yyyy hh:mm tt",
                "MM/dd/yyyy h:mm tt",
                "MM/dd/yyyy hh:mm tt",

                "d/M/yyyy H:mm:ss",
                "d/M/yyyy HH:mm:ss",
                "d/M/yyyy H:mm",
                "d/M/yyyy HH:mm",

                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm:ssZ",
                "yyyy-MM-ddTHH:mm:ssK",
                "yyyy-MM-ddTHH:mm:ss.fff",
                "yyyy-MM-ddTHH:mm:ss.fffZ",
                "yyyy-MM-ddTHH:mm:ss.fffK"
    };

            if (DateTime.TryParseExact(
                    input,
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                    out result))
            {
                return true;
            }

            if (DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
            {
                return true;
            }

            result = default;
            return false;
        }
    }
    #endregion
}
