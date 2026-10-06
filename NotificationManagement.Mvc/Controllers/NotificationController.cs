using Microsoft.AspNetCore.Mvc;
using NotificationManagement.Core.Enums;
using NotificationManagement.Core.Exceptions;
using NotificationManagement.Core.Interfaces;
using NotificationManagement.Core.Models;
using NotificationManagement.Mvc.ViewModels;

namespace NotificationManagement.Mvc.Controllers;

public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
        => _notificationService = notificationService;

    // GET /Notification

    public async Task<IActionResult> Index(
        string? search,
        NotificationType? type,
        NotificationStatus? status,
        string? sort,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        const int pageSize = 10;

        var filter = new NotificationFilter
        {
            Search = search,
            Type = type,
            Status = status,
            NewestFirst = sort != "date_asc",
            PageNumber = Math.Max(1, page),
            PageSize = pageSize
        };

        var result = await _notificationService
            .GetNotificationsAsync(filter, cancellationToken);

        var model = new NotificationListViewModel
        {
            Items = result.Items
                .Select(n => n.ToViewModel())
                .ToList(),

            Search = search,
            Type = type,
            Status = status,
            Sort = sort == "date_asc"
                ? "date_asc"
                : "date_desc",

            CurrentPage = filter.PageNumber,
            PageSize = pageSize,
            TotalCount = result.TotalCount
        };

        return View(model);
    }

    // GET /Notification/Details/5
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var notification = await _notificationService.GetByIdAsync(id, cancellationToken);
        return View(notification.ToViewModel());
    }

    // GET /Notification/Create
    [HttpGet]
    public IActionResult Create() => View(new CreateNotificationViewModel());

    // POST /Notification/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateNotificationViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var dto = new CreateNotificationDto
            {
                Receiver = model.Receiver,
                Message = model.Message,
                Type = model.Type!.Value
            };

            // Not tied to the request's cancellation token: once we start sending,
            // we want the final status saved even if the browser disconnects.
            var result = await _notificationService.SendNotificationAsync(dto);

            if (result.Status == NotificationStatus.Sent)
                TempData["Success"] = $"{result.NotificationType.Label()} notification sent successfully to {result.Receiver}.";
            else
                TempData["Error"] = "The notification could not be delivered. See the details below.";

            return RedirectToAction(nameof(Details), new { id = result.Id });
        }
        catch (NotificationValidationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
        catch (UnsupportedNotificationTypeException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
}
