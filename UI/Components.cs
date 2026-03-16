using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Chess.GameManager;

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

        private static TextBlock makeLabel(string text, int ft)
        {
            var label = new TextBlock
            {
                Text = text,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = ft
            };

            return label;
        }

        public static void CreateLabels(Grid top, Grid left, Grid bottom, Grid right)
        {
            for(int i=0; i<8; i++)
            {
                top.   ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                bottom.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

                var ch = ((char)('a' + i)).ToString();

                var tLabel = makeLabel(ch, 14);
                var bLabel = makeLabel(ch, 14);

                Grid.SetColumn(tLabel, i);
                Grid.SetColumn(bLabel, i);

                top.   Children.Add(tLabel);
                bottom.Children.Add(bLabel);

            }

            for(int i=0; i<8; i++)
            {
                left. RowDefinitions.Add(new RowDefinition(GridLength.Star));
                right.RowDefinitions.Add(new RowDefinition(GridLength.Star));

                var ch = (8 - i).ToString();
                
                var lLabel = makeLabel(ch, 14);
                var rLabel = makeLabel(ch, 14);

                Grid.SetRow(lLabel, i);
                Grid.SetRow(rLabel, i);

                left. Children.Add(lLabel);
                right.Children.Add(rLabel);

            }
        }
    }
}