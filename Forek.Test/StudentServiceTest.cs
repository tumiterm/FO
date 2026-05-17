// <copyright file="StudentServiceTest.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    22/03/2025 13:03 PM
// Purpose:         Defines the StudentServiceTest class

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
#endregion

namespace Forek.Test
{
    /// <summary>
    /// Contains all the StudentService Related xUnit Tests
    /// </summary>
    public class StudentServiceTest
    {

        [Fact]
        public async Task GetCourseStatisticsAsync_ValidCourseId_ReturnsCorrectStatistics()
        {
            var courseId = Guid.Parse("ff755817-a3fe-423f-b382-17ace8f33f60");
            var students = GetSampleStudents();
            var mockHelper = new Mock<IHelperService>();
            var mockCache = new Mock<IMemoryCache>();
            var mockLogger = new Mock<ILogger<StudentService>>();
            var mockApiClient = new Mock<IStudentService>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockApiClient.Setup(client => client.GetStudentListAsync()).ReturnsAsync(students);
            var mockCacheStore = new Mock<IStudentCacheStore>();
            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

            var service = new StudentService(mockLogger.Object, mockHelper.Object, mockCache.Object, mockHttpClientFactory.Object, mockUnitOfWork.Object, mockCacheStore.Object, mockServiceScopeFactory.Object);

            var statistics = await service.GetCourseStatisticsAsync(courseId);

            Assert.NotNull(statistics);
            Assert.Equal(2, statistics["TotalEnrolled"]);
            Assert.Equal(0, statistics["DroppedOut"]);
        }

        [Fact]
        public async Task GetCourseStatisticsAsync_CourseWithNoStudents_ReturnsZeroStatistics()
        {
            var courseId = Guid.Parse("1dd7b11d-1399-46e6-946e-17baed14046aC999"); // Non-existent course
            var students = GetSampleStudents();
            var mockHelper = new Mock<IHelperService>();
            var mockCache = new Mock<IMemoryCache>();
            var mockLogger = new Mock<ILogger<StudentService>>();
            var mockApiClient = new Mock<IStudentService>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockApiClient.Setup(client => client.GetStudentListAsync()).ReturnsAsync(students);
            var mockCacheStore = new Mock<IStudentCacheStore>();
            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

            var service = new StudentService(mockLogger.Object, mockHelper.Object, mockCache.Object, mockHttpClientFactory.Object, mockUnitOfWork.Object, mockCacheStore.Object, mockServiceScopeFactory.Object);
            var statistics = await service.GetCourseStatisticsAsync(courseId);

            Assert.NotNull(statistics);
            Assert.Equal(0, statistics["TotalEnrolled"]);
            Assert.Equal(0, statistics["DroppedOut"]);
        }

        [Fact]
        public async Task GetCourseStatisticsAsync_EmptyCourseId_ThrowsArgumentException()
        {
            var courseId = Guid.Empty;
            var mockCache = new Mock<IMemoryCache>();
            var mockHelper = new Mock<IHelperService>();
            var mockLogger = new Mock<ILogger<StudentService>>();
            var mockApiClient = new Mock<IStudentService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCacheStore = new Mock<IStudentCacheStore>();
            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

            var service = new StudentService(mockLogger.Object, mockHelper.Object, mockCache.Object, mockHttpClientFactory.Object, mockUnitOfWork.Object, mockCacheStore.Object, mockServiceScopeFactory.Object);
            await Assert.ThrowsAsync<ArgumentException>(() => service.GetCourseStatisticsAsync(courseId));
        }

        [Fact]
        public async Task GetStudentsByCourseAsync_ValidCourseIdOnlyActive_ReturnsActiveStudents()
        {
            var mockHelper = new Mock<IHelperService>();
            var courseId = Guid.Parse(mockHelper.Setup(helper => helper.GetConfigurationValue("Courses:Occupational:Plumbing", string.Empty)).ToString());
            var students = GetSampleStudents();
            var mockCache = new Mock<IMemoryCache>();
            var mockLogger = new Mock<ILogger<StudentService>>();
            var mockApiClient = new Mock<IStudentService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockApiClient.Setup(client => client.GetStudentListAsync()).ReturnsAsync(students);
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCacheStore = new Mock<IStudentCacheStore>();
            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

            var service = new StudentService(mockLogger.Object, mockHelper.Object, mockCache.Object, mockHttpClientFactory.Object, mockUnitOfWork.Object, mockCacheStore.Object, mockServiceScopeFactory.Object);
            var result = await service.GetStudentsByCourseAsync(courseId, onlyActive: true);

            Assert.Single(result); 
            Assert.Equal("FIT20252879", result[0].StudentNumber);
        }

        [Fact]
        public async Task GetStudentsByCourseAsync_ValidCourseIdAllEnrollments_ReturnsAllStudents()
        {
            var courseId = Guid.Parse("e3ab09b1-eff9-49e3-aef7-0a4aef87a8f7");
            var students = GetSampleStudents();
            var mockHelper = new Mock<IHelperService>();
            var mockCache = new Mock<IMemoryCache>();
            var mockLogger = new Mock<ILogger<StudentService>>();
            var mockApiClient = new Mock<IStudentService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockApiClient.Setup(client => client.GetStudentListAsync()).ReturnsAsync(students);
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCacheStore = new Mock<IStudentCacheStore>();
            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

            var service = new StudentService(mockLogger.Object, mockHelper.Object, mockCache.Object, mockHttpClientFactory.Object, mockUnitOfWork.Object, mockCacheStore.Object, mockServiceScopeFactory.Object);
            var result = await service.GetStudentsByCourseAsync(courseId, onlyActive: false);

            Assert.Equal(2, result.Count); 
            Assert.Contains(result, s => s.StudentNumber == "FIT20252976");
            Assert.Contains(result, s => s.StudentNumber == "FIT20252971");
        }

        [Fact]
        public async Task GetStudentsByCourseAsync_EmptyCourseId_ThrowsArgumentException()
        {
            var courseId = Guid.Empty;
            var mockCache = new Mock<IMemoryCache>();
            var mockHelper = new Mock<IHelperService>();
            var mockLogger = new Mock<ILogger<StudentService>>();
            var mockApiClient = new Mock<IStudentService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCacheStore = new Mock<IStudentCacheStore>();
            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

            var service = new StudentService(mockLogger.Object, mockHelper.Object, mockCache.Object, mockHttpClientFactory.Object, mockUnitOfWork.Object, mockCacheStore.Object, mockServiceScopeFactory.Object);
            await Assert.ThrowsAsync<ArgumentException>(() => service.GetStudentsByCourseAsync(courseId, true));
        }

        [Fact]
        public async Task GetStudentStatisticsAsync_ValidStudentId_ReturnsCorrectStatistics()
        {
            var studentNumber = "FIT20252972";
            var students = GetSampleStudents();
            var mockHelper = new Mock<IHelperService>();
            var mockCache = new Mock<IMemoryCache>();
            var mockLogger = new Mock<ILogger<StudentService>>();
            var mockApiClient = new Mock<IStudentService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockApiClient.Setup(client => client.GetStudentAsync(studentNumber))
                         .ReturnsAsync(students.First(s => s.StudentNumber == studentNumber));

            var mockCacheStore = new Mock<IStudentCacheStore>();
            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

            var service = new StudentService(mockLogger.Object, mockHelper.Object, mockCache.Object, mockHttpClientFactory.Object, mockUnitOfWork.Object, mockCacheStore.Object, mockServiceScopeFactory.Object);
            var statistics = await service.GetStudentStatisticsAsync(studentNumber);

            Assert.NotNull(statistics);
            Assert.Equal(2, statistics["EnrollmentCount"]); 
            Assert.Equal(1, statistics["ActiveEnrollments"]); 
        }

        [Fact]
        public async Task GetStudentStatisticsAsync_StudentWithNoEnrollments_ReturnsZeroStatistics()
        {
            var studentNumber = "FIT20252970";
            var student = new Student { StudentId = Guid.Parse("ca4303ba-9389-4ca8-961d-0d03b463882d"), StudentNumber = studentNumber, EnrollmentHistory = new List<EnrollmentHistory>() };
            var mockCache = new Mock<IMemoryCache>();
            var mockHelper = new Mock<IHelperService>();
            var mockLogger = new Mock<ILogger<StudentService>>();
            var mockApiClient = new Mock<IStudentService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockApiClient.Setup(client => client.GetStudentAsync(studentNumber)).ReturnsAsync(student);
            var mockCacheStore = new Mock<IStudentCacheStore>();
            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

            var service = new StudentService(mockLogger.Object, mockHelper.Object, mockCache.Object, mockHttpClientFactory.Object, mockUnitOfWork.Object, mockCacheStore.Object, mockServiceScopeFactory.Object);
            var statistics = await service.GetStudentStatisticsAsync(studentNumber);

            Assert.NotNull(statistics);
            Assert.Equal(0, statistics["EnrollmentCount"]);
            Assert.Equal(0, statistics["ActiveEnrollments"]);
        }

        [Fact]
        public async Task GetStudentStatisticsAsync_EmptyStudentId_ThrowsArgumentException()
        {
            var studentId = string.Empty;
            var mockCache = new Mock<IMemoryCache>();
            var mockLogger = new Mock<ILogger<StudentService>>();
            var mockHelper = new Mock<IHelperService>();
            var mockApiClient = new Mock<IStudentService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCacheStore = new Mock<IStudentCacheStore>();
            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

            var service = new StudentService(mockLogger.Object, mockHelper.Object, mockCache.Object, mockHttpClientFactory.Object, mockUnitOfWork.Object, mockCacheStore.Object, mockServiceScopeFactory.Object);
            await Assert.ThrowsAsync<ArgumentException>(() => service.GetStudentStatisticsAsync(studentId));
        }

        [Fact]
        public async Task GetStudentListAsync_SuccessfulRetrieval_ReturnsStudentList()
        {
            var students = GetSampleStudents();
            var mockCache = new Mock<IMemoryCache>();
            var mockHelper = new Mock<IHelperService>();
            var mockLogger = new Mock<ILogger<StudentService>>();
            var mockApiClient = new Mock<IStudentService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
                mockApiClient.Setup(client => client.GetStudentListAsync()).ReturnsAsync(students);
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var mockCacheStore = new Mock<IStudentCacheStore>();
            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

            var service = new StudentService(mockLogger.Object, mockHelper.Object, mockCache.Object, mockHttpClientFactory.Object, mockUnitOfWork.Object, mockCacheStore.Object, mockServiceScopeFactory.Object);
            var result = await service.GetStudentListAsync();

            Assert.Equal(3, result.Count);
            Assert.Contains(result, s => s.StudentNumber == "FIT20252886");
            Assert.Contains(result, s => s.StudentNumber == "FIT20252879");
            Assert.Contains(result, s => s.StudentNumber == "FIT20252883");
        }

        [Fact]
        public async Task GetStudentListAsync_EmptyApiResponse_ReturnsEmptyList()
        {
            var mockCache = new Mock<IMemoryCache>();
            var mockLogger = new Mock<ILogger<StudentService>>();
            var mockHelper = new Mock<IHelperService>();
            var mockApiClient = new Mock<IStudentService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockApiClient.Setup(client => client.GetStudentListAsync()).ReturnsAsync(new List<Student>());
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCacheStore = new Mock<IStudentCacheStore>();
            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

            var service = new StudentService(mockLogger.Object, mockHelper.Object, mockCache.Object, mockHttpClientFactory.Object, mockUnitOfWork.Object, mockCacheStore.Object, mockServiceScopeFactory.Object);
            var result = await service.GetStudentListAsync();

            Assert.Empty(result);
        }

        private List<Student> GetSampleStudents()
        {
            return new List<Student>
            {
                new Student
                {
                    StudentId = Guid.Parse("224631f2-156a-45cf-a71e-680c054c352"),
                    StudentNumber = "FIT20252848",
                    EnrollmentHistory = new List<EnrollmentHistory>
                    {
                        new EnrollmentHistory { CourseId = Guid.Parse("ff755817-a3fe-423f-b382-17ace8f33f60"), EnrollmentStatus = "Active", IsActive = true }
                    }
                },
                new Student
                {
                    StudentId = Guid.Parse("d0e1d9bb-e3a5-487f-8471-04b0369b70ef"),
                    StudentNumber = "FIT20242841",
                    EnrollmentHistory = new List<EnrollmentHistory>
                    {
                        new EnrollmentHistory { CourseId = Guid.Parse("ff755817-a3fe-423f-b382-17ace8f33f60"), EnrollmentStatus = "Completed", IsActive = false }
                    }
                },
                new Student
                {
                    StudentId = Guid.Parse("1a14703e-38b4-441d-a011-74abcf619123"),
                    StudentNumber = "FIT20242827",
                    EnrollmentHistory = new List<EnrollmentHistory>
                    {
                        new EnrollmentHistory { CourseId = Guid.Parse("1dd7b11d-1399-46e6-946e-17baed14046a"), EnrollmentStatus = "Active", IsActive = true }
                    }
                }
            };
        }
    }
}
