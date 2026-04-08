
using System.Collections.Generic;
using System.Data;
using Avalonia.Diagnostics.Screenshots;
using Chess.Logic;
using SkiaSharp;

namespace Chess.Pieces
{
    public class Knight : Piece
    {
        public Knight(bool isWhite) : base(isWhite)
        {
            Texture = "♞";
        }

        public override List<(int, int)> availableMoves(ChessManager manager) 
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
            Knight knight = new(IsWhite);
            knight.Row = Row;
            knight.Column = Column;

            return knight;
        }

    }
}