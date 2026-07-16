using NoSave.MVVM;
using NoSave.Services.Interfaces;

namespace NoSave.Models
{
    public class FirewallModel : ViewModelBase
    {
        private readonly IFirewallService _firewallService;
        private bool _isRuleActive;

        public bool IsRuleActive
        {
            get => _isRuleActive;
            private set
            {
                _isRuleActive = value;
                OnPropertyChanged();
            }
        }

        public FirewallModel(IFirewallService firewallService)
        {
            _firewallService = firewallService;
        }

        public void UpdateStatus()
        {
            IsRuleActive = _firewallService.CheckFirewallRule("NoSave");
        }
    }
}
