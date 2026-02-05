using System.Runtime.Versioning;

namespace TodoSeUsa.BlazorServer.UI.Services;

[SupportedOSPlatform("windows")]
public class TrayIconService : IHostedService
{
    private NotifyIcon? _notifyIcon;
    private readonly string _url;
    private readonly IHostApplicationLifetime _lifetime;
    private Thread? _trayThread;

    public TrayIconService(IHostApplicationLifetime lifetime, string url)
    {
        _lifetime = lifetime;
        _url = url;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _trayThread = new Thread(RunTrayIconThread);

        _trayThread.SetApartmentState(ApartmentState.STA);
        _trayThread.IsBackground = true;
        _trayThread.Start();

        return Task.CompletedTask;
    }

    private void RunTrayIconThread()
    {
        _notifyIcon = new NotifyIcon
        {
            Visible = true,
            Text = "TodoSeUsa Blazor Server",
            Icon = LoadIcon(),
            ContextMenuStrip = CreateContextMenu()
        };

        _notifyIcon.DoubleClick += (s, e) => OpenBrowser();

        System.Windows.Forms.Application.Run();
    }

    private Icon LoadIcon()
    {
        string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "assets", "icon.ico");

        if (File.Exists(iconPath))
        {
            return new Icon(iconPath);
        }

        return SystemIcons.Shield;
    }

    private ContextMenuStrip CreateContextMenu()
    {
        var contextMenu = new ContextMenuStrip();

        contextMenu.Items.Add("Abrir", null, (s, e) => OpenBrowser());
        contextMenu.Items.Add("-");
        contextMenu.Items.Add("Cerrar", null, (s, e) => ExitApp());

        return contextMenu;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        ExitApp();
        return Task.CompletedTask;
    }

    private void OpenBrowser()
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = _url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            // Log if browser fails to open
            Console.WriteLine($"Error opening browser: {ex.Message}");
        }
    }

    private void ExitApp()
    {
        if (_notifyIcon != null)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }

        _lifetime.StopApplication();
        System.Windows.Forms.Application.Exit();
    }
}