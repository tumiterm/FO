// <copyright file="EvidenceController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    03/01/2025 14:09:27 PM
// Purpose:         Defines the EvidenceController class


#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
#endregion

namespace ElecPOE.Controllers
{
    /// <summary>
    /// Controller responsible for handling learner assessments, including fetching and displaying assessment data.
    /// </summary>
    public class LearnerAssessmentController : Controller
    {
        /// <summary>
        /// The unit of work interface used to interact with data repositories.
        /// </summary>
        private readonly IUnitOfWork _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnerAssessmentController"/> class.
        /// </summary>
        /// <param name="context">The unit of work used for database operations.</param>
        public LearnerAssessmentController(IUnitOfWork context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all learner assessments from the database and prepares them for display.
        /// </summary>
        /// <returns>A view displaying the list of learner assessments.</returns>
        [HttpGet]
        public async Task<IActionResult> StudAssessments()
        {
            var list1 = await _context.Assessments.GetAllAsync();
            List<LearnerAssessmentViewModel> list2 = new();

            LearnerAssessmentViewModel? assessmentDTO = null;
           
            foreach (var item in list1) 
            {
                assessmentDTO = new LearnerAssessmentViewModel
                {
                    Assessment = new AssessmentViewModel
                    {
                        Module = item.Module,

                        Student = await GetStudent(item.StudentNumber),

                        CreatedBy= item.CreatedBy,

                        CreatedOn= item.CreatedOn,

                        Type= item.Type,
                    } 
                };

                list2.Add(assessmentDTO);   
            }

            return View(list2);
        }

        /// <summary>
        /// Retrieves a student's information by their student number from an external API.
        /// </summary>
        /// <param name="StudentNumber">The student number used to identify the student.</param>
        /// <returns>A string containing the student's full name and course information.</returns>
        private async Task<string> GetStudent(string StudentNumber)
        {
            string res = string.Empty;

            Student student = new Student();

            var client = new RestClient($"http://forekapi.dreamline-ict.co.za/api/StudentId?StudentNumber={StudentNumber}");

            var request = new RestRequest();

            request.AddParameter("StudentNumber", StudentNumber);

            request.AddHeader("Authorization", $"Bearer {Helper.GenerateJWTToken()}");

            var response = await client.ExecuteGetAsync(request);

            if (response.IsSuccessful)
            {
                res = response.Content.ToString();

                student = JsonConvert.DeserializeObject<Student>(res);

                EnrollmentHistoryViewModel enrollment = new EnrollmentHistoryViewModel
                {
                    CourseId = student.EnrollmentHistory[0].CourseId,

                    StartDate = student.EnrollmentHistory[0].StartDate,

                    CourseTitle = student.EnrollmentHistory[0].CourseTitle,

                    StudentId = student.EnrollmentHistory[0].StudentId,

                    CourseType = student.EnrollmentHistory[0].CourseType,

                    EnrollmentId = student.EnrollmentHistory[0].EnrollmentId,

                    EnrollmentStatus = student.EnrollmentHistory[0].EnrollmentStatus,

                    IsActive = student.EnrollmentHistory[0].IsActive

                };
            }

            return $"{student.FirstName} {student.LastName} [{student.EnrollmentHistory.First().CourseTitle}]";
        }

    }
}
