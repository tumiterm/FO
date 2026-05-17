
namespace ForekOnline.Domain.ViewModels
{
    public class SubmittedAnswerItem
    {
        public Guid QuestionId { get; set; }
        public Guid? SelectedOptionId { get; set; }
        public string? ShortAnswerValue { get; set; }
        public string? DiagramAnnotationJson { get; set; }
        public string? DiagramAnnotationSnapshotFileId { get; set; }
    }
}
