using DreamLab.MVVM.ViewModel;
using System.Text;
using System.Windows;


namespace DreamLab
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

    }
}