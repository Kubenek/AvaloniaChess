using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Chess.GameManager;
using Chess.Pieces;

namespace Chess.UI
{
    public class MoveHighlighter
    {
        private Border? [,] highlightedSquares = new Border[8,8];

        public event Action<Piece, TextBlock, int, int>? MoveMade;

        public void highlightPieceMoves(Piece piece, ChessManager manager, Grid GameBoard, TextBlock pieceVis, List<(int, int)> moves)
        {
            foreach(var move in moves)
            {
                int row = move.Item1; int col = move.Item2;

                var square = new Border
                {
                   Background = (row+col) % 2 == 0 ? Brushes.Green : Brushes.LimeGreen,
                   Opacity = 0.25
                };

                square.PointerPressed += (sender, e) =>
                {
                    MoveMade?.Invoke(piece, pieceVis, row, col);
                };

                Grid.SetRow(square, row);
                Grid.SetColumn(square, col);

                GameBoard.Children.Add(square);
                highlightedSquares[row, col] = square;
            }

        }
    
        public void clearHighlights(Grid GameBoard)
        {
            foreach(var hl in highlightedSquares)
            {
                if(hl is not null) GameBoard.Children.Remove(hl);
            }
            Array.Clear(highlightedSquares);
        }
    
        public void highlightCaptures(List<(int, int)> positions)
        {
            foreach(var pos in positions)
            {
                int row = pos.Item1; int col = pos.Item2;

                if(highlightedSquares[row, col] is not null) highlightedSquares[row, col]!.Background = Brushes.Red;

            }
        }
    }
}