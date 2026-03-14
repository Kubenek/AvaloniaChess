
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
        public bool lastMoveDouble = false;

        public override List<(int, int)> availableMoves(ChessManager manager) 
        {
            var moves = new List<(int, int)>();

            int direction = IsWhite ? -1 : 1;

            var currentRow = Row + direction;
            if(currentRow < 0 || currentRow >= 8) return moves;

            var currentCol = Column;

            var pieces = manager.pieces;

            var targetSquare = pieces[currentRow, currentCol];
            if(targetSquare == null) {

                moves.Add((currentRow, currentCol));

                if(doubleMove)
                {
                    targetSquare = pieces[currentRow + direction, currentCol];
                    if(targetSquare == null) moves.Add((currentRow + direction, currentCol));
                }
            }
            if(currentCol-1 >= 0 && currentCol-1 < 8) {
                var t1 = pieces[currentRow, currentCol-1];
                if(t1 != null && t1.IsWhite != IsWhite) moves.Add((currentRow, currentCol-1));
            }
            if(currentCol+1 >= 0 && currentCol+1 < 8) {
                var t2 = pieces[currentRow, currentCol+1];
                if(t2 != null && t2.IsWhite != IsWhite) moves.Add((currentRow, currentCol+1));
            }

            return moves;
        }

        public override Piece Clone()
        {
            Pawn pawn = new(IsWhite);
            pawn.Row = Row;
            pawn.Column = Column;
            pawn.doubleMove = doubleMove;
            pawn.lastMoveDouble = lastMoveDouble;

            return pawn;
        }

    }
}