namespace Chess;

public class MoveEntry
{
    public string move   { get; set; }
    public string player { get; set; }

    public MoveEntry(string _move, string _player)
    {
        move   = _move;
        player = _player;
    }
}