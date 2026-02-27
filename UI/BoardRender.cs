using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;

namespace Chess.UI
{
    public static class BoardRender
    {
        public static void renderBoard(Grid GameBoard)
        {
            int size = 8;

            Border [,] _border = new Border[8,8];

            for (int i=0; i<size; i++) { GameBoard.RowDefinitions.Add(new RowDefinition(GridLength.Star)); }
            for (int i=0; i<size; i++) { GameBoard.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); }

            for (int i=0; i<size; i++)
            {
                for(int j=0; j<size; j++)
                {
                    var square = new Border
                    {
                        Background= (i+j) % 2 == 0 ? Brushes.DimGray : Brushes.Gray
                    };

                    _border[i,j] = square;

                    Grid.SetRow(square, i);
                    Grid.SetColumn(square, j);

                    GameBoard.Children.Add(square);

                }
            }

        } 
    }
}