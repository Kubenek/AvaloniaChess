using Avalonia.Controls;
using System;
using Chess.Pieces;
using System.Collections.Generic;
using System.Linq;

namespace Chess.GameManager
{
    public class ChessManager
    {
        
        public Piece? [,] pieces = new Piece[8,8];
        public Piece? lastMovedPiece = null;
        public bool whiteTurn = true;
        public GameState state;

        public event Action<Pawn>? Promotion;

        public void initializePieces()
        {

            initializeSide(true);  // Initialize white pieces
            initializeSide(false); // Initialize black pieces

        }

        public void initializeSide(bool isWhite)
        {
            int pawnRow = isWhite ? 6 : 1;

            for(int col=0; col < 8; col++)  //* Initialize Pawn Row
                createPiece(new Pawn(isWhite), pawnRow, col);

            int backRow = isWhite ? 7 : 0;

            createPiece(new Rook   (isWhite), backRow, 0);
            createPiece(new Knight (isWhite), backRow, 1);
            createPiece(new Bishop (isWhite), backRow, 2);
            createPiece(new Queen  (isWhite), backRow, 3);
            createPiece(new King   (isWhite), backRow, 4);
            createPiece(new Bishop (isWhite), backRow, 5);
            createPiece(new Knight (isWhite), backRow, 6);
            createPiece(new Rook   (isWhite), backRow, 7);
        }

        public void createPiece(Piece piece, int row, int col)
        {
            piece.Row = row; piece.Column = col;
            pieces[row, col] = piece;
        }

        public bool movePiece(Piece piece, int row, int col)
        {
            bool capture = false;

            pieces.OfType<Pawn>().ToList().ForEach(p => p.lastMoveDouble = false);
            pieces[piece.Row, piece.Column] = null;

            if(piece is Pawn pawn) {
                int distance = Math.Abs(row - pawn.Row);
                pawn.doubleMove = false;
                if(distance == 2) pawn.lastMoveDouble = true;
            }

            var target = fetchPieceAt(row, col);

            if(piece is Pawn && target is null && piece.Column != col)
            {
                Piece cap = fetchPieceAt(piece.Row, col)!;
                capturePiece(cap);
                capture = true;
            } else if (piece is King king && target is null) {
                int distance = Math.Abs(piece.Column - col);
                if(distance >= 2)
                {
                    int rookCol = col > piece.Column ? col + 1 : col - 2;  // Kingside: col+1, Queenside: col-2
                    int newRookCol = col > piece.Column ? col - 1 : col + 1;  // Kingside: col-1, Queenside: col+1
                    
                    Piece rook = fetchPieceAt(row, rookCol)!;

                    king.Row    = row;
                    king.Column = col;
                    pieces[row, col] = king;
                    pieces[row, rookCol] = null;

                    rook.Row = row;
                    rook.Column = newRookCol;
                    pieces[row, newRookCol] = rook;

                    whiteTurn = !whiteTurn;

                    return false;
                }
            }
            else if(target is not null)
            {
                capturePiece(target); capture = true;
            }

            piece.Row    = row;
            piece.Column = col;
            pieces[row, col] = piece;

            if(piece is Pawn p && (row == 0 || row == 7))
            {
                Promotion?.Invoke(p);
                return capture;
            }

            whiteTurn = !whiteTurn;


            return capture;
        }

        public bool isMoveLegal(int fromRow, int fromCol, int toRow, int toCol)
        {
            ChessManager simulation = this.Clone();
            Piece? piece = simulation.pieces[fromRow, fromCol];
            if(piece is null) return false;

            simulation.movePiece(piece, toRow, toCol);

            King king = simulation.fetchKing(piece.IsWhite)!;
            
            return !simulation.isKingInCheck(king);
        }

        public void capturePiece(Piece capturedPiece)
        {   
            int row = capturedPiece.Row; int col = capturedPiece.Column;
            pieces[row, col] = null;
        }

        public Piece? fetchPieceAt(int row, int col)
        {
            return pieces[row, col];
        }

        public King? fetchKing(bool isWhite)
        {
            for(int i=0; i<8; i++)
            {
                for(int j=0; j<8; j++)
                {
                    var piece = pieces[i,j];
                    if(piece is King king && king.IsWhite == isWhite) return king; 
                }
            }
            return null;
        }

        public bool isKingInCheck(King king)
        {
            int row =    king.Row;
            int col = king.Column;

            for(int i=0; i<8; i++)
            {
                for(int j=0; j<8; j++) {
                    var piece = pieces[i,j];
                    if(piece is null) continue;
                    if(piece.IsWhite == king.IsWhite) continue;

                    var moves = piece.availableMoves(this);
                    foreach(var move in moves)
                    {
                        if(row == move.Item1 && col == move.Item2) return true;
                    }
                }
            }

            return false;
        }

        public bool isCheckmate(bool isWhite)
        {
            King king = this.fetchKing(isWhite)!;
            if(!this.isKingInCheck(king)) return false;
            
            var pcs = pieces.OfType<Piece>().Where(p => p.IsWhite == isWhite);
            
            var hasLegalMove = pcs.Any(piece => 
                piece.availableMoves(this).Any(move => 
                    isMoveLegal(piece.Row, piece.Column, move.Item1, move.Item2)
                )
            );

            return !hasLegalMove;
        }

        public bool isStalemate(bool isWhite)
        {
            King king = this.fetchKing(isWhite)!;
            if(this.isKingInCheck(king)) return false;

            var pcs = pieces.OfType<Piece>().Where(piece => piece.IsWhite == isWhite);

            var hasLegalMove = pcs.Any(piece => 
                piece.availableMoves(this).Any(move => 
                    isMoveLegal(piece.Row, piece.Column, move.Item1, move.Item2)
                )
            );

            return !hasLegalMove;
        }

        public ChessManager Clone()
        {
            ChessManager clone = new ();
            clone.whiteTurn = whiteTurn;

            for(int i=0; i<8; i++)
            {
                for(int j=0; j<8; j++)
                {
                    if(pieces[i,j] != null) clone.pieces[i,j] = pieces[i,j]!.Clone();
                }
            }

            return clone;
        }

    }
}