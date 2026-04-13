
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using Avalonia.Diagnostics.Screenshots;
using Chess.Logic;
using SkiaSharp;

namespace Chess.Pieces
{
    public class King : Piece
    {
        public King(bool isWhite, Position coords) : base(isWhite, coords)
        {
            Texture = "♚";
        }

        public bool hasMoved = false;

        public override List<Position> availableMoves(ChessManager manager) 
        {
            var moves = new List<Position>();

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
                int currentRow = Coords.Row + rowDir;
                int currentCol = Coords.Col + colDir;

                if(currentRow < 0 || currentCol < 0 || currentRow >= 8 || currentCol >= 8) continue;

                Position pos = new(currentRow, currentCol);
                var targetSquare = manager.fetchPieceAt(pos);

                if(targetSquare == null || targetSquare.IsWhite != IsWhite)
                    moves.Add(pos);

            }

            return moves;
        }

        public override Piece Clone()
        {
            return new King(IsWhite, Coords);
        }

    }
}