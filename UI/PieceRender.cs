using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Chess.Pieces;

namespace Chess.UI
{
    public static class PieceRender
    {
        public static void renderPieces(Grid GameBoard, Piece?[,] pieces)
        {
            
            foreach(var piece in pieces)
            {
                if(piece is null) continue;
                TextBlock pieceVis = createPieceVisual(piece);

                int row = piece.Row; 
                int col = piece.Column;

                Grid.SetRow    (pieceVis, row);
                Grid.SetColumn (pieceVis, col);

                GameBoard.Children.Add(pieceVis);

                // Create a visual for the piece
                // Add the piece to the Grid
            }

        }

        private static TextBlock createPieceVisual(Piece piece)
        {
            var pieceVisual = new TextBlock
            {
                Text = piece.Texture,
                FontSize = 50,
                FontFamily = "Segoe UI Symbol",
                Foreground = piece.IsWhite ? Brushes.White : Brushes.Black,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            return pieceVisual;
        }
    
    }
}