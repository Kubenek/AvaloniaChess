using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Chess.Views;

namespace Chess;

public partial class MainWindow : Window
{
    
    public MainWindow()
    {
        InitializeComponent();
        ShowMenu();
    }

    private void ShowMenu()
    {
        var menuView = new MenuView();
        menuView.StartGame += startGame;
        ContentArea.Content = menuView;
    }

    private void startGame(object? s, EventArgs e)
    {
        ContentArea.Content = new GameView();
    }

}