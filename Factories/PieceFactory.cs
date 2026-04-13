using System;

using Chess;
using Chess.Pieces;

namespace Chess.Factories;

public static class PieceFactory
{
    public static Piece createPiece(PieceType type, bool isWhite, Position coords)
    {
        Piece piece = type switch 
        {
            PieceType.Queen  => new   Queen(isWhite, coords),
            PieceType.Rook   => new    Rook(isWhite, coords),
            PieceType.Knight => new  Knight(isWhite, coords),
            PieceType.Bishop => new  Bishop(isWhite, coords),
            PieceType.King   => new    King(isWhite, coords),
            PieceType.Pawn   => new    Pawn(isWhite, coords),
            _ => throw new ArgumentException("Unknown piece type")
        };

        return piece;

    }
}