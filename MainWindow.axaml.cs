using System;
using Avalonia.Controls;
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
        _selectMenuView.Start1v1   += (_,_) => Mode1v1Local(s,e);
        ContentArea.Content = _selectMenuView;
    }

    private void Mode1v1Local(object? s, EventArgs e)
    {
        var _gameView = new GameView();
        _gameView.BackToMenu += (_, _) => ShowSelectionMenu(s, e);
        ContentArea.Content = _gameView;
    }

}