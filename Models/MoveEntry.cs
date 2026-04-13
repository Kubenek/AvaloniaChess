using Chess.Pieces;

namespace Chess;

public class MoveEntry
{
    public string    move   { get; set; }
    public string    player { get; set; }

    public Move Move { get; set; }

    public Piece?[,] board { get; set; }

    public MoveEntry(string _move, string _player, Piece?[,] _board, Move _Move)
    {
        move   = _move;
        player = _player;
        board = _board;

        Move = _Move;
    }
}