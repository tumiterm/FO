
namespace ForekOnline.Domain.ViewModels
{
    public class DashboardSummaryViewModel
    {
        public int Total { get; set; }   // total submissions today
        public int Recent { get; set; }  // submissions within last 2 days
        public string Top { get; set; } = "N/A"; // top programme name
    }
}
