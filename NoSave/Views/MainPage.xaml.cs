using NoSave.Models;
using NoSave.Services.Interfaces;
using NoSave.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace NoSave.Views
{
    public partial class MainPage : Page
    {
        private readonly MainPageVM _vm;

        public MainPage(IFirewallService firewallService, FirewallModel firewallModel)
        {
            _vm = new MainPageVM(firewallService, firewallModel);
            DataContext = _vm;
            InitializeComponent();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            _vm.RegisterHotkeys();
        }
    }
}