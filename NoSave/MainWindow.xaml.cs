using NoSave.Models;
using NoSave.Services.Interfaces;
using NoSave.ViewModels;
using NoSave.Views;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace NoSave
{
    public partial class MainWindow : Window
    {
        private readonly IFirewallService _firewallService;
        private readonly FirewallModel _firewallModel;

        public MainWindow(IFirewallService firewallService, FirewallModel firewallModel)
        {
            _firewallService = firewallService;
            _firewallModel = firewallModel;

            InitializeComponent();

            Closing += MainWindowClosing;

            DataContext = new MainWindowVM(_firewallModel);

            SetVersionInfo();
            MainFrame.Navigate(new MainPage(_firewallService, _firewallModel));
        }

        private void SetVersionInfo()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionInfoBlock.Text = $"NoSave v{version.Major}.{version.Minor}.{version.Build}";
        }

        private void DragWindow(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenM310ClubLink(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://discord.gg/wJAbKFg2Ab");
        }

        private void MainWindowClosing(object sender, CancelEventArgs e)
        {
            if (!_firewallModel.IsRuleActive)
                return;

            MessageBoxResult result = MessageBox.Show(
                "Servers are currently blocked.\n\nThe game may not work in online mode until you unblock them.\n\nClose NoSave anyway?",
                "Servers are blocked",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }
    }
}
