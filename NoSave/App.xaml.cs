using NoSave.Models;
using NoSave.Services;
using NoSave.Services.Interfaces;
using NoSave.Services.Localization;
using System.Diagnostics;
using System.Windows;

namespace NoSave
{
    public partial class App : Application
    {
        public static LocalizationService Localization { get; private set; }

        private IFirewallService _firewallService;
        private FirewallModel _firewallModel;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            CloseOtherInstances();

            Localization = new LocalizationService(e.Args);

            _firewallService = new FirewallService();
            _firewallModel = new FirewallModel(_firewallService);

            FirewallConnectivityTestService firewallConnectivityTest = new FirewallConnectivityTestService(_firewallService);

            if (firewallConnectivityTest.IsInternetWorking())
            {
                if (!firewallConnectivityTest.IsFirewallWorking())
                {
                    MessageBox.Show(
                        Localization.GetString("FirewallCheckFailedMessage"),
                        Localization.GetString("FirewallCheckFailedTitle"),
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    Shutdown();
                    return;
                }
            }
            else
            {
                MessageBox.Show(
                    Localization.GetString("ConnectionCheckFailedMessage"),
                    Localization.GetString("ConnectionCheckFailedTitle"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Shutdown();
                return;
            }

            MainWindow mainWindow = new MainWindow(_firewallService, _firewallModel)
            {
                Left = 10,
                Top = 10
            };
            mainWindow.Show();
        }

        private void CloseOtherInstances()
        {
            Process currentProcess = Process.GetCurrentProcess();
            string processName = currentProcess.ProcessName;

            foreach (Process process in Process.GetProcessesByName(processName))
            {
                if (process.Id == currentProcess.Id)
                    continue;

                try
                {
                    process.Kill();
                }
                catch { }
            }
        }
    }
}
