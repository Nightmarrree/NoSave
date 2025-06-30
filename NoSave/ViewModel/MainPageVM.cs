using NoSave.MVVM;
using NoSave.Services.Interfaces;
using NoSave.Services;
using System.Windows.Input;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace NoSave.ViewModel
{
    public class MainPageVM : ViewModelBase, IDisposable
    {
        private readonly IFirewallService _firewallService;
        private readonly IGlobalHotkeyService _hotkeyService;
        private string _ruleName = "NoSave";
        private string _remoteIP = "192.81.241.171";

        private string _statusText;
        private bool _isRuleActive;
        private bool _isBusy;

        public string StatusText
        {
            get { return _statusText; }
            set 
            {
                if (_statusText != value) 
                {
                    _statusText = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsRuleActive
        {
            get { return _isRuleActive; }
            set
            {
                if (_isRuleActive != value)
                {
                    _isRuleActive = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ButtonText));
                }
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ButtonText => IsRuleActive ? "Unblock R*" : "Block R*";

        public ICommand ToggleRuleCommand { get; }

        public MainPageVM()
        {
            _firewallService = new FirewallService();
            if (!_firewallService.IsFirewallEnabled())
            {
                MessageBox.Show("Firewall is disabled! Please enable it in Windows settings.",
                                "Firewall Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                Application.Current.Shutdown();
            }
            _hotkeyService = new GlobalHotkeyService();

            ToggleRuleCommand = new RelayCommand(
                execute: async (obj) => await ToggleRule(),
                canExecute: (obj) => !IsBusy
            );

            UpdateStatus();
        }

        public void UpdateStatus()
        {
            IsRuleActive = _firewallService.CheckFirewallRule(_ruleName);
        }

        public void RegisterHotkeys()
        {
            _hotkeyService.HotkeyPressed += OnHotkeyPressed;
            _hotkeyService.Register(Key.F9);
        }

        private async Task ToggleRule()
        {
            if(IsBusy)
                return;

            IsBusy = true;
            (ToggleRuleCommand as RelayCommand)?.RaiseCanExecuteChanged();
            Debug.WriteLine("Toggle");
            try
            {
                await Task.Run(() =>
                {
                    if (IsRuleActive)
                    {
                        _firewallService.RemoveRule(_ruleName);
                    }
                    else
                    {
                        _firewallService.AddRule(_ruleName, _remoteIP);
                    }
                });
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
                UpdateStatus();
                (ToggleRuleCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private void OnHotkeyPressed()
        {
            Debug.WriteLine("Hotkey pressed!");

            if (ToggleRuleCommand.CanExecute(null))
            {
                ToggleRuleCommand.Execute(null);
            }
            else
            {
                Debug.WriteLine("Command cannot be executed right now.");
            }
        }

        public void Dispose()
        {
            _hotkeyService?.Dispose();
        }
    }
}
