
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Remote.Protocol.Input;
using Chess.Board;

namespace Chess.Pieces
{
    public abstract class Piece(bool isWhite)
    {
        public bool IsWhite { get; set; } = isWhite;

        public string Texture { get; protected set; }
        public TextBlock pieceVisual {get; set; }

        public int Row { get; set;}
        public int Column { get; set;}

        public abstract List<(int, int)> GetLegalMoves(ChessBoard board);

    }
}