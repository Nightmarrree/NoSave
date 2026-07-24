using NoSave.Models;
using NoSave.MVVM;
using NoSave.Services;
using NoSave.Services.Interfaces;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NoSave.ViewModels
{
    public class MainPageVM : ViewModelBase, IDisposable
    {
        private readonly IFirewallService _firewallService;
        private readonly FirewallModel _firewallModel;

        private readonly IGlobalHotkeyService _hotkeyService;
        private bool _isHotkeyRegistered;

        private string _ruleName = "NoSave";
        private string _remoteIP = "192.81.241.171";

        private bool _isBusy;

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

        public string ButtonText => _firewallModel.IsRuleActive ? "Unblock R*" : "Block R*";

        public ICommand ToggleRuleCommand { get; }

        public MainPageVM(IFirewallService firewallService, FirewallModel firewallModel)
        {
            _firewallService = firewallService;
            _firewallModel = firewallModel;

            _firewallModel.PropertyChanged += OnFirewallModelPropertyChanged;

            _hotkeyService = new GlobalHotkeyService();
            ToggleRuleCommand = new RelayCommand(
                execute: async (obj) => await ToggleRule(),
                canExecute: (obj) => !IsBusy
            );

            _firewallModel.UpdateStatus();
        }

        public void RegisterHotkeys()
        {
            if (_isHotkeyRegistered)
                return;

            var keyToRegister = Key.F9;

            string configFileName = "hotkey.txt";
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName);

            if (File.Exists(configFilePath))
            {
                string keyNameFromFile = File.ReadAllText(configFilePath).Trim();

                if (Enum.TryParse<Key>(keyNameFromFile, true, out Key customKey))
                {
                    keyToRegister = customKey;
                }
            }
            
            bool registered = _hotkeyService.Register(keyToRegister);

            if (!registered)
            {
                MessageBox.Show(
                    "Failed to register hotkey. You can still use the button manually.",
                    "Hotkey error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            _hotkeyService.HotkeyPressed += OnHotkeyPressed;
            _isHotkeyRegistered = true;
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
                    if (_firewallModel.IsRuleActive)
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
                _firewallModel.UpdateStatus();
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

        private void OnFirewallModelPropertyChanged(
            object sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FirewallModel.IsRuleActive))
            {
                OnPropertyChanged(nameof(ButtonText));
            }
        }

        public void Dispose()
        {
            if (!_isHotkeyRegistered)
                return;

            _hotkeyService.HotkeyPressed -= OnHotkeyPressed;
            _hotkeyService.Dispose();
            _isHotkeyRegistered = false;
        }
    }
}