using Avalonia.Controls;
using System;
using Chess.Pieces;
using System.Collections.Generic;
using System.Linq;

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

        public King? GetKing(bool isWhite)
        {
            foreach(var piece in Pieces)
            {
                if(piece is King king && king.IsWhite == isWhite)
                {
                    return king;
                }
            }
            return null;
        }

        public bool isKingInCheck(King king)
        {
            var rowK = king.Row;
            var colK = king.Column;

            foreach (var piece in Pieces)
            {

                if(king.IsWhite == piece.IsWhite) continue;

                var pieceMoves = piece.GetLegalMoves(this);

                foreach(var move in pieceMoves)
                {
                    if(move.Item1 == rowK && move.Item2 == colK)
                    {
                        return true;
                    } else
                    {
                        continue;
                    }
                }
            }
            return false;
        }

        public ChessBoard Clone()
        {
            var clone = new ChessBoard();
            
            foreach(var p in this.Pieces)
            {
                clone.Pieces.Add(p.Clone());
            }

            return clone;
        }

    }
}