using NoSave.Models;
using NoSave.Services;
using NoSave.Services.Interfaces;
using System.Windows;

namespace NoSave
{
    public partial class App : Application
    {
        private IFirewallService _firewallService;
        private FirewallModel _firewallModel;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _firewallService = new FirewallService();
            _firewallModel = new FirewallModel(_firewallService);

            FirewallConnectivityTestService firewallConnectivityTest = new FirewallConnectivityTestService(_firewallService);

            if (firewallConnectivityTest.IsInternetWorking())
            {
                if (!firewallConnectivityTest.IsFirewallWorking())
                {
                    MessageBox.Show(
                        "NoSave could not verify that Windows Firewall rules are working.\n\n" +
                        "Please check Windows Firewall or disable third-party firewall software.",
                        "Firewall Check Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    Shutdown();
                    return;
                }
            }
            else
            {
                MessageBox.Show(
                    "NoSave could not connect to the test servers.\n\n" +
                    "Please check your internet connection and try again.",
                    "Connection Check Failed",
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
    }
}
