using System.Collections.Generic;
using Chess.Pieces;
using Microsoft.VisualBasic;

namespace Chess.Logic;

public static class FENGenerator
{
    public static string GenerateOutput(Piece?[,] board, bool isWhiteTurn)
    {

        var rows = new List<string>();

        for(int row=0; row < 8; row++)
        {
            var rowString = "";
            int emptyCount = 0;

            for(int col=0; col < 8; col++)
            {
                var piece = board[row, col];
                if(piece == null)
                {
                    emptyCount++;
                    continue;
                }

                if(emptyCount > 0)
                {
                    rowString += emptyCount.ToString();
                    emptyCount = 0;
                }
                rowString += PieceToFEN(piece);

            }

            if(emptyCount > 0) rowString += emptyCount.ToString();
            rows.Add(rowString);
        }

        var turn = isWhiteTurn ? "w" : "b";
        return $"{string.Join("/", rows)} {turn} KQkq - 0 1";
    }

    public static char PieceToFEN(Piece piece)
    {
        char ch = piece switch
        {
            Pawn   => 'p',
            Knight => 'n',
            King   => 'k',
            Bishop => 'b',
            Queen  => 'q',
            Rook   => 'r',
            _      => '?'
        };

        return piece.IsWhite ? char.ToUpper(ch) : ch;
    }
}