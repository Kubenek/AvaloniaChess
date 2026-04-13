
using System.Collections.Generic;
using System.Data;
using Avalonia.Diagnostics.Screenshots;
using Chess.Logic;
using SkiaSharp;

namespace Chess.Pieces
{
    public class Queen : Piece
    {
        public Queen(bool isWhite, Position coords) : base(isWhite, coords)
        {
            Texture = "♛";
        }

        public override List<Position> availableMoves(ChessManager manager) 
        {
            var moves = new List<Position>();

            (int, int)[] directions = Directions.All;

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
            return new Queen(IsWhite, Coords);
        }

    }
}