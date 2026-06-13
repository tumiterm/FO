using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;

namespace ElecPOE.Services;

public interface ILearningPracticeService
{
    Task<LearningPracticeAssessment> GetAssessmentAsync(
        string subject,
        string difficulty,
        int questionCount,
        CancellationToken cancellationToken);
}

public sealed record LearningPracticeQuestion(
    string Prompt,
    IReadOnlyList<string> Options,
    int CorrectOptionIndex);

public sealed record LearningPracticeAssessment(
    string Subject,
    string Difficulty,
    string Provider,
    IReadOnlyList<LearningPracticeQuestion> Questions);

public sealed class LearningPracticeService : ILearningPracticeService
{
    private const int MathematicsCategory = 19;
    private const int ScienceCategory = 17;
    private static readonly SemaphoreSlim ProviderLock = new(1, 1);
    private static DateTimeOffset _lastProviderRequest = DateTimeOffset.MinValue;

    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<LearningPracticeService> _logger;

    public LearningPracticeService(
        HttpClient httpClient,
        IMemoryCache cache,
        ILogger<LearningPracticeService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<LearningPracticeAssessment> GetAssessmentAsync(
        string subject,
        string difficulty,
        int questionCount,
        CancellationToken cancellationToken)
    {
        var normalizedSubject = string.Equals(subject, "science", StringComparison.OrdinalIgnoreCase)
            ? "science"
            : "maths";
        var requestedDifficulty = difficulty?.ToLowerInvariant();
        var normalizedDifficulty = requestedDifficulty is "easy" or "medium" or "hard"
            ? requestedDifficulty
            : "easy";
        var normalizedCount = Math.Clamp(questionCount, 5, 15);
        var cacheKey = $"learning-practice:{normalizedSubject}:{normalizedDifficulty}:{normalizedCount}";

        if (_cache.TryGetValue(cacheKey, out LearningPracticeAssessment? cached) && cached is not null)
        {
            return cached;
        }

        try
        {
            await ProviderLock.WaitAsync(cancellationToken);
            try
            {
                var elapsed = DateTimeOffset.UtcNow - _lastProviderRequest;
                var providerDelay = TimeSpan.FromSeconds(5) - elapsed;
                if (providerDelay > TimeSpan.Zero)
                {
                    await Task.Delay(providerDelay, cancellationToken);
                }

                var category = normalizedSubject == "science" ? ScienceCategory : MathematicsCategory;
                var path = $"api.php?amount={normalizedCount}&category={category}&difficulty={normalizedDifficulty}&type=multiple";
                var response = await _httpClient.GetFromJsonAsync<OpenTriviaResponse>(path, cancellationToken);
                _lastProviderRequest = DateTimeOffset.UtcNow;

                if (response?.ResponseCode == 0 && response.Results.Count >= normalizedCount)
                {
                    var assessment = new LearningPracticeAssessment(
                        normalizedSubject,
                        normalizedDifficulty,
                        "Open Trivia DB",
                        response.Results.Select(MapQuestion).ToList());

                    _cache.Set(cacheKey, assessment, TimeSpan.FromMinutes(10));
                    return assessment;
                }
            }
            finally
            {
                ProviderLock.Release();
            }
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            _logger.LogWarning(
                exception,
                "Open Trivia DB was unavailable for {Subject} at {Difficulty} difficulty.",
                normalizedSubject,
                normalizedDifficulty);
        }

        return CreateFallbackAssessment(normalizedSubject, normalizedDifficulty, normalizedCount);
    }

    private static LearningPracticeQuestion MapQuestion(OpenTriviaQuestion source)
    {
        var correctAnswer = WebUtility.HtmlDecode(source.CorrectAnswer);
        var options = source.IncorrectAnswers
            .Select(WebUtility.HtmlDecode)
            .Append(correctAnswer)
            .OrderBy(_ => Random.Shared.Next())
            .ToList();

        return new LearningPracticeQuestion(
            WebUtility.HtmlDecode(source.Question),
            options,
            options.IndexOf(correctAnswer));
    }

    private static LearningPracticeAssessment CreateFallbackAssessment(
        string subject,
        string difficulty,
        int questionCount)
    {
        var source = subject == "science" ? ScienceFallbackQuestions : MathsFallbackQuestions;
        var questions = Enumerable.Range(0, questionCount)
            .Select(index => source[index % source.Count])
            .OrderBy(_ => Random.Shared.Next())
            .ToList();

        return new LearningPracticeAssessment(subject, difficulty, "Forek offline question bank", questions);
    }

    private static readonly IReadOnlyList<LearningPracticeQuestion> MathsFallbackQuestions =
    [
        new("What is 12 × 8?", ["96", "88", "108", "86"], 0),
        new("Solve for x: 3x + 5 = 20.", ["5", "3", "10", "15"], 0),
        new("What is 25% of 200?", ["50", "25", "75", "100"], 0),
        new("A triangle has angles of 45° and 65°. What is the third angle?", ["70°", "80°", "90°", "60°"], 0),
        new("Which fraction is equivalent to 0.75?", ["3/4", "2/3", "1/4", "4/5"], 0),
        new("What is the square root of 144?", ["12", "14", "10", "16"], 0),
        new("Simplify: 2(4 + 3).", ["14", "11", "18", "9"], 0),
        new("What is the perimeter of a rectangle measuring 6 cm by 4 cm?", ["20 cm", "24 cm", "10 cm", "18 cm"], 0)
    ];

    private static readonly IReadOnlyList<LearningPracticeQuestion> ScienceFallbackQuestions =
    [
        new("Which gas do plants absorb during photosynthesis?", ["Carbon dioxide", "Oxygen", "Nitrogen", "Hydrogen"], 0),
        new("What is the basic unit of life?", ["Cell", "Atom", "Organ", "Tissue"], 0),
        new("Which force pulls objects toward Earth?", ["Gravity", "Friction", "Magnetism", "Buoyancy"], 0),
        new("At sea level, water boils at what temperature?", ["100°C", "0°C", "50°C", "212°C below zero"], 0),
        new("Which planet is known as the Red Planet?", ["Mars", "Venus", "Jupiter", "Mercury"], 0),
        new("What type of energy is stored in food?", ["Chemical energy", "Sound energy", "Light energy", "Nuclear energy"], 0),
        new("Which organ pumps blood around the human body?", ["Heart", "Lungs", "Liver", "Kidneys"], 0),
        new("A substance with a pH below 7 is described as what?", ["Acidic", "Neutral", "Alkaline", "Metallic"], 0)
    ];

    private sealed class OpenTriviaResponse
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; init; }

        [JsonPropertyName("results")]
        public List<OpenTriviaQuestion> Results { get; init; } = [];
    }

    private sealed class OpenTriviaQuestion
    {
        [JsonPropertyName("question")]
        public string Question { get; init; } = string.Empty;

        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get; init; } = string.Empty;

        [JsonPropertyName("incorrect_answers")]
        public List<string> IncorrectAnswers { get; init; } = [];
    }
}
