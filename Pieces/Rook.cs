
using System.Collections.Generic;
using System.Data;
using Avalonia.Diagnostics.Screenshots;
using Chess.Board;
using SkiaSharp;

namespace Chess.Pieces
{
    public class Rook : Piece
    {
        public Rook(bool isWhite) : base(isWhite)
        {
            Texture = "♜";
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
                (0, 1)     // right
            };

            foreach(var (rowDir, colDir) in directions)
            {
                int newRow = Row;
                int newCol = Column;

                while(true)
                {
                    newRow += rowDir;
                    newCol += colDir;

                    if(newRow < 0 || newCol < 0 || newRow >= 8 || newCol >= 8) break;

                    if(board.isEmpty(newRow, newCol))
                    {
                        moves.Add((newRow, newCol));
                    } else if(board.GetPieceAt(newRow, newCol)?.IsWhite != IsWhite)
                    {
                        moves.Add((newRow, newCol));
                        break;
                    } else
                    {
                        break;
                    }

                }
            }

            return moves;
        }
    
        public override Piece Clone()
        {
            var clone = new Rook(this.IsWhite);
            clone.Row = this.Row; clone.Column = this.Column;
            clone.hasMoved = this.hasMoved;
            return clone;
        }

    }
}