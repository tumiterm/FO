namespace ForekOnline.Application.Common.Interfaces
{
    public interface IBackgroundJobHandler
    {
        string JobType { get; }

        Task HandleAsync(string payloadJson, CancellationToken ct);
    }
}