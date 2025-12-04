using NoSave.View;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace NoSave
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetVersionInfo();
            MainFrame.Navigate(new MainPage());
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
            Application.Current.Shutdown();
        }

        private void OpenM310ClubLink(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://discord.gg/wJAbKFg2Ab");
        }
    }
}
