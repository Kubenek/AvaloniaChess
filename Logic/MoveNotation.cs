using Chess.Pieces;
using System;

namespace Chess.Logic;

public static class MoveNotation
{
    public static string getNotation(Piece piece, Move move, bool capture, GameStateType state)
    {
        char toColumn   = (char)('a' + move.To.Col);
        char fromColumn = (char)('a' + move.From.Col);
        int  toRank     = 8 - move.To.Row;

        string checkSymbol   = getCheckSymbol(state);
        string captureSymbol = capture ? "x" : "";

        return piece switch
        {
            Pawn   => getPawnNotation(fromColumn, toColumn, toRank, capture, checkSymbol),
            King   => getKingNotation(move, toColumn, toRank, captureSymbol, checkSymbol),
            Bishop =>  $"B{captureSymbol}{toColumn}{toRank}{checkSymbol}", 
            Rook   =>  $"R{captureSymbol}{toColumn}{toRank}{checkSymbol}",
            Queen  =>  $"Q{captureSymbol}{toColumn}{toRank}{checkSymbol}",
            Knight =>  $"N{captureSymbol}{toColumn}{toRank}{checkSymbol}",
            _ => ""
        };
    }

    private static string getPawnNotation(char fromColumn, char toColumn, int toRank, bool capture, string checkSymbol)
    {
        if (capture)
            return $"{fromColumn}x{toColumn}{toRank}{checkSymbol}";
        
        return $"{toColumn}{toRank}{checkSymbol}";
    }

    private static string getKingNotation(Move move, char toColumn, int toRank, string captureSymbol, string checkSymbol)
    {
        int horizontalDist = Math.Abs(move.From.Col - move.To.Col);

        if (horizontalDist >= 2)
        {
            bool isKingside = move.To.Col > move.From.Col;
            return isKingside ? "O-O" : "O-O-O";
        }

        return $"K{captureSymbol}{toColumn}{toRank}{checkSymbol}";
    }

    private static string getCheckSymbol(GameStateType state)
    {
        return state switch
        {
            GameStateType.Check => "+",
            GameStateType.Checkmate => "#",
            _ => ""  
        };
    }

    public static string addPromoteNotation(string baseMove, PieceType type)
    {
        string symbol = type switch
        {
            PieceType.Queen  => "Q",
            PieceType.Rook   => "R",
            PieceType.Knight => "N",
            PieceType.Bishop => "B",
            _                => "Q"
        };

        return $"{baseMove}={symbol}";
    }
}