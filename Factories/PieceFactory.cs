using System;

using Chess;
using Chess.Pieces;

namespace Chess.Factories;

public static class PieceFactory
{
    public static Piece createPiece(PieceType type, bool isWhite, int row, int col)
    {
        Piece piece = type switch 
        {
            PieceType.Queen  => new   Queen(isWhite),
            PieceType.Rook   => new    Rook(isWhite),
            PieceType.Knight => new  Knight(isWhite),
            PieceType.Bishop => new  Bishop(isWhite),
            PieceType.King   => new    King(isWhite),
            PieceType.Pawn   => new    Pawn(isWhite),
            _ => throw new ArgumentException("Unknown piece type")
        };

        piece.Row = row;
        piece.Column = col;

        return piece;

    }
}