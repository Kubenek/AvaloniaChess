
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Remote.Protocol.Input;
using Chess.Logic;

namespace Chess.Pieces
{
    public abstract class Piece(bool isWhite, Position coords)
    {
        public bool IsWhite { get; set; } = isWhite;

        public string Texture { get; protected set; } = "";

        public Position Coords { get; set; } = coords;

        public abstract List<Position> availableMoves(ChessManager manager);
        public abstract Piece Clone();

    }
}