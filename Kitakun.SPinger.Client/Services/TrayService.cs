namespace Kitakun.SPinger.Client.Services
{
    using System;

    using Kitakun.SPinger.Client.Extensions;

    /// <summary>
    /// Tray icon features
    /// </summary>
    public class TrayService : ITrayService
    {
        private System.Windows.Forms.NotifyIcon _trayIcon;

        public event OnEvent OnShowClicked;
        public event OnEvent OnIconDoubleClick;
        public event OnEvent OnExitClicked;

        public void Dispose() => _trayIcon.Dispose();

        public void Run()
        {
            _trayIcon = new System.Windows.Forms.NotifyIcon
            {
                Text = "SPinger",
                Icon = Properties.Resources.ping,
                Visible = true,
                ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip()
            };

            _trayIcon.ContextMenuStrip.Items.Add("Show", null, (_, __) => OnShowClicked?.Invoke());

            _trayIcon.ContextMenuStrip.Items.Add("Exit", null, (_, __) => OnExitClicked?.Invoke());

            _trayIcon.DoubleClick += (_, __) => OnIconDoubleClick?.Invoke();
        }

        public void ShowNotification(string text, NotificationType type)
        {
            var icon = default(System.Windows.Forms.ToolTipIcon);
            switch (type)
            {
                case NotificationType.Warning:
                    icon = System.Windows.Forms.ToolTipIcon.Warning;
                    break;

                case NotificationType.Erorr:
                    icon = System.Windows.Forms.ToolTipIcon.Error;
                    break;

                case NotificationType.Info:
                    icon = System.Windows.Forms.ToolTipIcon.Info;
                    break;

                case NotificationType.None:
                    icon = System.Windows.Forms.ToolTipIcon.None;
                    break;

                default:
                    throw new NotImplementedException($"Type {type} not implemented");
            }

            _trayIcon.ShowBalloonTip(1000 * 5, "Information", text, icon);
        }
    }

    public interface ITrayService : IDisposable
    {
        /// <summary>
        /// On show button clicked
        /// </summary>
        event OnEvent OnShowClicked;
        /// <summary>
        /// On tray icon double clicked
        /// </summary>
        event OnEvent OnIconDoubleClick;
        /// <summary>
        /// On exit button clicked
        /// </summary>
        event OnEvent OnExitClicked;

        /// <summary>
        /// Run tray notification logic
        /// </summary>
        void Run();

        void ShowNotification(string text, NotificationType type);
    }

    public enum NotificationType : byte
    {
        Warning = 0,
        Erorr = 1,
        Info = 2,
        None = 3
    }
}
