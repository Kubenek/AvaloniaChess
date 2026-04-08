using System;
using System.Linq;

using Chess.Pieces;

namespace Chess.Logic;

public class MoveEngine
{
    public event Action<Pawn>? Promotion;
    
    public bool movePiece(Piece piece, int row, int col, ChessManager _manager)
    {
        bool capture = false;

        _manager._state.Board.OfType<Pawn>().ToList().ForEach(p => p.lastMoveDouble = false);
        _manager._state.Board[piece.Row, piece.Column] = null;

        if(piece is Pawn pawn) {
            int distance = Math.Abs(row - pawn.Row);
            pawn.doubleMove = false;
            if(distance == 2) pawn.lastMoveDouble = true;
        }

        var target = _manager.fetchPieceAt(row, col);

        if(piece is Pawn && target is null && piece.Column != col)
        {
            Piece cap = _manager.fetchPieceAt(piece.Row, col)!;
            capturePiece(cap, _manager);
            capture = true;
        } else if (piece is King king && target is null) {
            int distance = Math.Abs(piece.Column - col);
            if(distance >= 2)
            {
                int rookCol = col > piece.Column ? col + 1 : col - 2;  // Kingside: col+1, Queenside: col-2
                int newRookCol = col > piece.Column ? col - 1 : col + 1;  // Kingside: col-1, Queenside: col+1
                
                Piece rook = _manager.fetchPieceAt(row, rookCol)!;

                king.Row    = row;
                king.Column = col;
                _manager._state.Board[row, col] = king;
                _manager._state.Board[row, rookCol] = null;

                rook.Row = row;
                rook.Column = newRookCol;
                _manager._state.Board[row, newRookCol] = rook;

                _manager._state.IsWhiteTurn = !_manager._state.IsWhiteTurn;

                return false;
            }
        }
        else if(target is not null)
        {
            capturePiece(target, _manager); capture = true;
        }

        piece.Row    = row;
        piece.Column = col;
        _manager._state.Board[row, col] = piece;

        if(piece is Pawn p && (row == 0 || row == 7))
        {
            Promotion?.Invoke(p);
            return capture;
        }

        _manager._state.IsWhiteTurn = !_manager._state.IsWhiteTurn;


        return capture;
    }

    private void capturePiece(Piece capturedPiece, ChessManager _manager)
    {   
        int row = capturedPiece.Row; int col = capturedPiece.Column;
        _manager._state.Board[row, col] = null;
    }

    public bool isMoveLegal(int fromRow, int fromCol, int toRow, int toCol, ChessManager _manager)
    {
        ChessManager simulation = _manager.Clone();
        Piece? piece = simulation._state.Board[fromRow, fromCol];
        if(piece is null) return false;

        movePiece(piece, toRow, toCol, simulation);

        King king = simulation.fetchKing(piece.IsWhite)!;
        
        return !Evaluator.isKingInCheck(king, simulation);
    }

}