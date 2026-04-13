
namespace Chess.Logic;

public static class Directions
{
    public static readonly (int, int)[] Diagonals =
    {
        (-1, -1), (-1, 1), (1, -1), (1, 1)
    };

    public static readonly (int, int)[] Straight =
    {
        (-1, 0), (1, 0), (0, -1), (0, 1)
    };

    public static readonly (int, int)[] All =
    {
        (-1, -1), (-1, 1), (1, -1), (1, 1),
        (-1, 0), (1, 0), (0, -1), (0, 1)
    };

    public static readonly (int, int)[] LShaped =
    {
        (-2, -1), (-2, 1), (-1, 2), (1, 2),   
        (-1, -2), (1, -2), (2, -1), (2, 1)     
    };
}