// <copyright file="StudentCacheRefreshServiceTest.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace Forek.Test
{
    public class StudentCacheRefreshServiceTest
    {
        [Fact]
        public async Task RefreshFromApiAsync_HydratesDetailsAndWritesBothTables()
        {
            var studentId = Guid.NewGuid();
            var enrollmentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var listStudent = new Student
            {
                StudentId = studentId,
                StudentNumber = "FIT-2026-0001",
                FirstName = "Cache",
                LastName = "Test"
            };
            var detailStudent = new Student
            {
                StudentId = studentId,
                StudentNumber = listStudent.StudentNumber,
                FirstName = listStudent.FirstName,
                LastName = listStudent.LastName,
                EnrollmentHistory = new List<EnrollmentHistory>
                {
                    new()
                    {
                        EnrollmentId = enrollmentId,
                        StudentId = studentId,
                        CourseId = courseId,
                        CourseTitle = "Electrical",
                        CourseType = "Occupational",
                        EnrollmentStatus = "Active",
                        StartDate = DateTime.UtcNow.Date,
                        IsActive = true
                    }
                }
            };

            var handler = new StubHttpMessageHandler(request =>
            {
                object payload = request.RequestUri!.AbsolutePath.EndsWith("/Students", StringComparison.Ordinal)
                    ? new List<Student> { listStudent }
                    : detailStudent;
                return JsonResponse(payload);
            });
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(handler));

            var helper = new Mock<IHelperService>();
            helper.Setup(service => service.GetConfigurationValue("AppSettings:ApiBaseAddress", "defaultApiBaseAddress"))
                .Returns("https://legacy.example/api/");
            helper.Setup(service => service.GenerateJwtToken()).ReturnsAsync("token");

            List<Student>? savedStudents = null;
            var cacheStore = new Mock<IStudentCacheStore>();
            cacheStore
                .Setup(store => store.SyncStudentsAsync(It.IsAny<List<Student>>(), true))
                .Callback<List<Student>, bool>((students, _) => savedStudents = students)
                .Returns(Task.CompletedTask);
            cacheStore.Setup(store => store.GetStatusAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StudentCacheStatusViewModel
                {
                    StudentCount = 1,
                    EnrollmentHistoryCount = 1
                });

            var service = new StudentCacheRefreshService(
                httpClientFactory.Object,
                helper.Object,
                cacheStore.Object,
                Mock.Of<ILogger<StudentCacheRefreshService>>());

            var result = await service.RefreshFromApiAsync();

            Assert.Equal(1, result.StudentCount);
            Assert.Equal(1, result.EnrollmentHistoryCount);
            Assert.Equal(1, result.DetailRecordsLoaded);
            Assert.Equal(0, result.SkippedRecordCount);
            Assert.Empty(result.SkippedRecordExamples);
            Assert.NotNull(savedStudents);
            Assert.Single(savedStudents!);
            Assert.Single(savedStudents![0].EnrollmentHistory!);
            cacheStore.Verify(store => store.SyncStudentsAsync(It.IsAny<List<Student>>(), true), Times.Once);
        }

        [Fact]
        public async Task RefreshFromApiAsync_AcceptsLegacyAdmissionCategoryValues()
        {
            const string studentNumber = "FIT-2026-0003";
            var studentId = Guid.NewGuid();
            var handler = new StubHttpMessageHandler(request =>
            {
                var admissionCategory = request.RequestUri!.AbsolutePath.EndsWith("/Students", StringComparison.Ordinal)
                    ? "null"
                    : "\"Part Time\"";
                var payload =
                    $$"""
                    {
                      "StudentId": "{{studentId}}",
                      "StudentNumber": "{{studentNumber}}",
                      "FirstName": "Legacy",
                      "LastName": "Category",
                      "AdmissionCategory": {{admissionCategory}}
                    }
                    """;

                if (request.RequestUri.AbsolutePath.EndsWith("/Students", StringComparison.Ordinal))
                {
                    payload = $"[{{payload}}]";
                }

                return RawJsonResponse(payload);
            });
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(handler));

            var helper = new Mock<IHelperService>();
            helper.Setup(service => service.GetConfigurationValue("AppSettings:ApiBaseAddress", "defaultApiBaseAddress"))
                .Returns("https://legacy.example/api/");
            helper.Setup(service => service.GenerateJwtToken()).ReturnsAsync("token");

            List<Student>? savedStudents = null;
            var cacheStore = new Mock<IStudentCacheStore>();
            cacheStore
                .Setup(store => store.SyncStudentsAsync(It.IsAny<List<Student>>(), true))
                .Callback<List<Student>, bool>((students, _) => savedStudents = students)
                .Returns(Task.CompletedTask);
            cacheStore.Setup(store => store.GetStatusAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StudentCacheStatusViewModel { StudentCount = 1 });

            var service = new StudentCacheRefreshService(
                httpClientFactory.Object,
                helper.Object,
                cacheStore.Object,
                Mock.Of<ILogger<StudentCacheRefreshService>>());

            await service.RefreshFromApiAsync();

            Assert.NotNull(savedStudents);
            Assert.Equal(eAdmissionCategory.PartTime, Assert.Single(savedStudents!).AdmissionCategory);
        }

        [Fact]
        public async Task RefreshFromApiAsync_AcceptsLegacyFormatsAcrossStudentEnums()
        {
            const string studentNumber = "FIT-2026-0004";
            var studentId = Guid.NewGuid();
            var guardianId = Guid.NewGuid();
            var documentId = Guid.NewGuid();
            var handler = new StubHttpMessageHandler(request =>
            {
                var isListRequest = request.RequestUri!.AbsolutePath.EndsWith("/Students", StringComparison.Ordinal);
                var payload = isListRequest
                    ? $$"""
                      [{
                        "StudentId": "{{studentId}}",
                        "StudentNumber": "{{studentNumber}}",
                        "FirstName": "Legacy",
                        "LastName": "Enums",
                        "Gender": null,
                        "Province": "western cape",
                        "AdmissionCategory": ""
                      }]
                      """
                    : $$"""
                      {
                        "StudentId": "{{studentId}}",
                        "StudentNumber": "{{studentNumber}}",
                        "FirstName": "Legacy",
                        "LastName": "Enums",
                        "Gender": "female",
                        "Province": "KwaZulu-Natal",
                        "AdmissionCategory": "Part Time",
                        "Placement": {
                          "PlacementId": "{{Guid.NewGuid()}}",
                          "Status": "Dropped Out"
                        },
                        "Guardian": {
                          "GuardianId": "{{guardianId}}",
                          "Relationship": ""
                        },
                        "Documents": [{
                          "StudentDocumentId": "{{documentId}}",
                          "StudentId": "{{studentId}}",
                          "DocumentType": "Highest Qualification"
                        }]
                      }
                      """;

                return RawJsonResponse(payload);
            });
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(handler));

            var helper = new Mock<IHelperService>();
            helper.Setup(service => service.GetConfigurationValue("AppSettings:ApiBaseAddress", "defaultApiBaseAddress"))
                .Returns("https://legacy.example/api/");
            helper.Setup(service => service.GenerateJwtToken()).ReturnsAsync("token");

            List<Student>? savedStudents = null;
            var cacheStore = new Mock<IStudentCacheStore>();
            cacheStore
                .Setup(store => store.SyncStudentsAsync(It.IsAny<List<Student>>(), true))
                .Callback<List<Student>, bool>((students, _) => savedStudents = students)
                .Returns(Task.CompletedTask);
            cacheStore.Setup(store => store.GetStatusAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StudentCacheStatusViewModel { StudentCount = 1 });

            var service = new StudentCacheRefreshService(
                httpClientFactory.Object,
                helper.Object,
                cacheStore.Object,
                Mock.Of<ILogger<StudentCacheRefreshService>>());

            await service.RefreshFromApiAsync();

            var savedStudent = Assert.Single(savedStudents!);
            Assert.Equal(eGender.Female, savedStudent.Gender);
            Assert.Equal(eProvince.KwaZuluNatal, savedStudent.Province);
            Assert.Equal(eAdmissionCategory.PartTime, savedStudent.AdmissionCategory);
            Assert.Equal(eStatus.DroppedOut, savedStudent.Placement!.Status);
            Assert.Equal(eRelationship.Other, savedStudent.Guardian!.Relationship);
            Assert.Equal(
                eStudentDocumentType.HighestQualification,
                Assert.Single(savedStudent.Documents!).DocumentType);
        }

        [Fact]
        public async Task RefreshFromApiAsync_UnknownEnumValuePreservesExistingCache()
        {
            const string studentNumber = "FIT-2026-0005";
            var studentId = Guid.NewGuid();
            var handler = new StubHttpMessageHandler(request =>
            {
                var gender = request.RequestUri!.AbsolutePath.EndsWith("/Students", StringComparison.Ordinal)
                    ? "null"
                    : "\"DefinitelyNotAGender\"";
                var payload =
                    $$"""
                    {
                      "StudentId": "{{studentId}}",
                      "StudentNumber": "{{studentNumber}}",
                      "FirstName": "Invalid",
                      "LastName": "Enum",
                      "Gender": {{gender}}
                    }
                    """;

                if (request.RequestUri.AbsolutePath.EndsWith("/Students", StringComparison.Ordinal))
                {
                    payload = $"[{{payload}}]";
                }

                return RawJsonResponse(payload);
            });
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(handler));

            var helper = new Mock<IHelperService>();
            helper.Setup(service => service.GetConfigurationValue("AppSettings:ApiBaseAddress", "defaultApiBaseAddress"))
                .Returns("https://legacy.example/api/");
            helper.Setup(service => service.GenerateJwtToken()).ReturnsAsync("token");

            var cacheStore = new Mock<IStudentCacheStore>();
            var service = new StudentCacheRefreshService(
                httpClientFactory.Object,
                helper.Object,
                cacheStore.Object,
                Mock.Of<ILogger<StudentCacheRefreshService>>());

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.RefreshFromApiAsync());

            Assert.Contains("Unknown eGender value", exception.Message);
            cacheStore.Verify(
                store => store.SyncStudentsAsync(It.IsAny<List<Student>>(), It.IsAny<bool>()),
                Times.Never);
        }

        [Fact]
        public async Task RefreshFromApiAsync_SkipsFailedDetailAndContinuesWithNextStudent()
        {
            var failedStudent = new Student
            {
                StudentId = Guid.NewGuid(),
                StudentNumber = "FIT-2026-FAIL",
                FirstName = "Failure",
                LastName = "Test"
            };
            var successfulStudent = new Student
            {
                StudentId = Guid.NewGuid(),
                StudentNumber = "FIT-2026-SUCCESS",
                FirstName = "Success",
                LastName = "Test"
            };
            var requestedStudentNumbers = new List<string>();

            var handler = new StubHttpMessageHandler(request =>
            {
                if (request.RequestUri!.AbsolutePath.EndsWith("/Students", StringComparison.Ordinal))
                {
                    return JsonResponse(new List<Student> { failedStudent, successfulStudent });
                }

                var studentNumber = WebUtility.UrlDecode(
                    request.RequestUri.Query.Split('=', 2, StringSplitOptions.TrimEntries)[1]);
                requestedStudentNumbers.Add(studentNumber);
                return studentNumber == failedStudent.StudentNumber
                    ? new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    : JsonResponse(successfulStudent);
            });
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(handler));

            var helper = new Mock<IHelperService>();
            helper.Setup(service => service.GetConfigurationValue("AppSettings:ApiBaseAddress", "defaultApiBaseAddress"))
                .Returns("https://legacy.example/api/");
            helper.Setup(service => service.GenerateJwtToken()).ReturnsAsync("token");

            List<Student>? savedStudents = null;
            var cacheStore = new Mock<IStudentCacheStore>();
            cacheStore
                .Setup(store => store.SyncStudentsAsync(It.IsAny<List<Student>>(), true))
                .Callback<List<Student>, bool>((students, _) => savedStudents = students)
                .Returns(Task.CompletedTask);
            cacheStore.Setup(store => store.GetStatusAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StudentCacheStatusViewModel
                {
                    StudentCount = 1,
                    EnrollmentHistoryCount = 0
                });

            var service = new StudentCacheRefreshService(
                httpClientFactory.Object,
                helper.Object,
                cacheStore.Object,
                Mock.Of<ILogger<StudentCacheRefreshService>>());

            var result = await service.RefreshFromApiAsync();

            Assert.Equal(new[] { failedStudent.StudentNumber, successfulStudent.StudentNumber }, requestedStudentNumbers);
            Assert.Equal(1, result.DetailRecordsLoaded);
            Assert.Equal(1, result.SkippedRecordCount);
            Assert.Contains(failedStudent.StudentNumber, Assert.Single(result.SkippedRecordExamples));
            Assert.Equal(successfulStudent.StudentNumber, Assert.Single(savedStudents!).StudentNumber);
            cacheStore.Verify(store => store.SyncStudentsAsync(It.IsAny<List<Student>>(), true), Times.Once);
        }

        [Fact]
        public async Task RefreshFromApiAsync_AllDetailFailuresPreserveExistingCache()
        {
            var listStudent = new Student
            {
                StudentId = Guid.NewGuid(),
                StudentNumber = "FIT-2026-0002",
                FirstName = "Failure",
                LastName = "Test"
            };

            var handler = new StubHttpMessageHandler(request =>
            {
                if (request.RequestUri!.AbsolutePath.EndsWith("/Students", StringComparison.Ordinal))
                {
                    return JsonResponse(new List<Student> { listStudent });
                }

                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            });
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(handler));

            var helper = new Mock<IHelperService>();
            helper.Setup(service => service.GetConfigurationValue("AppSettings:ApiBaseAddress", "defaultApiBaseAddress"))
                .Returns("https://legacy.example/api/");
            helper.Setup(service => service.GenerateJwtToken()).ReturnsAsync("token");

            var cacheStore = new Mock<IStudentCacheStore>();
            var service = new StudentCacheRefreshService(
                httpClientFactory.Object,
                helper.Object,
                cacheStore.Object,
                Mock.Of<ILogger<StudentCacheRefreshService>>());

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.RefreshFromApiAsync());
            cacheStore.Verify(
                store => store.SyncStudentsAsync(It.IsAny<List<Student>>(), It.IsAny<bool>()),
                Times.Never);
        }

        private static HttpResponseMessage JsonResponse(object payload)
            => RawJsonResponse(JsonSerializer.Serialize(payload));

        private static HttpResponseMessage RawJsonResponse(string payload)
            => new(HttpStatusCode.OK)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

        private sealed class StubHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

            public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
            {
                _responseFactory = responseFactory;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
                => Task.FromResult(_responseFactory(request));
        }
    }
}
