using Chess.Pieces;

namespace Chess;

public class MoveEntry
{
    public string    move   { get; set; }
    public string    player { get; set; }

    public (int, int) fromPos { get; set; }
    public (int, int) toPos   { get; set; }

    public Piece?[,] board { get; set; }

    public MoveEntry(string _move, string _player, Piece?[,] _board, (int, int) _fromPos, (int, int) _toPos)
    {
        move   = _move;
        player = _player;
        board = _board;

        fromPos = _fromPos;
        toPos   = _toPos;
    }
}