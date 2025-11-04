using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DreamLab.MVVM.View.Games
{
    public partial class ColorsGameView : UserControl
    {

        public ColorsGameView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this);
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is ViewModel.Games.ColorsGameViewModel vm)
            {
                if (e.Key == Key.Space)
                {
                    vm.StartGame();
                    return;
                }

                vm.HandleKeyPress(e.Key);
            }
        }
    }
}
