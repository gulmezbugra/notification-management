namespace NotificationManagement.Mvc.ViewModels;

public class DashboardViewModel
{
    public int Total { get; set; }
    public int Sent { get; set; }
    public int Failed { get; set; }
    public int Pending { get; set; }
    public List<NotificationItemViewModel> Recent { get; set; } = new();
}
