using System.Net;
using System.Text;
using ElecPOE.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace Forek.Test;

public class LearningPracticeServiceTests
{
    [Fact]
    public async Task GetAssessmentAsync_MapsAndDecodesProviderQuestions()
    {
        const string json = """
            {
              "response_code": 0,
              "results": [
                {"question":"What is 2 &times; 3?","correct_answer":"6","incorrect_answers":["4","5","8"]},
                {"question":"Question 2","correct_answer":"A","incorrect_answers":["B","C","D"]},
                {"question":"Question 3","correct_answer":"A","incorrect_answers":["B","C","D"]},
                {"question":"Question 4","correct_answer":"A","incorrect_answers":["B","C","D"]},
                {"question":"Question 5","correct_answer":"A","incorrect_answers":["B","C","D"]}
              ]
            }
            """;
        var service = CreateService(new StubHandler(HttpStatusCode.OK, json));

        var result = await service.GetAssessmentAsync("maths", "easy", 5, CancellationToken.None);

        Assert.Equal("Open Trivia DB", result.Provider);
        Assert.Equal(5, result.Questions.Count);
        Assert.Equal("What is 2 × 3?", result.Questions[0].Prompt);
        Assert.Equal("6", result.Questions[0].Options[result.Questions[0].CorrectOptionIndex]);
    }

    [Fact]
    public async Task GetAssessmentAsync_UsesOfflineQuestionsWhenProviderFails()
    {
        var service = CreateService(new StubHandler(HttpStatusCode.ServiceUnavailable, "{}"));

        var result = await service.GetAssessmentAsync("science", "medium", 5, CancellationToken.None);

        Assert.Equal("Forek offline question bank", result.Provider);
        Assert.Equal("science", result.Subject);
        Assert.Equal(5, result.Questions.Count);
    }

    private static LearningPracticeService CreateService(HttpMessageHandler handler)
    {
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://opentdb.com/") };
        return new LearningPracticeService(
            client,
            new MemoryCache(new MemoryCacheOptions()),
            NullLogger<LearningPracticeService>.Instance);
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _content;

        public StubHandler(HttpStatusCode statusCode, string content)
        {
            _statusCode = statusCode;
            _content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_content, Encoding.UTF8, "application/json")
            });
        }
    }
}
