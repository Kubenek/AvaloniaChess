
using System.Collections.Generic;
using System.Data;
using Avalonia.Diagnostics.Screenshots;
using Chess.GameManager;
using SkiaSharp;

namespace Chess.Pieces
{
    public class Queen : Piece
    {
        public Queen(bool isWhite) : base(isWhite)
        {
            Texture = "♛";
        }

        public override List<(int, int)> availableMoves(ChessManager manager) 
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

            var pieces = manager.pieces;

            foreach(var (rowDir, colDir) in directions)
            {
                int currentRow =    Row;
                int currentCol = Column;

                while(true)
                {
                    currentRow += rowDir;
                    currentCol += colDir;

                    if(currentRow < 0 || currentCol < 0 || currentRow >= 8 || currentCol >= 8) break;

                    var targetSquare = pieces[currentRow, currentCol];

                    if(targetSquare == null)
                        moves.Add((currentRow, currentCol));
                    else if(targetSquare.IsWhite != IsWhite)
                    {
                        moves.Add((currentRow, currentCol));
                        break;
                    }
                    else break;
                }

            }

            return moves;
        }
    
    }
}