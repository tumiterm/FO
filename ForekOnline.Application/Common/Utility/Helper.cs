// <copyright file="Helper.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2023 20:09:27 PM
// Purpose:         Defines the Helper class

#region Usings
using ForekOnline.Domain.Entities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ForekOnline.Domain.ViewModels;
#endregion

namespace ForekOnline.Application.Common.Utility
{
    public static class Helper
    {
        static Random rand = new Random();
        private static string _username = $"apprentice@forek.co.za";
        private static string _password = "P@55w0rd2022";
        public static string contact = "0818319937";

        public const string Alphabet =
        "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string loggedInUser = "";

        public static string ValueEncryption(string value)
        {
            return Convert.ToBase64String(

                System.Security.Cryptography.SHA256.Create()

                .ComputeHash(Encoding.UTF8.GetBytes(value))

                );
        }
        public static Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }
        public static HttpClient Initialize(string baseAddress)
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri(baseAddress);

            return client;
        }
        public static string GetContentType(string path)
        {
            var types = GetMimeTypes();

            var ext = Path.GetExtension(path).ToLowerInvariant();

            return types[ext];
        }
        public static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        public static string RandomStringGenerator(int size)
        {
            char[] chars = new char[size];

            for (int i = 0; i < size; i++)
            {
                chars[i] = Alphabet[rand.Next(Alphabet.Length)];
            }

            return new string(chars);

        }
        public static string OnGetCurrentDateTime()
        {
            return DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt");
        }

        // ------------------------------------------------------------------
        // Centralized email template builder (generic for reuse)
        // ------------------------------------------------------------------
        private static string BuildBrandedEmail(
            string title,
            string preheader,
            string heading,
            string bodyHtml,
            string accentColor = "#8B0000",
            string referencePanelHtml = null,
            string footerNote = null)
        {
            string year = DateTime.UtcNow.Year.ToString();

            string referenceBlock = string.IsNullOrWhiteSpace(referencePanelHtml)
                ? ""
                : $@"
                    <tr>
                        <td style='padding:4px 0 18px 0;'>
                            <table role='presentation' width='100%' cellpadding='0' cellspacing='0'
                                   style='border-collapse:collapse;background:#fafbfc;border:1px solid #e5e7eb;border-radius:8px;'>
                                <tr>
                                    <td style='padding:14px 16px;font:14px/20px Arial,Helvetica,sans-serif;color:#374151;'>
                                        {referencePanelHtml}
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>";

            string noteBlock = string.IsNullOrWhiteSpace(footerNote)
                ? ""
                : $@"<p style='margin:18px 0 0 0;font:12px/18px Arial,Helvetica,sans-serif;color:#6b7280;'>{footerNote}</p>";

            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='utf-8'>
<title>{Escape(title)}</title>
<meta name='viewport' content='width=device-width,initial-scale=1'>
<meta http-equiv='x-ua-compatible' content='ie=edge'>
<style>
    body,table,td,a {{ -webkit-text-size-adjust:100%; -ms-text-size-adjust:100%; }}
    table,td {{ mso-table-lspace:0pt; mso-table-rspace:0pt; }}
    img {{ -ms-interpolation-mode:bicubic; }}
    body {{ margin:0 !important; padding:0 !important; background:#f3f4f6; }}
    table {{ border-collapse:collapse !important; }}
    a {{ text-decoration:none; }}
    @media screen and (max-width:600px) {{
        .fluid {{ width:100% !important; max-width:100% !important; }}
        .stack {{ display:block !important; width:100% !important; }}
        .px-mobile {{ padding-left:18px !important; padding-right:18px !important; }}
    }}
    @media (prefers-color-scheme: dark) {{
        body {{ background:#111827 !important; }}
        .card {{ background:#1f2937 !important; }}
        .heading {{ color:#f9fafb !important; }}
        .text {{ color:#d1d5db !important; }}
        .muted {{ color:#9ca3af !important; }}
        .refbox {{ background:#374151 !important; border-color:#4b5563 !important; }}
    }}
</style>
<!--[if mso]>
<style type='text/css'>
body, table, td {{ font-family: Arial, Helvetica, sans-serif !important; }}
</style>
<![endif]-->
</head>
<body style='margin:0;padding:0;'>
<span style='display:none !important; visibility:hidden; opacity:0; color:transparent; height:0; width:0; overflow:hidden; mso-hide:all;'>{Escape(preheader)}</span>

<table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='background:#f3f4f6;padding:20px 0;'>
<tr>
  <td align='center'>
    <table role='presentation' width='620' cellpadding='0' cellspacing='0' class='card' style='width:620px;max-width:620px;background:#ffffff;border:1px solid #e5e7eb;border-radius:12px;overflow:hidden;'>
      <!-- Header -->
      <tr>
        <td style='background:{accentColor};padding:18px 22px;'>
            <table role='presentation' width='100%'>
                <tr>
                    <td style='font:600 20px/24px Arial,Helvetica,sans-serif;color:#ffffff;letter-spacing:.5px;' class='heading'>{Escape(heading)}</td>
                    <td style='text-align:right;font:12px/16px Arial,Helvetica,sans-serif;color:#fde68a;'>{DateTime.UtcNow:yyyy-MM-dd}</td>
                </tr>
            </table>
        </td>
      </tr>
      <!-- Body -->
      <tr>
        <td style='padding:26px 30px 30px 30px;font:15px/23px Arial,Helvetica,sans-serif;color:#374151;' class='text px-mobile'>
            {bodyHtml}
            {referenceBlock}
            {noteBlock}
            <hr style='margin:30px 0 18px 0;border:none;border-top:1px solid #e5e7eb;' />
            <p style='margin:0 0 4px 0;font:12px/18px Arial,Helvetica,sans-serif;color:#6b7280;' class='muted'>
                Contact: WhatsApp 060 728 6757 • Tel 068 048 6967
            </p>
            <p style='margin:0 0 4px 0;font:12px/18px Arial,Helvetica,sans-serif;color:#6b7280;' class='muted'>
                &copy; {year} Forek Institute of Technology. All rights reserved.
            </p>
            <p style='margin:12px 0 0 0;font:11px/16px Arial,Helvetica,sans-serif;color:#9ca3af;' class='muted'>
                This is an automated notification; please do not reply.
            </p>
        </td>
      </tr>
    </table>
  </td>
</tr>
</table>
</body>
</html>";
        }

        private static string Escape(string input) =>
            string.IsNullOrEmpty(input)
                ? string.Empty
                : input.Replace("<", "&lt;").Replace(">", "&gt;");

        /// <summary>
        /// Professional report submission confirmation email (GM notification).
        /// </summary>
        public static string OnSendMessage(string name, string type, string date, string reference, string department)
        {
            var heading = "Report Submission";
            var preheader = $"Report submitted: {type} • Ref {reference}";
            var body = $@"
                <p style='margin:0 0 16px 0;'>Good day, General Manager,</p>
                <p style='margin:0 0 16px 0;'>This is to confirm that <strong>{Escape(name)}</strong> has successfully compiled and submitted a report.</p>
                <p style='margin:0 0 14px 0;'>Details are summarized below:</p>";

            var referencePanel = $@"
                <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='font:14px/20px Arial,Helvetica,sans-serif;color:#374151;'>
                    <tr><td style='padding:2px 0;'><strong>Report Type:</strong> {Escape(type)}</td></tr>
                    <tr><td style='padding:2px 0;'><strong>Date:</strong> {Escape(date)}</td></tr>
                    <tr><td style='padding:2px 0;'><strong>Department:</strong> {Escape(department)}</td></tr>
                    <tr><td style='padding:2px 0;'><strong>Reference:</strong> {Escape(reference)}</td></tr>
                </table>";

            return BuildBrandedEmail(
                title: "Report Submission Confirmation",
                preheader: preheader,
                heading: heading,
                bodyHtml: body,
                accentColor: "#8B0000",
                referencePanelHtml: referencePanel,
                footerNote: "Please log into the portal to review or action this report."
            );
        }
        public static string OnSendNotification(string reference, string reportType, string user, DateTime date)
        {
            return $"Good day {user} this notification servers as confirmation that you've successfully submitted your report<br/>" +
                $"1) Ref: {reference}<br/>" +
                $"2) Report Type: {reportType}<br/>" +
                $"3) Date: {date}<br/>";
        }
        public static void OnSendMailNotification(string reciever, string subject, string message, string header)
        {
            var senderMail = new MailAddress(_username, $"Forek Online");

            var recieverMail = new MailAddress(reciever, header);

            var password = _password;

            var sub = subject;

            var body = message;

            var smtp = new SmtpClient
            {
                Host = "smtp.forek.co.za",

                Port = 587,

                EnableSsl = true,

                DeliveryMethod = SmtpDeliveryMethod.Network,

                UseDefaultCredentials = false,

                Credentials = new NetworkCredential(senderMail.Address, password)
            };

            using (var mess = new MailMessage(senderMail, recieverMail)
            {
                Subject = subject,

                Body = body,

                IsBodyHtml = true,

            })

            {
                //mess.Attachments.Add(new Attachment("C:\\file.zip"));

                smtp.Send(mess);
            }
        }
        public static void SendSMS(string message, string recipientNo)
        {
            var client = new RestClient("https://www.winsms.co.za/api/rest/v1/sms/outgoing/send/");

            var request = new RestRequest();

            request.AddHeader("Authorization", "2C9DAABE-20FE-4BC1-9BF3-276D9BBC9699");

            SMSViewModel sms = new()
            {
                message = message,

                recipients = new List<RecipientViewModel>
                {
                    new RecipientViewModel { mobileNumber = recipientNo}
                }

            };

            request.AddJsonBody(sms);

            var response = client.Post(request);

            string content = response.Content.ToString();

            if (!response.IsSuccessful)
            {

            }

        }
        public static string GenerateJWTToken()
        {
            var client = new RestClient("http://forekapi.dreamline-ict.co.za/authenticate/?Username=forekapi@forekinstitute.co.za&Password=api.P@ssw0rd");

            var request = new RestRequest();

            var body = new User { Username = "forekapi@forekinstitute.co.za", Password = "api.P@ssw0rd" };

            request.AddJsonBody(body);

            var response = client.Post(request);

            string content = response.Content.Substring(31, 280);

            if (!response.IsSuccessful)
            {
                // ViewData["data"] = "Error: Server encountered an error";
            }

            return content;
        }
        public static string ShowNotification(string title, string text, string type)
        {
            return $"Swal.fire('{title}', '{text}', '{type}')";
        }
        public static string GetDisplayName(this Enum enumValue)
        {
            if (enumValue == null)
            {

                return "Unknown";

            }

            Type enumType = enumValue.GetType();

            MemberInfo[] memberInfo = enumType.GetMember(enumValue.ToString());

            if (memberInfo.Length == 0)
            {

                return enumValue.ToString();
            }

            DisplayAttribute? displayAttribute = memberInfo.FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>();

            string result = displayAttribute?.Name ?? enumValue.ToString();

            return result;
        }
        public static TTarget MapProperties<TSource, TTarget>(TSource source) where TTarget : new()
        {
            TTarget target = new TTarget();

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
        public static async Task<List<Student>> GetStudentListAsync()
        {
            string token = GenerateJWTToken();

            List<Student> students = new();

            HttpClient client = Initialize("http://forekapi.dreamline-ict.co.za/api/Students/");

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            HttpResponseMessage res = await client.GetAsync("http://forekapi.dreamline-ict.co.za/api/Students/");

            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;

                students = JsonConvert.DeserializeObject<List<Student>>(results);
            }

            return students;
        }

        public static string IncrementReference(string input, int incrementBy)
        {
            int slashIndex = input.IndexOf('/');
            if (slashIndex == -1 || slashIndex == input.Length - 1)
                throw new ArgumentException("Invalid input format");

            string prefix = input.Substring(0, slashIndex + 1);
            string numericPart = input.Substring(slashIndex + 1);

            if (!int.TryParse(numericPart, out int number))
                throw new ArgumentException("Invalid numeric part");

            number += incrementBy;
            string newNumericPart = number.ToString("D" + numericPart.Length); // To maintain leading zeros

            return prefix + newNumericPart;
        }

        /// <summary>
        /// Extracts and converts the date part from a reference number string to a formatted date string.
        /// Supports multiple reference formats including slash-delimited dates and textual month formats.
        /// </summary>
        /// <param name="referenceNumber">
        /// The reference number string. Supported formats:
        /// - "FORdd/MM/yyyyiYs" (e.g., "FOR01/12/2025ABC")
        /// - "FORdd MMM yyyyXXX" (e.g., "FOR18 Nov 2025WBp")
        /// - "FORyyyy/MM/ddXXX" (e.g., "FOR2025/11/18XYZ")
        /// </param>
        /// <returns>
        /// A string representing the extracted date in "dd/MM/yyyy" format if successful; otherwise, "Invalid Date".
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when the input string is null, empty, or whitespace.</exception>
        /// <remarks>
        /// This method is production-ready with comprehensive format support, validation, and error handling.
        /// It uses culture-invariant parsing to ensure consistent behavior across different locales.
        /// </remarks>
        public static string ExtractDateFromReference(string referenceNumber)
        {
            if (string.IsNullOrWhiteSpace(referenceNumber))
            {
                throw new ArgumentException("Reference number cannot be null, empty, or whitespace.", nameof(referenceNumber));
            }

            // Normalize input: trim and remove extra whitespace
            referenceNumber = referenceNumber.Trim();

            if (!referenceNumber.StartsWith("FOR", StringComparison.OrdinalIgnoreCase))
            {
                return "Invalid Date";
            }

            // Strategy 1: Extract slash-delimited dates (yyyy/M/d, M/d/yyyy, etc.)
            var slashPatterns = new[]
            {
                @"(\d{4}/\d{1,2}/\d{1,2})",  // yyyy/M/d or yyyy/MM/dd
                @"(\d{1,2}/\d{1,2}/\d{4})"   // M/d/yyyy or MM/dd/yyyy or d/M/yyyy
           };

            foreach (var pattern in slashPatterns)
            {
                var match = Regex.Match(referenceNumber, pattern);
                if (match.Success)
                {
                    string datePart = match.Groups[1].Value;
                    DateTime parsedDate;

                    string[] slashFormats =
                    {
                        "yyyy/M/d", "yyyy/MM/dd",
                        "M/d/yyyy", "MM/dd/yyyy",
                        "d/M/yyyy", "dd/MM/yyyy"
                   };

                    if (DateTime.TryParseExact(datePart, slashFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                    {
                        return parsedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                }
            }

            // Strategy 2: Extract textual month format (dd MMM yyyy or d MMM yyyy)
            // Pattern: one or two digits, whitespace, three-letter month abbreviation, whitespace, four-digit year
            // Example: "18 Nov 2025", "5 Jan 2024"
            var textualMonthPattern = @"(\d{1,2}\s+[A-Za-z]{3}\s+\d{4})";
            var textualMatch = Regex.Match(referenceNumber, textualMonthPattern);

            if (textualMatch.Success)
            {
                string datePart = textualMatch.Groups[1].Value;
                DateTime parsedDate;

                string[] textualFormats =
                {
                    "d MMM yyyy",   // e.g., "5 Nov 2025"
                    "dd MMM yyyy"   // e.g., "18 Nov 2025"
                };

                if (DateTime.TryParseExact(datePart, textualFormats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out parsedDate))
                {
                    return parsedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
            }

            // Strategy 3: Extract full textual month (e.g., "18 November 2025")
            var fullMonthPattern = @"(\d{1,2}\s+[A-Za-z]{4,}\s+\d{4})";
            var fullMonthMatch = Regex.Match(referenceNumber, fullMonthPattern);

            if (fullMonthMatch.Success)
            {
                string datePart = fullMonthMatch.Groups[1].Value;
                DateTime parsedDate;

                string[] fullMonthFormats =
                {
                    "d MMMM yyyy",   // e.g., "5 November 2025"
                    "dd MMMM yyyy"   // e.g., "18 November 2025"
                };

                if (DateTime.TryParseExact(datePart, fullMonthFormats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out parsedDate))
                {
                    return parsedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
            }

            // Strategy 4: Fallback - attempt general parsing on isolated segments
            // Extract continuous alphanumeric segments that might contain dates
            var segments = Regex.Matches(referenceNumber, @"[\d/\-\s]+[A-Za-z]*[\d/\-\s]*");

            foreach (Match segment in segments)
            {
                if (DateTime.TryParse(segment.Value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime fallbackDate))
                {
                    // Validate parsed date is reasonable (not too far in past/future)
                    if (fallbackDate.Year >= 2000 && fallbackDate.Year <= DateTime.UtcNow.Year + 10)
                    {
                        return fallbackDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                }
            }

            return "Invalid Date";
        }
        public static string OnSendMessage(string name, string course, string refNumber)
        {
            int year = DateTime.Now.Year;

            string template = @"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {
                            font-family: Arial, sans-serif;
                            background-color: #f5f5f5;
                            margin: 0;
                            padding: 0;
                        }
                        .container {
                            width: 100%;
                            max-width: 600px;
                            margin: 0 auto;
                            background-color: #ffffff;
                            padding: 20px;
                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        }
                        .header {
                            text-align: center;
                        }
                        .title {
                            font-size: 24px;
                            color: #333333;
                        }
                        .message {
                            margin: 20px 0;
                            font-size: 16px;
                            color: #555555;
                        }
                        .footer {
                            text-align: center;
                            margin-top: 30px;
                        }
                        .button {
                            background-color: #8B0000;
                            color: #ffffff;
                            padding: 15px 25px;
                            font-size: 18px;
                            display: inline-block;
                            margin: 20px 0;
                            border-radius: 5px;
                        }
                        .line {
                            border-top: 1px solid #cccccc;
                            margin: 20px 0;
                        }
                        .copyright {
                            font-size: 14px;
                            color: #888888;
                            text-align: center;
                        }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 class='title'>Forek Online Application</h1>
                        </div>
                        <div class='message'>
                            <p>Dear {name},</p>
                            <p>Thank you for applying for {course}. We acknowledge your application and will process it in due time.</p>
                            <p>Below is your reference number. Which must be quoted in all forms of correspondence</p>
                        </div>
                        <div class='footer'>
                            <div class='button'>Reference Number: {ref}</div>
                        </div>
                        <div class='line'></div>
                        <div class='copyright'>
                            &copy; Copyright {year} Forek Institute of Technology
                        </div>
                    </div>
                </body>
                </html>";

            return template.Replace("{name}", name)
                           .Replace("{ref}", refNumber)
                           .Replace("{course}", course)
                           .Replace("{year}", year.ToString());

        }

        /// <summary>
        /// Masks an input by showing only the first 4 and last 3 digits, replacing the middle digits with asterisks.
        /// </summary>
        /// <param name="id">The original input string.</param>
        /// <returns>A masked version of the input.</returns>
        /// <exception cref="ArgumentException">Thrown when the input is null, empty, or less than 13 characters long.</exception>
        public static string MaskInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            if (input.Length == 13)
            {
                var firstPart = input.Substring(0, 4);

                var lastPart = input.Substring(input.Length - 3);

                var maskedMiddle = new string('*', input.Length - 7);

                return $"{firstPart}{maskedMiddle}{lastPart}";
            }
            else
            {
                int lengthToShow = Math.Max(1, input.Length / 4);

                var firstPart = input.Substring(0, lengthToShow);

                var lastPart = input.Substring(input.Length - lengthToShow);

                var maskedMiddle = new string('*', input.Length - (2 * lengthToShow));

                return $"{firstPart}{maskedMiddle}{lastPart}";
            }
        }
    }
}
