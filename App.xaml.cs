using System.Windows;

namespace NoSave
{
    public partial class App : Application
    {
        public string ruleName = "NoSave";
        public string remoteIP = "192.81.241.171";
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (!Utilities.IsFirewallEnabled())
            {
                MessageBox.Show("Firewall is disabled! Please enable it in Windows settings.",
                                "Firewall Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Environment.Exit(0);
            }
            MainWindow mainWindow = new MainWindow
            {
                Left = 10,
                Top = 10
            };
            mainWindow.Show();
        }
    }
}
