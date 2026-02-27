
using System.Collections.Generic;
using System.Data;
using Avalonia.Diagnostics.Screenshots;
using SkiaSharp;

namespace Chess.Pieces
{
    public class Queen : Piece
    {
        public Queen(bool isWhite) : base(isWhite)
        {
            Texture = "♛";
        }

        /*

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
            var clone = new Queen(this.IsWhite);
            clone.Row = this.Row; clone.Column = this.Column;
            return clone;
        }
        */
    }
}