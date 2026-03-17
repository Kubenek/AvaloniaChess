using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using System.Runtime.Serialization;

using System;
using System.Collections.Generic;
using Avalonia.Remote.Protocol.Input;
using System.Data.SqlTypes;
using System.Net;
using System.Linq;
using Avalonia.Data;

namespace Chess.Modules;

public partial class OvCheckmate : UserControl
{
    public OvCheckmate() {
        InitializeComponent();
    }

    public void Show()
    {
        CheckmateOverlay.IsVisible = true;
    }

    public void Hide()
    {
        CheckmateOverlay.IsVisible = false;
    }

    public void setText(string text)
    {
        CheckmateText.Text = text;
    }

}