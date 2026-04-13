namespace Chess;

public readonly record struct Position
{
    public int Row { get; }
    public int Col { get; }

    public Position(int row, int col)
    {
        this.Row = row;
        this.Col = col;
    }
}