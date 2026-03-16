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

namespace Chess.Modules;

public partial class Promotion : UserControl
{
    public Promotion()
    {
        InitializeComponent();
        SetupButtons();
    }

    public event Action<PieceType>? _promotionChoice;

    private void SetupButtons()
    {
        PromoteQueen.Click  += (s, e) => _promotionChoice?.Invoke(PieceType.Queen);
        PromoteBishop.Click += (s, e) => _promotionChoice?.Invoke(PieceType.Bishop);
        PromoteKnight.Click += (s, e) => _promotionChoice?.Invoke(PieceType.Knight);
        PromoteRook.Click   += (s, e) => _promotionChoice?.Invoke(PieceType.Rook);
    }

    public void Show() 
    {
        PromotionOverlay.IsVisible = true;
    }
    public void Hide()
    {
        PromotionOverlay.IsVisible = false;
    }

}