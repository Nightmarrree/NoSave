using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NoSave
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _ = Task.Run(StatusChecker);
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Utilities.CheckFirewallRule())
            {
                Utilities.SetFirewall(false);
            }
            else
            {
                Utilities.SetFirewall(true);
            }
        }

        private async Task StatusChecker()
        {
            App app = Application.Current as App;

            while (true)
            {
                bool ruleExists = Utilities.CheckFirewallRule();

                await Dispatcher.InvokeAsync(() =>
                {
                    if (ruleExists)
                    {
                        Button.Content = "Unblock R*";
                    }
                    else
                    {
                        Button.Content = "Block R*";
                    }
                });

                await Task.Delay(200);
            }
        }


        private void DragWindow(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}