namespace Chess.Logic;

public static class UciConverter
{
    public static Move UciToMove(string uci)
    {
        var from = new Position(8 - (uci[1] - '0'), uci[0] - 'a');
        var to   = new Position(8 - (uci[3] - '0'), uci[2] - 'a');
        return new Move(from, to);
    }

    public static string MoveToUci(Move move)
    {
        var fromCol = (char)('a' + move.From.Col);
        var fromRow = (8 - move.From.Row).ToString();
        var toCol   = (char)('a' + move.To.Col);
        var toRow   = (8 - move.To.Row).ToString();

        return $"{fromCol}{fromRow}{toCol}{toRow}";
    }
}