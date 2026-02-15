
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Remote.Protocol.Input;
using Chess.Board;

namespace Chess.Pieces
{
    public abstract class Piece
    {
        public bool IsWhite { get; set; }

        public string Texture { get; protected set; }
        public TextBlock pieceVisual {get; set; } = new TextBlock();

        public int Row { get; set;}
        public int Column { get; set;}

        public Piece(bool isWhite)
        {
            IsWhite = isWhite;
            Texture = "";
        }

        public abstract List<(int, int)> GetLegalMoves(ChessBoard board);

    }
}