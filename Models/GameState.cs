using System.Runtime.Serialization;
using Chess.Pieces;

namespace Chess;

public class GameState
{
    public Piece?[,] Board { get; set; }

    public Piece? LastMovedPiece { get; set; }

    public bool IsWhiteTurn { get; set; }

    public GameStateType CurrentState { get; set; }

    public GameState()
    {
        Board = new Piece?[8,8];
        LastMovedPiece = null;
        IsWhiteTurn = true;
        CurrentState = GameStateType.Playing;
    }

    public GameState Clone()
    {
        GameState clone = new();
        
        clone.IsWhiteTurn = IsWhiteTurn;
        clone.CurrentState = CurrentState;

        for(int i=0; i<8; i++)
        {
            for(int j=0; j<8; j++)
            {
                if(Board[i,j] != null) clone.Board[i,j] = Board[i,j]!.Clone();
            }
        }

        if(LastMovedPiece != null) clone.LastMovedPiece = LastMovedPiece.Clone();

        return clone;
    }

}