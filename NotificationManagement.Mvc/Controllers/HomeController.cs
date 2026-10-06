using Microsoft.AspNetCore.Mvc;
using NotificationManagement.Core.Interfaces;
using NotificationManagement.Mvc.ViewModels;

namespace NotificationManagement.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly INotificationService _notificationService;

    public HomeController(INotificationService notificationService)
        => _notificationService = notificationService;

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var data = await _notificationService.GetDashboardAsync(8, cancellationToken);

        var model = new DashboardViewModel
        {
            Total = data.Stats.Total,
            Sent = data.Stats.Sent,
            Failed = data.Stats.Failed,
            Pending = data.Stats.Pending,
            Recent = data.Recent.Select(n => n.ToViewModel()).ToList()
        };

        return View(model);
    }

    [Route("Home/Error")]
    public IActionResult Error(int? statusCode)
    {
        var code = statusCode ?? 500;

        var model = code switch
        {
            404 => new ErrorViewModel
            {
                StatusCode = 404,
                Title = "Not found",
                Message = "The page or record you are looking for could not be found."
            },
            400 => new ErrorViewModel
            {
                StatusCode = 400,
                Title = "Invalid request",
                Message = "The request could not be processed. Please check your input and try again."
            },
            _ => new ErrorViewModel
            {
                StatusCode = 500,
                Title = "Something went wrong",
                Message = "An unexpected error occurred. Please try again later."
            }
        };

        Response.StatusCode = model.StatusCode;
        return View(model);
    }
}
