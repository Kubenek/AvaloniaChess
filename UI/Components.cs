using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;

namespace Chess.UI
{
    public static class Components
    {
        public static void updateTurnText(bool wTurn, TextBlock txWhite, TextBlock txBlack)
        {
            txWhite.Opacity = wTurn ? 1 : 0.2;
            txBlack.Opacity = wTurn ? 0.2 : 1;
        }
    }
}