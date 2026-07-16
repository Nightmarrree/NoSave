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

            MainWindow mainWindow = new MainWindow(_firewallService, _firewallModel)
            {
                Left = 10,
                Top = 10
            };
            mainWindow.Show();
        }
    }
}
