using Avalonia.Controls;

using Chess.Views;

namespace Chess;

public partial class MainWindow : Window
{
    
    public MainWindow()
    {
        InitializeComponent();

        ContentArea.Content = new GameView();
    }

}