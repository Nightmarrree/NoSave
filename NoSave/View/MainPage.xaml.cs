using System.Windows;
using System.Windows.Controls;
using NoSave.ViewModel;

namespace NoSave.View
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