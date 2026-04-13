
using System.Collections.Generic;
using Chess.Logic;
using SkiaSharp;

namespace Chess.Pieces
{
    public class Pawn : Piece
    {
        public Pawn(bool isWhite, Position coords) : base(isWhite, coords)
        {
            Texture = "♟";
        }

        public bool doubleMove = true;
        public bool lastMoveDouble = false;

        public override List<Position> availableMoves(ChessManager manager) 
        {
            var moves = new List<Position>();

            int direction = IsWhite ? -1 : 1;

            var currentRow = Coords.Row + direction;
            if(currentRow < 0 || currentRow >= 8) return moves;
            var currentCol = Coords.Col;

            Position pos = new(currentRow, currentCol);
            var targetSquare = manager.fetchPieceAt(pos);

            if(targetSquare == null) {
                moves.Add(pos);

                if(doubleMove)
                {
                    pos = new (currentRow + direction, currentCol);
                    targetSquare = manager.fetchPieceAt(pos);
                    if(targetSquare == null) moves.Add(pos);
                }
            }
            if(currentCol-1 >= 0 && currentCol-1 < 8) {
                pos = new (currentRow, currentCol-1);
                var t1 = manager.fetchPieceAt(pos);
                if(t1 != null && t1.IsWhite != IsWhite) moves.Add(pos);
            }
            if(currentCol+1 >= 0 && currentCol+1 < 8) {
                pos = new (currentRow, currentCol+1);
                var t2 = manager.fetchPieceAt(pos);
                if(t2 != null && t2.IsWhite != IsWhite) moves.Add(pos);
            }

            return moves;
        }

        public override Piece Clone()
        {
            Pawn pawn = new(IsWhite, Coords);
            pawn.doubleMove = doubleMove;
            pawn.lastMoveDouble = lastMoveDouble;
            return pawn;
        }

    }
}