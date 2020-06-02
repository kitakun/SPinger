# SPinger
Deadly simple ping application for client machine.

Point of this application is monitoring for some servers.

## Made on

* .NET Framework 4.7.2
* WPF
* No any other packages

## How to use:

![App Main Frame](/git-content/SPingerFrame.png)
On main window we can see all existing servers and add or delete existing one.

Close button will hide this window, full application shutdown can be achieved only with trayIcon -> Exit button.

![Tray Icon](/git-content/SPingerTray.png)
On TrayIcon we can open main frame or exit from application.

![Notifications](/git-content/SPingerNotifications.png)
If server status changes (being online or offline) user will receive notification about the server and his status.
