
using System;

public class Events
{
    public static Action GameFinish;
    //BoardHighlights
    public static Action<int, int> SetTileYellow;
    public static Action<int, int> SetTileCheck;
    public static Action<bool[,], bool> HighlightPossibleMoves;
    public static Action< bool> HighlightCheckmate;
    public static Action DisableAllHighlights;
    //ChessAI
    public static Action NPCMove;
    //BoardManager
    public static Action<int, int> MoveChessman;
}
