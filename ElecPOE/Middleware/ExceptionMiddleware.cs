// <copyright file="ExceptionMiddleware.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    08/01/2025 10:08:27 PM
// Purpose:         Defines the ExceptionMiddleware class

#region Usings
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Primitives;
using static System.Runtime.InteropServices.JavaScript.JSType;
#endregion

namespace ElecPOE.Middleware
{
    /// <summary>
    /// Production-ready global exception middleware with rich contextual logging,
    /// correlation, controller/action resolution and structured email notifications.
    /// </summary>
    public sealed class ExceptionMiddleware
    {
        #region Private
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        #endregion

        /// <summary>
        /// Middleware that handles exceptions occurring during the request pipeline execution.
        /// </summary>
        /// <remarks>This middleware intercepts unhandled exceptions, logs them, and generates an
        /// appropriate HTTP response. It is typically added to the middleware pipeline to provide centralized exception
        /// handling.</remarks>
        /// <param name="next">The next middleware in the request pipeline.</param>
        /// <param name="logger">The logger instance used to log exception details.</param>
        /// <param name="env">The hosting environment, used to determine the application's runtime environment.</param>
        /// <param name="config">The application configuration, used to access configuration settings.</param>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,IWebHostEnvironment env, IConfiguration config)
        {
            _next = next;
            _logger = logger;
            _env = env;
            _config = config;
        }

        /// <summary>
        /// Processes an HTTP request, adding a correlation ID to the response headers and handling exceptions globally.
        /// </summary>
        /// <remarks>This method ensures that each HTTP request is assigned a unique correlation ID, which
        /// is added to the response headers under the "X-Correlation-ID" key. If an unhandled exception occurs during
        /// the request pipeline, it is logged and handled gracefully, unless the response has already started, in which
        /// case the exception is rethrown.</remarks>
        /// <param name="context">The <see cref="HttpContext"/> representing the current HTTP request and response.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var start = DateTime.UtcNow;

            string correlationId = GetOrCreateCorrelationId(context);
            context.Response.Headers["X-Correlation-ID"] = correlationId;

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogError(ex, "Response already started. Cannot apply global exception handling. Correlation {CorrelationId}", correlationId);
                    throw;
                }

                await HandleExceptionAsync(context, ex, correlationId, start);
            }
        }

        /// <summary>
        /// Handles unhandled exceptions that occur during the processing of an HTTP request. Logs the exception
        /// details, generates a unique ticket number for reference, and provides an appropriate response to the client
        /// based on the request type (API or non-API).
        /// </summary>
        /// <remarks>This method performs the following actions: <list type="bullet"> <item>Logs the
        /// exception details, including contextual information such as the controller, action, and request
        /// metadata.</item> <item>Generates a unique ticket number to reference the error in logs and user-facing
        /// messages.</item> <item>Sends an email notification with detailed exception information for further
        /// investigation.</item> <item>Determines the response format based on the request type: <list type="bullet">
        /// <item>For API requests, returns a JSON response conforming to RFC 7807 Problem Details.</item> <item>For
        /// non-API requests, redirects the user to a generic error page.</item> </list> </item> </list></remarks>
        /// <param name="context">The <see cref="HttpContext"/> representing the current HTTP request and response.</param>
        /// <param name="exception">The <see cref="Exception"/> that was thrown during the request processing.</param>
        /// <param name="correlationId">A unique identifier for correlating logs and diagnostics across the request lifecycle.</param>
        /// <param name="startUtc">The UTC timestamp indicating when the request processing started.</param>
        /// <returns></returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId, DateTime startUtc)
        {
            string ticketNumber = $"T-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

            var endpoint = context.GetEndpoint();
            string? controllerName = null;
            string? actionName = null;

            if (endpoint != null)
            {
                var cad = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                controllerName = cad?.ControllerName;
                actionName = cad?.ActionName;
            }

            string bodySnippet = await ReadRequestBodySnippetAsync(context);

            string exceptionChain = BuildExceptionChain(exception);

            _logger.LogError(exception,
                "Unhandled exception. Ticket {TicketNumber} Correlation {CorrelationId} Machine {Machine} Env {Environment} Path {Path} Query {Query} Method {Method} Controller {Controller} Action {Action} RemoteIP {RemoteIP} UserAgent {UserAgent}",
                ticketNumber,
                correlationId,
                Environment.MachineName,
                _env.EnvironmentName,
                context.Request.Path,
                context.Request.QueryString.ToString(),
                context.Request.Method,
                controllerName,
                actionName,
                context.Connection.RemoteIpAddress?.ToString(),
                context.Request.Headers.UserAgent.ToString()
            );

            _ = SendExceptionEmailSafeAsync(ticketNumber, correlationId, exceptionChain, exception, context, controllerName, actionName, bodySnippet, startUtc);

            if (context.Items["TempData"] is ITempDataDictionary tempData)
            {
                tempData["error"] = $"Something went wrong. Reference: {ticketNumber}";
            }

            bool isApiRequest =
                context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase) ||
                context.Request.Headers.Accept.Any(h => h.Contains("application/json", StringComparison.OrdinalIgnoreCase));

            context.Response.Clear();

            if (isApiRequest)
            {
                // RFC 7807 ProblemDetails
                var problem = new ProblemDetails
                {
                    Title = "An unexpected error occurred.",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "The team has been notified. Use the reference for support.",
                    Instance = context.Request.Path
                };

                problem.Extensions["ticket"] = ticketNumber;
                problem.Extensions["correlationId"] = correlationId;

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problem);
            }
            else
            {
                context.Items["error"] = $"Unexpected error. Ticket: {ticketNumber}";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.Redirect("/Home/Error");
            }
        }

        /// <summary>
        /// Reads a snippet of the HTTP request body as a string, starting from the beginning of the body.
        /// </summary>
        /// <remarks>This method ensures the request body can be read by enabling buffering if necessary.
        /// The request body position is reset to the beginning after reading, allowing subsequent middleware or
        /// components to access the body.</remarks>
        /// <param name="context">The <see cref="HttpContext"/> representing the current HTTP request.</param>
        /// <returns>A string containing the first 1024 characters of the request body, or a truncated snippet if the body
        /// exceeds this length. Returns "<empty>" if the body is empty, or "<unavailable>" if an error occurs while
        /// reading the body.</returns>
        private async Task<string> ReadRequestBodySnippetAsync(HttpContext context)
        {
            try
            {
                if (context.Request.Body == null || !context.Request.Body.CanSeek)
                {
                    context.Request.EnableBuffering();
                }

                context.Request.Body.Position = 0;
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                char[] buffer = new char[1024];
                int read = await reader.ReadAsync(buffer, 0, buffer.Length);
                context.Request.Body.Position = 0;

                var snippet = new string(buffer, 0, read);
                if (snippet.Length == 0) return "<empty>";
                if (snippet.Length == buffer.Length) snippet += "...";
                return snippet;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to read request body snippet.");
                return "<unavailable>";
            }
        }

        /// <summary>
        /// Constructs a detailed string representation of an exception and its inner exceptions.
        /// </summary>
        /// <param name="ex">The root <see cref="Exception"/> to process. Cannot be <see langword="null"/>.</param>
        /// <returns>A string containing the full exception chain, including the type, message, and stack trace of each exception
        /// in the chain. Each exception is prefixed with its depth in the chain.</returns>
        private string BuildExceptionChain(Exception ex)
        {
            var sb = new StringBuilder();
            int depth = 0;
            while (ex != null)
            {
                sb.AppendLine($"[{depth}] {ex.GetType().FullName}: {ex.Message}");
                sb.AppendLine(ex.StackTrace);
                ex = ex.InnerException;
                depth++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Sends an email notification containing detailed information about an unhandled exception.
        /// </summary>
        /// <remarks>This method attempts to send an email with exception details to a configured
        /// recipient. If the SMTP configuration is incomplete or an error occurs during email transmission, the
        /// exception is logged, and the email is not sent.</remarks>
        /// <param name="ticketNumber">The unique ticket number associated with the exception.</param>
        /// <param name="correlationId">The correlation ID used to trace the request across systems.</param>
        /// <param name="exceptionChain">A detailed string representation of the exception chain.</param>
        /// <param name="exception">The exception object that triggered the notification.</param>
        /// <param name="context">The HTTP context of the request during which the exception occurred.</param>
        /// <param name="controller">The name of the controller where the exception occurred, or <c>null</c> if unavailable.</param>
        /// <param name="action">The name of the action where the exception occurred, or <c>null</c> if unavailable.</param>
        /// <param name="bodySnippet">A snippet of the request body, if available, to provide additional context.</param>
        /// <param name="startUtc">The UTC timestamp indicating when the request started.</param>
        /// <returns></returns>
        private async Task SendExceptionEmailSafeAsync(string ticketNumber, string correlationId, string exceptionChain, Exception exception,
                                                       HttpContext context, string? controller, string? action, string bodySnippet, DateTime startUtc)
        {
            try
            {
                //if (!_env.IsProduction())
                //{
                //    return;
                //}

                var appVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "unknown";
                var durationMs = (DateTime.UtcNow - startUtc).TotalMilliseconds;

                string emailBody = $@"
                    <h2 style='font-family:Segoe UI'>Unhandled Exception Notification</h2>
                    <table cellpadding='6' style='font-family:Segoe UI;font-size:13px;border-collapse:collapse'>
                        <tr><td><strong>Ticket</strong></td><td>{ticketNumber}</td></tr>
                        <tr><td><strong>Correlation</strong></td><td>{correlationId}</td></tr>
                        <tr><td><strong>Timestamp (UTC)</strong></td><td>{DateTime.UtcNow:O}</td></tr>
                        <tr><td><strong>Environment</strong></td><td>{_env.EnvironmentName}</td></tr>
                        <tr><td><strong>Machine</strong></td><td>{Environment.MachineName}</td></tr>
                        <tr><td><strong>App Version</strong></td><td>{appVersion}</td></tr>
                        <tr><td><strong>Request Path</strong></td><td>{context.Request.Path}</td></tr>
                        <tr><td><strong>Query</strong></td><td>{context.Request.QueryString}</td></tr>
                        <tr><td><strong>Method</strong></td><td>{context.Request.Method}</td></tr>
                        <tr><td><strong>Controller</strong></td><td>{controller ?? "(n/a)"}</td></tr>
                        <tr><td><strong>Action</strong></td><td>{action ?? "(n/a)"}</td></tr>
                        <tr><td><strong>Remote IP</strong></td><td>{context.Connection.RemoteIpAddress}</td></tr>
                        <tr><td><strong>User Agent</strong></td><td>{WebUtility.HtmlEncode(context.Request.Headers.UserAgent.ToString())}</td></tr>
                        <tr><td><strong>Accept-Language</strong></td><td>{context.Request.Headers.AcceptLanguage}</td></tr>
                        <tr><td><strong>Duration (ms)</strong></td><td>{durationMs:F2}</td></tr>
                        <tr><td><strong>Body Snippet</strong></td><td><pre style='white-space:pre-wrap'>{WebUtility.HtmlEncode(bodySnippet)}</pre></td></tr>
                        <tr><td><strong>Authenticated User</strong></td><td>{(context.User?.Identity?.IsAuthenticated == true ? context.User.Identity.Name : "(anonymous)")}</td></tr>
                    </table>
                    <h3 style='font-family:Segoe UI'>Exception Chain</h3>
                    <pre style='background:#f5f5f5;padding:10px;font-size:11px;border:1px solid #ddd;'>{WebUtility.HtmlEncode(exceptionChain)}</pre>
                ";

                var recipient = _config["EmailAccounts:logs"] ?? "errorlogs@forekict.com";
                var senderAddress = _config["EmailAccounts:SystemAccount"];
                var senderPassword = _config["EmailAccounts:Password"];
                var smtpHost = _config["EmailAccounts:SmtpHost"];
                var smtpPort = _config.GetValue<int?>("EmailAccounts:SmtpPort") ?? 587;
                var enableSsl = _config.GetValue<bool?>("EmailAccounts:EnableSsl") ?? true;

                if (string.IsNullOrWhiteSpace(senderAddress) || string.IsNullOrWhiteSpace(senderPassword) || string.IsNullOrWhiteSpace(smtpHost))
                {
                    _logger.LogWarning("SMTP configuration incomplete. Skipping exception email for ticket {TicketNumber}.", ticketNumber);
                    return;
                }

                using var smtp = new SmtpClient(smtpHost)
                {
                    Port = smtpPort,
                    EnableSsl = enableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderAddress, senderPassword)
                };

                using var mailMessage = new MailMessage(
                    new MailAddress(senderAddress, "Forek Online Monitoring"),
                    new MailAddress(recipient))
                {
                    Subject = $"[ERROR] {ticketNumber} | {controller}/{action} | {_env.EnvironmentName}",
                    Body = emailBody,
                    IsBodyHtml = true
                };

                await smtp.SendMailAsync(mailMessage);
            }
            catch (Exception emailEx)
            {
                _logger.LogError(emailEx, "Failed sending exception email for ticket {TicketNumber}.", ticketNumber);
            }
        }

        /// <summary>
        /// Retrieves the correlation ID from the HTTP request headers or generates a new one if none is provided.
        /// </summary>
        /// <remarks>This method checks the "X-Correlation-ID" header in the HTTP request. If the header
        /// is present and contains a valid value,  that value is returned as the correlation ID. If the header is
        /// missing or empty, the method falls back to using the  <see cref="HttpContext.TraceIdentifier"/> as the
        /// correlation ID.</remarks>
        /// <param name="context">The <see cref="HttpContext"/> representing the current HTTP request.</param>
        /// <returns>The correlation ID retrieved from the "X-Correlation-ID" header if it exists and is not empty;  otherwise,
        /// the trace identifier associated with the current HTTP context.</returns>
        private string GetOrCreateCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Correlation-ID", out var headerVal) && !StringValues.IsNullOrEmpty(headerVal))
            {
                return headerVal.ToString();
            }
            return context.TraceIdentifier; 
        }
    }
}
