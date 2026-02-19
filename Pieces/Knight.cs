
using System.Collections.Generic;
using System.Data;
using Avalonia.Diagnostics.Screenshots;
using Chess.Board;
using SkiaSharp;

namespace Chess.Pieces
{
    public class Knight : Piece
    {
        public Knight(bool isWhite) : base(isWhite)
        {
            Texture = "♞";
        }

        public override List<(int, int)> GetLegalMoves(ChessBoard board) 
        {
            var moves = new List<(int, int)>();

            (int, int)[] directions = new (int, int)[]
            {
                (-2, -1),  // two up left
                (-2, 1),   // two up right
                (-1, 2),   // two right up
                (1, 2),    // two right down
                (-1, -2),  // two left up
                (1, -2),   // two left down 
                (2, -1),   // two down left
                (2, 1)     // two down right
            };

            foreach(var (rowDir, colDir) in directions)
            {
                int newRow = Row;
                int newCol = Column;

                newRow += rowDir;
                newCol += colDir;

                if(newRow < 0 || newCol < 0 || newRow >= 8 || newCol >= 8) continue;

                if(board.isEmpty(newRow, newCol))
                {
                    moves.Add((newRow, newCol));
                } else if(board.GetPieceAt(newRow, newCol)?.IsWhite != IsWhite)
                {
                    moves.Add((newRow, newCol));
                    continue;
                } else
                {
                    continue;
                }
            }

            return moves;
        }
    
        public override Piece Clone()
        {
            var clone = new Knight(this.IsWhite);
            clone.Row = this.Row; clone.Column = this.Column;
            return clone;
        }
    }
}