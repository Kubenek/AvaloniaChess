using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
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
        public static void showCheckmate(bool wWin, Border CheckmateOverlay, TextBlock CheckmateText)
        {
            CheckmateText.Text = wWin ? "White wins by Checkmate!" : "Black wins by Checkmate!"; 
            CheckmateOverlay.IsVisible = true;
        }
        public static void showStalemate(Border CheckmateOverlay, TextBlock CheckmateText)
        {
            CheckmateText.Text = "Game ends in Stalemate!"; 
            CheckmateOverlay.IsVisible = true;
        }
        public static void CreateLabels(Grid top, Grid left, Grid bottom, Grid right)
        {
            for(int i=0; i<8; i++)
            {
                top.   ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                bottom.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

                var tLabel = new TextBlock
                {
                    Text = ((char)('a' + i)).ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14
                };

                var bLabel = new TextBlock
                {
                    Text = ((char)('a' + i)).ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14
                };

                Grid.SetColumn(tLabel, i);
                Grid.SetColumn(bLabel, i);

                top.   Children.Add(tLabel);
                bottom.Children.Add(bLabel);

            }

            for(int i=0; i<8; i++)
            {
                left. RowDefinitions.Add(new RowDefinition(GridLength.Star));
                right.RowDefinitions.Add(new RowDefinition(GridLength.Star));

                var lLabel = new TextBlock
                {
                    Text = (8 - i).ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14
                };

                var rLabel = new TextBlock
                {
                    Text = (8 - i).ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14
                };

                Grid.SetRow(lLabel, i);
                Grid.SetRow(rLabel, i);

                left. Children.Add(lLabel);
                right.Children.Add(rLabel);

            }
        }
    }
}