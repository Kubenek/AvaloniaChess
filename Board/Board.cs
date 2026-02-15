using Avalonia.Controls;
using System;
using Chess.Pieces;
using System.Collections.Generic;

namespace Chess.Board
{
    public class ChessBoard
    {
        public List<Piece> Pieces { get; } = new List<Piece>();

        public bool isEmpty(int row, int column)
        {
            if (row < 0 || row >= 8 || column < 0 || column >= 8) return false;

            foreach (var piece in Pieces)
            {
                if (piece.Row == row && piece.Column == column)
                    return false; 
            }

            return true; 
        }

        public Piece? GetPieceAt(int row, int col)
        {
            foreach(var piece in Pieces)
            {
                if (piece.Row == row && piece.Column == col)
                    return piece;
            }

            return null;
        }

    }
}