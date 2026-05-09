using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace Chess.Views;

public partial class SettingsView : UserControl
{
    public event EventHandler? BackToMenu;
    
    public SettingsView()
    {
        InitializeComponent();

        btnBack.Click += ReturnToMenu;
    }

    private void ReturnToMenu(object? s, EventArgs e)
    {
        BackToMenu?.Invoke(s, e);
    }
}