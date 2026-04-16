using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Chess.Pieces;

namespace Chess.Logic;

public class MoveEngine
{
    public event Action<Pawn>? Promotion;
    
    public bool movePiece(Piece piece, Position dest, ChessManager _manager, bool simulation)
    {
        bool capture = false;

        _manager._state.Board.OfType<Pawn>().ToList().ForEach(p => p.lastMoveDouble = false);
        _manager._state.Board[piece.Coords.Row, piece.Coords.Col] = null;

        if(piece is Pawn pawn) {
            int distance = Math.Abs(dest.Row - pawn.Coords.Row);
            pawn.doubleMove = false;
            if(distance == 2) pawn.lastMoveDouble = true;
        }

        var target = _manager.fetchPieceAt(dest);

        if(piece is Pawn && target is null && piece.Coords.Col != dest.Col)
        {
            Piece cap = _manager.fetchPieceAt(new Position(piece.Coords.Row, dest.Col))!;
            capturePiece(cap, _manager);
            capture = true;
        } else if (piece is King king && target is null) {
            int distance = Math.Abs(piece.Coords.Col - dest.Col);
            if(distance >= 2)
            {
                int rookCol = dest.Col > piece.Coords.Col ? dest.Col + 1 : dest.Col - 2;  // Kingside: col+1, Queenside: col-2
                int newRookCol = dest.Col > piece.Coords.Col ? dest.Col - 1 : dest.Col + 1;  // Kingside: col-1, Queenside: col+1
                
                Piece rook = _manager.fetchPieceAt(new Position(dest.Row, rookCol))!;
                Position kingPos = new(dest.Row, dest.Col);
                Position rookPos = new(dest.Row, newRookCol);

                king.Coords = kingPos;
                rook.Coords = rookPos;
                _manager._state.Board[dest.Row, dest.Col] = king;
                _manager._state.Board[dest.Row, rookCol] = null;
                _manager._state.Board[dest.Row, newRookCol] = rook;

                _manager._state.IsWhiteTurn = !_manager._state.IsWhiteTurn;

                return false;
            }
        }
        else if(target is not null)
        {
            capturePiece(target, _manager); 
            capture = true;
        }

        Position newPos = new(dest.Row, dest.Col);
        
        piece.Coords = newPos;
        _manager._state.Board[dest.Row, dest.Col] = piece;

        if(piece is Pawn p && (dest.Row == 0 || dest.Row == 7) && !simulation)
        {
            Promotion?.Invoke(p);
            return capture;
        }

        if(!simulation)
            _manager._state.IsWhiteTurn = !_manager._state.IsWhiteTurn;

        return capture;
    }

    private void capturePiece(Piece capturedPiece, ChessManager _manager)
    {   
        int row = capturedPiece.Coords.Row; int col = capturedPiece.Coords.Col;
        _manager._state.Board[row, col] = null;
    }

    public bool isMoveLegal(Move move, ChessManager _manager)
    {
        ChessManager simulation = _manager.Clone();
        Piece? piece = simulation._state.Board[move.From.Row, move.From.Col];
        if(piece is null) return false;

        movePiece(piece, move.To, simulation, true);

        King king = simulation.fetchKing(piece.IsWhite)!;
        
        return !Evaluator.isKingInCheck(king, simulation);
    }

    public (List<Position> moves, List<Position> captures) getPieceMoves(Piece piece, ChessManager manager)
    {
        List<Position> positions = piece.availableMoves(manager);

        List<Position> moves    = [];
        List<Position> captures = [];

        foreach(var pos in positions)
        {
            Move move = new(piece.Coords, pos);
            if(!isMoveLegal(move, manager)) continue;
            moves.Add(move.To);

            var target = manager.fetchPieceAt(move.To); 
            if(target != null && target.IsWhite != piece.IsWhite) captures.Add(move.To);
        }

        if (piece is Pawn pawn)
        {
            var epMoves = getEnPassantMoves(pawn, manager);

            moves.   AddRange(epMoves);
            captures.AddRange(epMoves);
        }

        if(piece is King king && !king.hasMoved)
            moves.AddRange(getCastleMoves(king, manager));

        return (moves, captures);
    }

    private List<Position> getEnPassantMoves(Pawn pawn, ChessManager manager)
    {
        List<Position> positions = [];

        for(int i=0; i<2; i++)
        {
            int column = (i==0) ? pawn.Coords.Col - 1 : pawn.Coords.Col + 1;
            int row = pawn.Coords.Row;

            if(column < 0 || column >= 8) continue;
            
            var target = manager.fetchPieceAt(new Position(row, column));

            if(target != null && target.IsWhite != pawn.IsWhite && target is Pawn epawn && epawn.lastMoveDouble)
            {
                int r = pawn.IsWhite ? pawn.Coords.Row - 1 : pawn.Coords.Row + 1;
                int c = column;

                Position posFrom = new(row, pawn.Coords.Col);
                Position posTo = new(r, c);
                Move mv = new(posFrom, posTo);

                if(!isMoveLegal(mv, manager)) continue;   
                positions.Add(posTo);    
            }
            
        }

        return positions;
    }

    private List<Position> getCastleMoves(King king, ChessManager manager)
    {
        List<Position> positions = [];

        int row = king.Coords.Row;
        int col = king.Coords.Col;

        var c1 = manager.fetchPieceAt(new Position(row, col+1));
        var c2 = manager.fetchPieceAt(new Position(row, col+2));
        var c3 = manager.fetchPieceAt(new Position(row, col+3));

        if(c1 is null && c2 is null && c3 is Rook rook && !rook.hasMoved) 
            positions.Add(new Position(row, col+2));
        
        var cl1 = manager.fetchPieceAt(new Position(row, col-1));
        var cl2 = manager.fetchPieceAt(new Position(row, col-2));
        var cl3 = manager.fetchPieceAt(new Position(row, col-3));
        var cl4 = manager.fetchPieceAt(new Position(row, col-4));

        if(cl1 is null && cl2 is null && cl3 is null && cl4 is Rook r && !r.hasMoved) 
            positions.Add(new Position(row, col-2));

        return positions;
    }

}