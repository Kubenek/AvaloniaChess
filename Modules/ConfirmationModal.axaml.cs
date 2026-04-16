using System;
using System.Threading;
using Avalonia.Controls;

namespace Chess.Modules;

public partial class ConfirmationModal : UserControl
{
    public event EventHandler? ButtonConfirm;
    public event EventHandler? ButtonDecline;

    public ConfirmationModal()
    {
        InitializeComponent();

        btnYes.Click += OnButtonConfirm;
        btnNo.Click  += OnButtonDecilne;
    }

    public void Show()
    {
        Modal.IsVisible = true;
    }

    public void Hide()
    {
        Modal.IsVisible = false;
    }

    public void setText(string text)
    {
        ModalText.Text = text;
    }

    public void OnButtonConfirm(object? sender, EventArgs e)
    {
        ButtonConfirm?.Invoke(sender, e);
    }

    public void OnButtonDecilne(object? sender, EventArgs e)
    {
        ButtonDecline?.Invoke(sender, e);
    }

}