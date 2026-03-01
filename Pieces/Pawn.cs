
using System.Collections.Generic;
using Chess.GameManager;
using SkiaSharp;

namespace Chess.Pieces
{
    public class Pawn : Piece
    {
        public Pawn(bool isWhite) : base(isWhite)
        {
            Texture = "♟";
        }

        public bool doubleMove = true;

        public override List<(int, int)> availableMoves(ChessManager manager) 
        {
            var moves = new List<(int, int)>();

            int direction = IsWhite ? -1 : 1;

            var currentRow = Row    + direction;
            var currentCol = Column;

            var pieces = manager.pieces;
            var targetSquare = pieces[currentRow, currentCol];

            if(targetSquare == null)
                moves.Add((currentRow, currentCol));

            return moves;
        }


    }
}