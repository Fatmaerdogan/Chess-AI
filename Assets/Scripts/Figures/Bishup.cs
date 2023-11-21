using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishup : Chessman
{
    public Bishup()=>value = 30;
    
    public override bool[,] PossibleMoves()
    {
        bool[,] moves = new bool[8, 8];
        int x = currentX;
        int y = currentY;
        while (x++ < 7 && y-- > 0)
        {
            if (!BishupMove(x, y, ref moves))
                break;
        }

        x = currentX;
        y = currentY;
        while (x++ < 7 && y++ < 7)
        {
            if (!BishupMove(x, y, ref moves))
                break;
        }

        x = currentX;
        y = currentY;
        while (x-- > 0 && y-- > 0)
        {
            if (!BishupMove(x, y, ref moves))
                break;
        }

        x = currentX;
        y = currentY;
        while (x-- > 0 && y++ < 7)
        {
            if (!BishupMove(x, y, ref moves))
                break;
        }

        return moves;
    }

    private bool BishupMove(int x, int y, ref bool[,] moves)
    {
        Chessman piece = BoardManager.Instance.Chessmans[x, y];
        if (piece == null)
        {
            if(!this.KingInDanger(x, y))
                moves[x, y] = true;
            return true; 
        }
        else if (piece.isWhite != isWhite)
        {
            if(!this.KingInDanger(x, y))
                moves[x, y] = true;
        }

        return false;  
    }
}
