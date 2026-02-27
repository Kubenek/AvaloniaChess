
using System.Collections.Generic;
using System.Data;
using Avalonia.Controls;
using Avalonia.Diagnostics.Screenshots;
using Avalonia.Vulkan;
using Chess.GameManager;
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

        public override List<(int, int)> availableMoves(ChessManager manager) 
        {
            var moves = new List<(int, int)>();

            (int, int)[] directions =
            [
                (-1, 0),   // up
                (1, 0),    // down
                (0, -1),   // left
                (0, 1)     // right
            ];

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