using System;
using System.Runtime.CompilerServices;
using Avalonia.Controls;

namespace Chess.Views;

public partial class GameModeView : UserControl
{
    public event EventHandler? BackToMenu;
    public event EventHandler? Start1v1;
    
    public GameModeView()
    {
        InitializeComponent();
        ModeListBox.SelectionChanged += ChangeSelection;
        ButtonBack.Click += ReturnToMenu;
        ButtonPlay.Click += LaunchMode;
    }

    private void ChangeSelection(object? s, EventArgs e)
    {
        if (ModeListBox.SelectedIndex == -1)
        {
            ButtonPlay.IsVisible = false;
        }

        ButtonPlay.IsVisible = true;

        switch (ModeListBox.SelectedIndex)
        {
            case 0: // 1v1 Local
                PreviewTitle.Text = "1v1 Local";
                PreviewDescription.Text = "Classic chess against a friend on the same device. Take turns making moves on the same board.";
                break;

            case 1: // vs Computer
                PreviewTitle.Text = "vs Computer";
                PreviewDescription.Text = "Play against AI opponents of varying difficulty. Choose your challenge level and test your skills.";
                break;

            case 2: // Puzzles 
                PreviewTitle.Text = "Puzzles";
                PreviewDescription.Text = "Train your problem solving with puzzles. Solve positions ranging from mate-in-1 to complex endgames.";
                break;
        }
    }

    private void LaunchMode(object? s, EventArgs e)
    {
        switch (ModeListBox.SelectedIndex)
        {
            case 0: // 1v1 Local
                Start1v1?.Invoke(s, e);
                break;

            case 1: // vs Computer
                break;

            case 2: // Puzzles 
                break;
        }
    }

    private void ReturnToMenu(object? s, EventArgs e)
    {
        BackToMenu?.Invoke(s, e);
    }

}