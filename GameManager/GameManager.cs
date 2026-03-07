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
        public bool whiteTurn = true;

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
            createPiece(new Knight (isWhite), backRow, 5);
            createPiece(new Bishop (isWhite), backRow, 6);
            createPiece(new Rook   (isWhite), backRow, 7);
        }

        public void createPiece(Piece piece, int row, int col)
        {
            piece.Row = row; piece.Column = col;
            pieces[row, col] = piece;
        }

        public void movePiece(Piece piece, int row, int col)
        {
            pieces[piece.Row, piece.Column] = null;
            if(piece is Pawn pawn) pawn.doubleMove = false;

            var target = fetchPieceAt(row, col);

            if(target is not null) capturePiece(target);

            piece.Row    = row;
            piece.Column = col;

            pieces[row, col] = piece;

            whiteTurn = !whiteTurn;
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

    }
}