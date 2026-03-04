using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Chess.GameManager;
using Chess.Pieces;

namespace Chess.UI
{
    //TODO:
    //when piece is clicked:
    //- get piece moves
    //- iterate through them
    //- set all of the squares colors to moveable
    public class MoveHighlighter
    {
        private List<Control> highlightedSquares = [];

        public event Action<Piece, TextBlock, int, int>? MoveMade;

        public void highlightPieceMoves(Piece piece, ChessManager manager, Grid GameBoard, TextBlock pieceVis)
        {
            var moves = piece.availableMoves(manager);

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
                highlightedSquares.Add(square);
            }

        }
    
        public void clearHighlights(Grid GameBoard)
        {
            foreach(var hl in highlightedSquares)
            {
                GameBoard.Children.Remove(hl);
            }
            highlightedSquares.Clear();
        }
    }
}