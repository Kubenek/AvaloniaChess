
using System;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;

namespace Chess.UI
{
    public static class Components
    {
        public static event EventHandler? ExitPressed;

        public static void updateTurnText(bool wTurn, TextBlock txWhite, TextBlock txBlack)
        {
            txWhite.Opacity = wTurn ? 1 : 0.2;
            txBlack.Opacity = wTurn ? 0.2 : 1;
            txWhite.Text = "White's Turn";
            txBlack.Text = "Black's Turn";

            txBlack.Foreground = new SolidColorBrush(Colors.White);
        }
        public static void updateReviewModeText(TextBlock txWhite, TextBlock txBlack)
        {
            txWhite.Opacity = 1;
            txBlack.Opacity = 1;

            txBlack.Text = "Review Mode";
            txBlack.Foreground = new SolidColorBrush(Color.Parse("#4682B4"));

            txWhite.Text = "Click here to exit";
            txWhite.PointerPressed += (s, e) =>
            {
                ExitPressed?.Invoke(s, e);
            };
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
    
        public static void drawArrow(Move move, Color color, Grid board)
        {
            int cellSize = 50;
            double opc = 0.7;
    
            double x1 = move.From.Col * cellSize + cellSize / 2.0;
            double y1 = move.From.Row * cellSize + cellSize / 2.0;
            double x2 = move.To.Col * cellSize + cellSize / 2.0;
            double y2 = move.To.Row * cellSize + cellSize / 2.0;

            double angle = Math.Atan2(y2 - y1, x2 - x1);
            double arrowSize = 15;
            double shortenLine = 13;

            double x2Short = x2 - shortenLine*Math.Cos(angle);
            double y2Short = y2 - shortenLine*Math.Sin(angle);
            
            var line = new Line
            {
                StartPoint = new Point(x1, y1),
                EndPoint = new Point(x2Short, y2Short),
                Stroke = new SolidColorBrush(color),
                StrokeThickness = 6,
                ZIndex = 100,
                Opacity = opc
            };
            
            var arrowHead = new Polygon
            {
                Fill = new SolidColorBrush(color),
                Points = new Points
                {
                    new Point(x2, y2),
                    new Point(
                        x2 - arrowSize * Math.Cos(angle - Math.PI / 6),
                        y2 - arrowSize * Math.Sin(angle - Math.PI / 6)
                    ),
                    new Point(
                        x2 - arrowSize * Math.Cos(angle + Math.PI / 6),
                        y2 - arrowSize * Math.Sin(angle + Math.PI / 6)
                    )
                },
                ZIndex = 100,
                Opacity = opc
            };
            
            board.Children.Add(line);
            board.Children.Add(arrowHead);
        }
    
        public static void clearArrows(Grid board)
        {
            var arrows = board.Children
                .OfType<Line>()
                .Cast<Control>()
                .Concat(board.Children.OfType<Polygon>().Cast<Control>())
                .ToList();
            
            foreach (var arrow in arrows)
            {
                board.Children.Remove(arrow);
            }
        }
    }
}