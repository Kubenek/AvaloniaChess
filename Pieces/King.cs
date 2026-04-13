
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

            (int, int)[] directions = Directions.All;

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