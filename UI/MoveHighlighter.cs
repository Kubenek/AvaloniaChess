using System;
using System.Collections.Generic;

using Avalonia.Controls;
using Avalonia.Media;

using Chess.Pieces;

namespace Chess.UI
{
    public class MoveHighlighter
    {
        private Border? [,] highlightedSquares = new Border[8,8];
        private Border? [,] checks             = new Border[8,8];

        public event Action<Piece, TextBlock, Move>? MoveMade;

        public void highlightReviewMove(Grid gameBoard, Move move)
        {
            var fromSquare = new Border
            {
                Background = Brushes.Blue,
                Opacity = 0.25
            };
            Grid.SetRow(fromSquare, move.From.Row);
            Grid.SetColumn(fromSquare, move.From.Col);
            gameBoard.Children.Add(fromSquare);
            highlightedSquares[move.From.Row, move.From.Col] = fromSquare;

            var toSquare = new Border
            {
                Background = Brushes.Blue,
                Opacity = 0.25
            };
            Grid.SetRow(toSquare, move.To.Row);
            Grid.SetColumn(toSquare, move.To.Col);
            gameBoard.Children.Add(toSquare);
            highlightedSquares[move.To.Row, move.To.Col] = toSquare;
        }

        public void highlightPieceMoves(Piece piece, Grid GameBoard, TextBlock pieceVis, List<Position> moves)
        {
            foreach(var pos in moves)
            {
                int row = pos.Row; int col = pos.Col;

                Position from = piece.Coords;
                Position to = new(row, col);
                Move move = new Move(from, to);

                var square = new Border
                {
                   Background = (row+col) % 2 == 0 ? Brushes.Green : Brushes.LimeGreen,
                   Opacity = 0.25
                };

                square.PointerPressed += (sender, e) =>
                {
                    MoveMade?.Invoke(piece, pieceVis, move);
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
    
        public void highlightCaptures(List<Position> positions)
        {
            foreach(var pos in positions)
            {
                int row = pos.Row; int col = pos.Col;

                if(highlightedSquares[row, col] is not null) highlightedSquares[row, col]!.Background = Brushes.Red;

            }
        }
    
        public void highlightCheck(Grid GameBoard, Position pos)
        {
            var square = new Border
            {
                Background = Brushes.DarkRed,
                Opacity = 0.5,
                IsHitTestVisible = false
            };

            Grid.SetRow(square, pos.Row);
            Grid.SetColumn(square, pos.Col);

            GameBoard.Children.Add(square);
            checks[pos.Row, pos.Col] = square;
        }
    
        public bool isHighlighted(Position pos)
        {
            if(highlightedSquares[pos.Row, pos.Col] is not null) return true;
            if(checks[pos.Row, pos.Col] is not null) return true;
            return false;
        }

        public void clearCheck(Grid GameBoard)
        {
            foreach(var hl in checks)
            {
                if(hl is not null) GameBoard.Children.Remove(hl);
            }
            Array.Clear(checks);
        }
    }
}