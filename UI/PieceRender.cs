using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Metadata;
using Chess.Logic;
using Chess.Pieces;

namespace Chess.UI
{
    public class PieceRender
    {
        public event Action<Piece, TextBlock>? PiecePressed;

        private Dictionary<Piece, TextBlock> _visuals = [];

        public void renderPieces(Grid GameBoard, ChessManager manager)
        {
            var pieces = manager._state.Board;
            
            foreach(var piece in pieces)
            {
                if(piece is null) continue;
                TextBlock pieceVis = createPieceVisual(piece);

                int row = piece.Row; 
                int col = piece.Column;

                Grid.SetRow    (pieceVis, row);
                Grid.SetColumn (pieceVis, col);

                GameBoard.Children.Add(pieceVis);
                _visuals[piece] = pieceVis;

                // Create a visual for the piece
                // Add the piece to the Grid
            }

        }

        private TextBlock createPieceVisual(Piece piece)
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
                PiecePressed?.Invoke(piece, pieceVisual);
            };

            return pieceVisual;
        }
    
        public void movePiece(Grid GameBoard, TextBlock pieceVis, Piece piece, int row, int col, ChessManager _manager)
        {
            GameBoard.Children.Remove(pieceVis);

            var target = _manager.fetchPieceAt(row, col);

            if(target is not null)
            {
                TextBlock pieceVisual = _visuals[target];
                GameBoard.Children.Remove(pieceVisual);
            } else if(target is null && piece is Pawn && piece.Column != col)
            {
                Piece capPawn = _manager.fetchPieceAt(piece.Row, col)!;
                TextBlock vis = _visuals[capPawn];
                GameBoard.Children.Remove(vis);
            } else if(piece is King && target is null)
            {
                int distance = Math.Abs(piece.Column - col);
                if(distance >= 2)
                {
                    int rookCol = col > piece.Column ? col + 1 : col - 2;  // Kingside: col+1, Queenside: col-2
                    int newRookCol = col > piece.Column ? col - 1 : col + 1;  // Kingside: col-1, Queenside: col+1
                    
                    Piece rook = _manager.fetchPieceAt(row, rookCol)!;
                    TextBlock rookVis = _visuals[rook];
                    
                    GameBoard.Children.Remove(rookVis);
                    Grid.SetRow(rookVis, row);
                    Grid.SetColumn(rookVis, newRookCol);
                    GameBoard.Children.Add(rookVis);
                }
            }
 
            Grid.SetRow    (pieceVis, row);
            Grid.SetColumn (pieceVis, col);

            GameBoard.Children.Add(pieceVis);
        }

        public void updatePieceVisual(Grid GameBoard, Piece piece, Piece newPiece)
        {
            TextBlock pVis = _visuals[piece];
            GameBoard.Children.Remove(pVis);
            _visuals.Remove(piece);

            int row = piece.Row; int col = piece.Column;

            TextBlock pieceVisual = createPieceVisual(newPiece);
            _visuals[newPiece] = pieceVisual;

            Grid.SetRow   (pieceVisual, row);
            Grid.SetColumn(pieceVisual, col);
            GameBoard.Children.Add(pieceVisual);
        }

        public void wipeBoard(Grid GameBoard)
        {
            foreach(var vis in _visuals)
            {
                GameBoard.Children.Remove(vis.Value);
            }
            _visuals.Clear();
        }

    }
}