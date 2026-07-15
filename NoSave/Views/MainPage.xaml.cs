using System.Windows;
using System.Windows.Controls;
using NoSave.ViewModels;

namespace NoSave.Views
{
    public partial class MainPage : Page
    {
        private readonly MainPageVM _vm;
        public MainPage()
        {
            _vm = new MainPageVM();
            DataContext = _vm;
            InitializeComponent();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            _vm.RegisterHotkeys();
        }
    }
}