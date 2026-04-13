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
                createPiece(PieceType.Pawn, isWhite, new Position(pawnRow, col));

            int backRow = isWhite ? 7 : 0;

            createPiece(PieceType.Rook,   isWhite, new Position(backRow, 0));
            createPiece(PieceType.Knight, isWhite, new Position(backRow, 1));
            createPiece(PieceType.Bishop, isWhite, new Position(backRow, 2));
            createPiece(PieceType.Queen,  isWhite, new Position(backRow, 3));
            createPiece(PieceType.King,   isWhite, new Position(backRow, 4));
            createPiece(PieceType.Bishop, isWhite, new Position(backRow, 5));
            createPiece(PieceType.Knight, isWhite, new Position(backRow, 6));
            createPiece(PieceType.Rook,   isWhite, new Position(backRow, 7));
        }

        public void createPiece(PieceType type, bool isWhite, Position position)
        {
            Piece piece = PieceFactory.createPiece(type, isWhite, position);
            _state.Board[position.Row, position.Col] = piece;
        }

        public Piece? fetchPieceAt(Position position)
        {
            return _state.Board[position.Row, position.Col];
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