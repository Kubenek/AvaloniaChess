
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
        public King(bool isWhite) : base(isWhite)
        {
            Texture = "♚";
        }

        public bool hasMoved = false;

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

            foreach(var (rowDir, colDir) in directions)
            {
                int currentRow =    Row + rowDir;
                int currentCol = Column + colDir;

                if(currentRow < 0 || currentCol < 0 || currentRow >= 8 || currentCol >= 8) continue;

                var pieces = manager._state.Board;

                var targetSquare = pieces[currentRow, currentCol];

                if(targetSquare == null || targetSquare.IsWhite != IsWhite)
                    moves.Add((currentRow, currentCol));

            }

            return moves;
        }

        public override Piece Clone()
        {
            King king = new(IsWhite);
            king.Row = Row;
            king.Column = Column;
            king.hasMoved = hasMoved;

            return king;
        }

    }
}