using System.Linq;
using Chess.Pieces;

namespace Chess.Logic;

public static class Evaluator
{
    public static GameStateType EvaluateGame(bool isWhite, MoveEngine _engine, ChessManager _manager)
    {
        King king  = _manager.fetchKing(isWhite)!;
        
        if (isKingInCheck(king, _manager))
        return isCheckmate(isWhite, _engine, _manager)
            ? GameStateType.Checkmate
            : GameStateType.Check;

        return isStalemate(isWhite, _engine, _manager)
            ? GameStateType.Stalemate
            : GameStateType.Playing;
    }

    public static bool isKingInCheck(King king, ChessManager _manager)
    {
        for(int i=0; i<8; i++)
        {
            for(int j=0; j<8; j++) {
                var piece = _manager._state.Board[i,j];
                if(piece is null) continue;
                if(piece.IsWhite == king.IsWhite) continue;

                var moves = piece.availableMoves(_manager);
                foreach(var move in moves)
                {
                    if(king.Coords == move) return true;
                }
            }
        }

        return false;
    }

    public static bool isCheckmate(bool isWhite, MoveEngine _engine, ChessManager _manager)
    {
        King king = _manager.fetchKing(isWhite)!;
        if(!isKingInCheck(king, _manager)) return false;
        
        var pcs = _manager._state.Board.OfType<Piece>().Where(p => p.IsWhite == isWhite);
        
        var hasLegalMove = pcs.Any(piece => 
            piece.availableMoves(_manager).Any(position => 
                _engine.isMoveLegal(new Move(piece.Coords, position), _manager)
            )
        );

        return !hasLegalMove;
    }

    public static bool isStalemate(bool isWhite, MoveEngine _engine, ChessManager _manager)
    {
        King king = _manager.fetchKing(isWhite)!;
        if(isKingInCheck(king, _manager)) return false;

        var pcs = _manager._state.Board.OfType<Piece>().Where(piece => piece.IsWhite == isWhite);

        var hasLegalMove = pcs.Any(piece => 
            piece.availableMoves(_manager).Any(position => 
                _engine.isMoveLegal(new Move(piece.Coords, position), _manager)
            )
        );

        return !hasLegalMove;
    }


}