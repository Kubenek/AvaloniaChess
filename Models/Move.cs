namespace Chess;

public class Move
{
    public Position From { get; set; }
    public Position  To { get; set; }

    public Move(Position _from, Position _to)
    {
        From = _from;
        To = _to;
    }
}