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
        menuView.StartGame += ShowSelectionMenu;
        ContentArea.Content = menuView;
    }

    private void ShowSelectionMenu(object? s, EventArgs e)
    {
        var _selectMenuView = new GameModeView();
        _selectMenuView.BackToMenu += (_,_) => ShowMenu();
        ContentArea.Content = _selectMenuView;
    }

    private void startGame(object? s, EventArgs e)
    {
        var _gameView = new GameView();
        _gameView.BackToMenu += (_, _) => ShowMenu();
        ContentArea.Content = _gameView;
    }

}