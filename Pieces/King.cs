
using System.Collections.Generic;
using System.Data;
using Avalonia.Diagnostics.Screenshots;
using Chess.Board;
using SkiaSharp;

namespace Chess.Pieces
{
    public class King : Piece
    {
        public King(bool isWhite) : base(isWhite)
        {
            Texture = "♚";
        }

        public bool hasMoved = false;

        public override List<(int, int)> GetLegalMoves(ChessBoard board) 
        {
            var moves = new List<(int, int)>();

            (int, int)[] directions = new (int, int)[]
            {
                (-1, 0),   // up
                (1, 0),    // down
                (0, -1),   // left
                (0, 1),    // right
                (-1, -1),  // up-left
                (-1, 1),   // up-right
                (1, -1),   // down-left
                (1, 1)     // down-right
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
    }
}