using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Chessman
{
    public Pawn()=>value = 10;
    
    public override bool[,] PossibleMoves()
    {
        bool[,] moves = new bool[8, 8];
        int x = currentX;
        int y = currentY;

        Chessman leftChessman = null;
        Chessman rightChessman = null;
        Chessman forwardChessman = null;

        int[] EnPassant = BoardManager.Instance.EnPassant;

        if (isWhite)
        {
            if(y > 0)
            {
                if (x > 0) leftChessman = BoardManager.Instance.Chessmans[x - 1, y - 1];
                if (x < 7) rightChessman = BoardManager.Instance.Chessmans[x + 1, y - 1];
                forwardChessman = BoardManager.Instance.Chessmans[x, y - 1];
            }
            if (forwardChessman == null) 
            {
                if(!this.KingInDanger(x, y - 1))
                    moves[x, y - 1] = true;
            }
            if(leftChessman != null && !leftChessman.isWhite)
            {
                if(!this.KingInDanger(x - 1, y - 1))
                    moves[x - 1, y - 1] = true;
            }
            else if(leftChessman == null && EnPassant[1] == y - 1 &&  EnPassant[0] == x - 1)
            {
                if(!this.KingInDanger(x - 1, y - 1))
                    moves[x - 1, y - 1] = true;
            }
            if(rightChessman != null && !rightChessman.isWhite)
            {
                if(!this.KingInDanger(x + 1, y - 1))
                    moves[x + 1, y - 1] = true;
            }
            else if (rightChessman == null && EnPassant[1] == y - 1 && EnPassant[0] == x + 1)
            {
                if(!this.KingInDanger(x + 1, y - 1))
                    moves[x + 1, y - 1] = true;
            }
            if (y == 6 && forwardChessman == null && BoardManager.Instance.Chessmans[x, y - 2] == null)
            {
                if(!this.KingInDanger(x, y - 2))
                    moves[x, y - 2] = true;
            }
        }
        else
        {
            if (y < 7)
            {
                if (x > 0) leftChessman = BoardManager.Instance.Chessmans[x - 1, y + 1];
                if (x < 7) rightChessman = BoardManager.Instance.Chessmans[x + 1, y + 1];
                forwardChessman = BoardManager.Instance.Chessmans[x, y + 1];
            }
            if (forwardChessman == null)
            {
                if(!this.KingInDanger(x, y + 1))
                    moves[x, y + 1] = true;
            }
            if (leftChessman != null && leftChessman.isWhite)
            {
                if(!this.KingInDanger(x - 1, y + 1))
                    moves[x - 1, y + 1] = true;
            }
            else if (leftChessman == null && EnPassant[1] == y + 1 && EnPassant[0] == x - 1)
            {
                if(!this.KingInDanger(x - 1, y + 1))
                    moves[x - 1, y + 1] = true;
            }
            if (rightChessman != null && rightChessman.isWhite)
            {
                if(!this.KingInDanger(x + 1, y + 1))
                    moves[x + 1, y + 1] = true;
            }
            else if (rightChessman == null && EnPassant[1] == y + 1 && EnPassant[0] == x + 1)
            {
                if(!this.KingInDanger(x + 1, y + 1))
                    moves[x + 1, y + 1] = true;
            }
            if (y == 1 && forwardChessman == null && BoardManager.Instance.Chessmans[x, y + 2] == null)
            {
                if(!this.KingInDanger(x, y + 2))
                    moves[x, y + 2] = true;
            }
        }

        return moves;
    }
}
