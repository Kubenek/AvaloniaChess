using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Metadata;
using Chess.GameManager;
using Chess.Pieces;

namespace Chess.UI
{
    public static class PieceRender
    {

        public static void renderPieces(Grid GameBoard, ChessManager manager, Border [,] _board, MoveHighlighter _controller)
        {

            var pieces = manager.pieces;
            
            foreach(var piece in pieces)
            {
                if(piece is null) continue;
                TextBlock pieceVis = createPieceVisual(piece, manager, GameBoard, _controller);

                int row = piece.Row; 
                int col = piece.Column;

                Grid.SetRow    (pieceVis, row);
                Grid.SetColumn (pieceVis, col);

                GameBoard.Children.Add(pieceVis);

                // Create a visual for the piece
                // Add the piece to the Grid
            }

        }

        private static TextBlock createPieceVisual(Piece piece, ChessManager manager, Grid GameBoard, MoveHighlighter _controller)
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

            pieceVisual.PointerPressed += (sender, e) =>
            {
                _controller.clearHighlights(GameBoard);
                _controller.highlightPieceMoves(piece, manager, GameBoard);
            };

            return pieceVisual;
        }
    
    }
}