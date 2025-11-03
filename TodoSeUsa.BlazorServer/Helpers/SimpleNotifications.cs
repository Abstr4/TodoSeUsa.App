using Radzen;

namespace TodoSeUsa.BlazorServer.Helpers;

public class SimpleNotifications
{
    private readonly NotificationService _notificationService;

    public SimpleNotifications(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public void Success(string detail)
    {
        _notificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Success,
            Summary = "Operación exitosa",
            Detail = detail,
            Duration = 5000
        });
    }

    public void Error(string detail)
    {
        _notificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Error,
            Summary = "Operación fallida",
            Detail = detail,
            Duration = 5000
        });
    }

    public void Warning(string detail)
    {
        _notificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Warning,
            Summary = "Cuidado!",
            Detail = detail,
            Duration = 5000
        });
    }

    public void Info(string detail)
    {
        _notificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Info,
            Summary = "Info:",
            Detail = detail,
            Duration = 5000
        });
    }
}