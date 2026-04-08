using System;
using System.Linq;

using Chess.Pieces;
using Chess.Factories;

namespace Chess.Logic
{
    public class ChessManager
    {
        public GameState _state;

        public ChessManager(GameState state)
        {
            _state = state;
        }

        public void initializePieces()
        {

            initializeSide(true);  // Initialize white pieces
            initializeSide(false); // Initialize black pieces

        }

        public void initializeSide(bool isWhite)
        {
            int pawnRow = isWhite ? 6 : 1;

            for(int col=0; col < 8; col++)  //* Initialize Pawn Row
                createPiece(PieceType.Pawn, isWhite, pawnRow, col);

            int backRow = isWhite ? 7 : 0;

            createPiece(PieceType.Rook,   isWhite, backRow, 0);
            createPiece(PieceType.Knight, isWhite, backRow, 1);
            createPiece(PieceType.Bishop, isWhite, backRow, 2);
            createPiece(PieceType.Queen,  isWhite, backRow, 3);
            createPiece(PieceType.King,   isWhite, backRow, 4);
            createPiece(PieceType.Bishop, isWhite, backRow, 5);
            createPiece(PieceType.Knight, isWhite, backRow, 6);
            createPiece(PieceType.Rook,   isWhite, backRow, 7);
        }

        public void createPiece(PieceType type, bool isWhite, int row, int col)
        {
            Piece piece = PieceFactory.createPiece(type, isWhite, row, col);
            _state.Board[row, col] = piece;
        }

        public Piece? fetchPieceAt(int row, int col)
        {
            return _state.Board[row, col];
        }

        public King? fetchKing(bool isWhite)
        {
            for(int i=0; i<8; i++)
            {
                for(int j=0; j<8; j++)
                {
                    var piece = _state.Board[i,j];
                    if(piece is King king && king.IsWhite == isWhite) return king; 
                }
            }
            return null;
        }

        public ChessManager Clone()
        {
            return new ChessManager(_state.Clone());
        }

    }
}