namespace ForekOnline.Application.Common.Utility
{
    public static class BadgeHelper
    {
            public static string GetBadgeClass(string status)
            {
                switch (status)
                {
                    case "Started":
                        return "badge rounded-pill bg-info";

                    case "Completed":
                        return "badge rounded-pill bg-success";

                case "DroppedOut":
                    return "badge rounded-pill bg-danger";

                case "Transferred":
                    return "badge rounded-pill bg-primary";

                default:
                        return "badge rounded-pill bg-dark";
                }
            }
    }
}
