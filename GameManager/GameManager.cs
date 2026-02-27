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

        public void initializePieces()
        {

            initializeSide(true);  //* Initialize white pieces
            initializeSide(false); //* Initialize black pieces

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

    }
}