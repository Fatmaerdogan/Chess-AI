using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Chessman
{
    public Knight()=>value = 30;

    public override bool[,] PossibleMoves()
    {
        bool[,] moves = new bool[8, 8];
        int x = currentX;
        int y = currentY;
        for(int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                if(Mathf.Abs(i) != Mathf.Abs(j)&& i != 0 && j != 0)
                {
                    KnightMove(x - i, y - j, ref moves);
                }
            }
        }
        return moves;
    }

    private void KnightMove(int x, int y, ref bool[,] moves)
    {
        if (x >= 0 && y >= 0 && x <= 7 && y <= 7)
        {
            Chessman piece = BoardManager.Instance.Chessmans[x, y];
            if (piece == null)
            {
                if(!this.KingInDanger(x, y))
                    moves[x, y] = true;
            }
            else if (piece.isWhite != isWhite)
            {
                if(!this.KingInDanger(x, y))
                    moves[x, y] = true;
            }
        }
    }
}
