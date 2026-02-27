
using System.Collections.Generic;
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

        /*

        public override List<(int, int)> GetLegalMoves(ChessBoard board) 
        {
            var moves = new List<(int, int)>();

            int direction = IsWhite ? -1 : 1;

            (int, int)[] dirs = new (int, int)[]
            {
                (-1, -1),  // up left
                (-1, 1),   // up right
                (1, -1),   // down left
                (1, 1)     // down right
            };

            var directions = IsWhite ? dirs[0..2] : dirs[2..4];

            foreach(var (rowDir, colDir) in directions)
            {
                int newR = Row;
                int newC = Column;
                newR += rowDir;
                newC += colDir;

                if(newR < 0 || newC < 0 || newR >= 8 || newC >= 8) continue;

                var posPiece = board.GetPieceAt(newR, newC);

                if(posPiece != null && posPiece.IsWhite != IsWhite)
                {
                    moves.Add((newR, newC));
                }

            }

            int newRow = Row + direction;

            if(board.isEmpty(newRow, Column))
            {
                moves.Add((newRow, Column));
            } 
            if(doubleMove && board.isEmpty(newRow, Column) && board.isEmpty(newRow+direction, Column))
            {
                moves.Add((newRow+direction, Column));
            }

            return moves;
        }

        public override Piece Clone()
        {
            var clone = new Pawn(this.IsWhite);
            clone.Row = this.Row; clone.Column = this.Column;
            return clone;
        }

        */

    }
}