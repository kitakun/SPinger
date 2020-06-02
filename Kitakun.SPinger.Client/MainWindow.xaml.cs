namespace Kitakun.SPinger.Client
{
    using System.Collections.ObjectModel;
    using System.Windows;

    using Kitakun.SPinger.Client.Extensions;
    using Kitakun.SPinger.Client.Services;
    using Kitakun.SPinger.Core.Models;
    using Kitakun.SPinger.Core.Services;
    using Kitakun.SPinger.Services;
    using Kitakun.SPinger.Validators;

    /// <summary>
    /// Main window with creation and list of existing items
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ITrayService TrayService { get; } = new TrayService();
        public static IPingService PingService { get; } = new PingService(60); // check every hour
        public static ISettingsService SettingsService { get; } = new SettingsService();

        /// <summary>
        /// Create new object for Observation
        /// </summary>
        public string PingToNewAddress { get; set; } = string.Empty;
        /// <summary>
        /// All items for observation
        /// </summary>
        public ObservableCollection<PingerTargetModel> Elements { get; set; } = new ObservableCollection<PingerTargetModel>();

        private bool _protectedFromClosing = true;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // events
            TrayService.OnShowClicked += ShowMainFrame;
            TrayService.OnIconDoubleClick += ShowMainFrame;
            TrayService.OnExitClicked += CloseApplication;
            // run
            TrayService.Run();

#if !(DEBUG_RUN)
            // hide window on startup
            OnMainWindow_Closing(null, null);
#endif
            var existingRecords = SettingsService.Load();
            if (existingRecords != null && existingRecords.Length > 0)
            {
                for (var i = 0; i < existingRecords.Length; i++)
                {
                    Elements.Add(new PingerTargetModel
                    {
                        Address = existingRecords[i],
                        IsAvailable = true
                    });
                }
            }

            // start watch for servers
            PingService.Watch(Elements);
            PingService.OnServerChanged += PingService_OnServerChanged;
        }

        private void ShowMainFrame()
        {
            Show();
            WindowState = WindowState.Normal;
        }

        private void CloseApplication()
        {
            _protectedFromClosing = false;
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Add new server for watching
        /// </summary>
        private void Button_ClickAddNewRecord(object _, RoutedEventArgs __)
        {
            if (string.IsNullOrEmpty(PingToNewAddress))
                return;

            void addAddress(bool isAvailable)
            {
                inptPingToAddrs.OnUI((textBox) =>
                {
                    Elements.Add(new PingerTargetModel
                    {
                        Address = PingToNewAddress,
                        IsAvailable = isAvailable
                    });

                    PingToNewAddress = string.Empty;
                    textBox.Text = PingToNewAddress;

                    UpdateSettings();
                });
            };

            HttpAddressValidator.ValidateAsync(PingToNewAddress).ContinueWith(task =>
            {
                if (task.Exception == null)
                {
                    ValidationResponse validationResult = task.Result;
                    switch (validationResult)
                    {
                        case ValidationResponse.InvalidAddress:
                            break;

                        case ValidationResponse.ServerUnavailable:
                            addAddress(false);
                            break;

                        case ValidationResponse.Validated:
                            addAddress(true);
                            break;

                        default:
                            throw new System.NotImplementedException($"Type {validationResult} not implemented!");
                    }
                }
                else
                {
                    System.Console.WriteLine($"Error happened on validation={task.Exception.Message}");
                }
            });
        }

        // On enter -> add new server
        private void OnKeyDownForNewAddress(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Button_ClickAddNewRecord(sender, e);
            }
        }

        // prevent main frame from closing app
        private void OnMainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_protectedFromClosing)
            {
                if (e != null)
                {
                    e.Cancel = true;
                }
                Hide();
                WindowState = WindowState.Minimized;
            }
            else
            {
                TrayService.Dispose();
            }
        }

        // Delete existing record
        private void Button_DeleteExistingRecord(object sender, RoutedEventArgs e)
        {
            var elementForDelete = ((FrameworkElement)sender).DataContext as PingerTargetModel;
            Elements.Remove(elementForDelete);
            UpdateSettings();
        }

        // Server status is changed
        private void PingService_OnServerChanged(PingerTargetModel changedModel)
        {
            if (changedModel.IsAvailable)
            {
                TrayService.ShowNotification($"Server [{changedModel.Address}] is available!", NotificationType.Info);
            }
            else
            {
                TrayService.ShowNotification($"Server [{changedModel.Address}] is down!", NotificationType.Erorr);
            }
        }

        private void UpdateSettings()
        {
            var records = new string[Elements.Count];
            for (var i = 0; i < Elements.Count; i++)
            {
                records[i] = Elements[i].Address;
            }
            SettingsService.Save(records);
        }
    }
}
