using System;
using System.Diagnostics;
using Avalonia.Controls;
using Chess.Services;
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

        menuView.StartGame    += ShowSelectionMenu;
        menuView.OpenSettings += ShowSettings;

        ContentArea.Content = menuView;
    }

    private void ShowSelectionMenu(object? s, EventArgs e)
    {
        var _selectMenuView = new GameModeView();

        _selectMenuView.BackToMenu += (_,_) => ShowMenu();
        _selectMenuView.Start1v1   += (_,_) => Mode1v1Local(s,e);
        _selectMenuView.StartVsBot += (_,_) => ModeVsBot(s,e);

        ContentArea.Content = _selectMenuView;
    }

    private void ShowSettings(object? s, EventArgs e)
    {
        var _settingsView = new SettingsView();

        _settingsView.BackToMenu += (_,_) => ShowMenu();

        ContentArea.Content = _settingsView;
    }

    private void Mode1v1Local(object? s, EventArgs e)
    {
        var _gameView = new GameView();

        _gameView.BackToMenu += (_, _) => ShowSelectionMenu(s, e);
        
        ContentArea.Content = _gameView;
    }

    private void ModeVsBot(object? s, EventArgs e)
    {
        var _botView = new BotView();
        
        _botView.BackToMenu += (_,_) => ShowSelectionMenu(s,e);
        
        ContentArea.Content = _botView;
    }

}