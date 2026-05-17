using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElecPOE.Controllers
{
    public class CommunicationController : Controller
    {
        public readonly IStudentService _studentService;
        public readonly ICourseService _courseService;
        public readonly IHelperService _helperService;
        private readonly IUnitOfWork _context;


        public CommunicationController(IStudentService studentService, ICourseService courseService, IHelperService helperService, IUnitOfWork context)
        {
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        [HttpGet]
        public async Task<IActionResult> CommunicationCourses(string? q = null, int take = 50)
        {
            take = take <= 0 ? 50 : Math.Min(take, 200);

            var courses = await _context.Courses.GetAllAsync();

            var filtered = courses
                .Where(c => c != null && c.IsActive)
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                filtered = filtered.Where(c =>
                    (c.CourseName ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            var items = filtered
                .OrderBy(c => c.CourseName)
                .Take(take)
                .Select(c => new CommunicationCourseItem(
                    CourseId: c.CourseId,
                    CourseName: c.CourseName ?? string.Empty))
                .ToList();

            return Json(items);
        }

        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        [HttpGet]
        public async Task<IActionResult> CommunicationStudents(string? q = null, Guid? courseId = null, int page = 1, int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 5, 100);

            List<Student> students;

            if (courseId.HasValue && courseId.Value != Guid.Empty)
            {
                students = await _studentService.GetStudentsByCourseAsync(courseId.Value, onlyActive: true);
            }
            else
            {
                students = await _studentService.GetStudentListAsync();
            }

            IEnumerable<Student> filtered = students;

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();

                filtered = filtered.Where(s =>
                    ($"{s.FirstName} {s.LastName}").Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (s.Email ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (s.StudentNumber ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (s.Cellphone ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase));
            }
            var total = filtered.Count();

            var items = filtered
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new CommunicationStudentLookupItem(
                    StudentId: s.StudentId,
                    StudentNumber: s.StudentNumber ?? string.Empty,
                    FullName: $"{s.FirstName} {s.LastName}".Trim(),
                    Email: s.Email,
                    Cellphone: s.Cellphone))
                .ToList();

            return Json(new
            {
                page,
                pageSize,
                totalCount = total,
                items
            });
        }

        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCommunication([FromBody] CommunicationSendRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var channel = (request.Channel ?? string.Empty).Trim().ToLowerInvariant();

            if (channel is not ("email" or "sms"))
                return BadRequest("Invalid Channel. Use 'email' or 'sms'.");

            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("Message is required.");

            if (channel == "email" && string.IsNullOrWhiteSpace(request.Subject))
                return BadRequest("Subject is required for email.");

            if (!string.Equals(request.RecipientType, "student", StringComparison.OrdinalIgnoreCase))
                return BadRequest("RecipientType 'guardian' is not wired yet. Use 'student' for now.");

            var targetStudentNumbers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (request.StudentNumbers is { Count: > 0 })
            {
                foreach (var x in request.StudentNumbers)
                {
                    if (!string.IsNullOrWhiteSpace(x))
                        targetStudentNumbers.Add(x.Trim());
                }
            }
            List<Student> students;

            if (targetStudentNumbers.Count > 0)
            {
                students = await _studentService.GetStudentListAsync();
                students = students
                    .Where(s => !string.IsNullOrWhiteSpace(s.StudentNumber) && targetStudentNumbers.Contains(s.StudentNumber.Trim()))
                    .ToList();
            }
            else if (request.CourseId.HasValue && request.CourseId.Value != Guid.Empty)
            {
                students = await _studentService.GetStudentsByCourseAsync(request.CourseId.Value, onlyActive: true);
            }
            else
            {
                return BadRequest("Select recipients by StudentNumbers or provide a CourseId.");
            }

            if (students.Count == 0)
                return BadRequest("No matching students were found for the selected audience.");

            var invalidRecipients = new List<string>();
            var sendCount = 0;
            if (channel == "email")
            {
                var emailTargets = students
                    .Where(s => !string.IsNullOrWhiteSpace(s.Email))
                    .Select(s => s.Email!.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (emailTargets.Count == 0)
                    return BadRequest("Selected students have no email addresses.");

                var tasks = emailTargets.Select(email =>
                {
                    var mail = new EmailDataViewModel
                    {
                        Recipient = email,
                        Header = "Communication",
                        Subject = request.Subject!.Trim(),
                        Body = request.Message.Trim(),
                        From = "Forek Online"
                    };

                    return _helperService.SendMailNotificationAsync(mail);
                });

                await Task.WhenAll(tasks);
                sendCount = emailTargets.Count;
            }
            else if (channel == "sms")
            {
                foreach (var s in students)
                {
                    var phone = (s.Cellphone ?? string.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(phone))
                    {
                        invalidRecipients.Add(s.StudentNumber ?? s.StudentId.ToString());
                        continue;
                    }

                    _helperService.SendSms(request.Message.Trim(), phone);
                    sendCount++;
                }

                if (sendCount == 0)
                    return BadRequest("Selected students have no cellphone numbers.");
            }

            return Ok(new
            {
                attempted = students.Count,
                sent = sendCount,
                invalid = invalidRecipients.Count,
                invalidRecipients
            });
        }
    }
}
