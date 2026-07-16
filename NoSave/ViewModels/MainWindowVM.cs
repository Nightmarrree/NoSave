using NoSave.Models;
using NoSave.MVVM;
using System.ComponentModel;

namespace NoSave.ViewModels
{
    public class MainWindowVM : ViewModelBase
    {
        private readonly FirewallModel _firewallModel;

        public bool IsRuleActive => _firewallModel.IsRuleActive;

        public MainWindowVM(FirewallModel firewallModel)
        {
            _firewallModel = firewallModel;
            _firewallModel.PropertyChanged += OnFirewallModelPropertyChanged;
        }

        private void OnFirewallModelPropertyChanged(
            object sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FirewallModel.IsRuleActive))
            {
                OnPropertyChanged(nameof(IsRuleActive));
            }
        }
    }
}
