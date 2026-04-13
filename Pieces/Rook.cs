
using System.Collections.Generic;
using System.Data;
using Avalonia.Controls;
using Avalonia.Diagnostics.Screenshots;
using Avalonia.Vulkan;
using Chess.Logic;
using SkiaSharp;

namespace Chess.Pieces
{
    public class Rook : Piece
    {
        public Rook(bool isWhite, Position coords) : base(isWhite, coords)
        {
            Texture = "♜";
        }

        public bool hasMoved = false;

        public override List<Position> availableMoves(ChessManager manager) 
        {
            var moves = new List<Position>();

            (int, int)[] directions =
            [
                (-1, 0),   // up
                (1, 0),    // down
                (0, -1),   // left
                (0, 1)     // right
            ];

            foreach(var (rowDir, colDir) in directions)
            {
                int currentRow = Coords.Row;
                int currentCol = Coords.Col;

                while(true)
                {
                    currentRow += rowDir;
                    currentCol += colDir;

                    if(currentRow < 0 || currentCol < 0 || currentRow >= 8 || currentCol >= 8) break;

                    Position pos = new(currentRow, currentCol);
                    var targetSquare = manager.fetchPieceAt(pos);

                    if(targetSquare == null)
                        moves.Add(pos);
                    else if(targetSquare.IsWhite != IsWhite)
                    {
                        moves.Add(pos);
                        break;
                    }
                    else break;
                }

            }

            return moves;
        }

        public override Piece Clone()
        {
            Rook rook = new(IsWhite, Coords);
            rook.hasMoved = hasMoved;

            return rook;
        }

    }
}