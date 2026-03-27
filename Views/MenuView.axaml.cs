using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Chess.Views;

public partial class MenuView : UserControl
{

    public event EventHandler? StartGame;

    public MenuView()
    {
        InitializeComponent();

        btnStart.Click += onStart;
        btnQuit. Click += onQuit;
    }


    private void onStart(object? s, RoutedEventArgs e)
    {
        StartGame?.Invoke(this, EventArgs.Empty);
    }

    private void onQuit(object? s, RoutedEventArgs e)
    {
        Environment.Exit(0);
    }

}