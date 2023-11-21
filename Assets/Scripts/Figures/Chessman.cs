using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chessman : MonoBehaviour
{
    public int currentX { set; get; }
    public int currentY { set; get; }
    public bool isWhite;
    public int value;
    public bool isMoved = false;

    public Chessman Clone()
    {
       return (Chessman) this.MemberwiseClone();
    }

    public void SetPosition(int x, int y)
    {
        currentX = x;
        currentY = y;
    }

    public virtual bool[,] PossibleMoves()
    {
        bool[,] arr = new bool[8,8];
        for(int i=0; i<8; i++)
        {
            for(int j=0; j<8; j++)
            {
                arr[i, j] = false;
            }
        }
        return arr;
    }

    public bool InDanger()
    {
        Chessman piece = null;

        int x = currentX;
        int y = currentY;
        if(y - 1 >= 0)
        {
            piece = BoardManager.Instance.Chessmans[x, y - 1];
            if(piece != null && piece.isWhite != isWhite &&  piece.GetType() == typeof(King))
                return true;
        }
        while (y-- > 0)
        {
            piece = BoardManager.Instance.Chessmans[x, y];
            if (piece == null)
                continue;
            else if (piece.isWhite == isWhite)
                break;
            if(piece.GetType() == typeof(Rook) || piece.GetType() == typeof(Queen))
            {
                return true;
            }

            break;
        }

        x = currentX;
        y = currentY;
        if(x + 1 <= 7)
        {
            piece = BoardManager.Instance.Chessmans[x + 1, y];
            if(piece != null && piece.isWhite != isWhite &&  piece.GetType() == typeof(King))
                return true;
        }
        while (x++ < 7)
        {
            piece = BoardManager.Instance.Chessmans[x, y];
            if (piece == null)
                continue;  
            else if (piece.isWhite == isWhite)
                break;
            if(piece.GetType() == typeof(Rook) || piece.GetType() == typeof(Queen))
            {
                return true;
            }

            break;
        }

        x = currentX;
        y = currentY;
        if(x - 1 >= 0)
        {
            piece = BoardManager.Instance.Chessmans[x - 1, y];
            if(piece != null && piece.isWhite != isWhite &&  piece.GetType() == typeof(King))
                return true;
        }
        while (x-- > 0)
        {
            piece = BoardManager.Instance.Chessmans[x, y];
            if (piece == null)
                continue;
            else if (piece.isWhite == isWhite)
                break;
            if(piece.GetType() == typeof(Rook) || piece.GetType() == typeof(Queen))
            {
                return true;
            }

            break;
        }

        x = currentX;
        y = currentY;
        if(y + 1 <= 7)
        {
            piece = BoardManager.Instance.Chessmans[x, y + 1];
            if(piece != null && piece.isWhite != isWhite &&  piece.GetType() == typeof(King))
                return true;
        }
        while (y++ < 7)
        {
            piece = BoardManager.Instance.Chessmans[x, y];
            if (piece == null)
                continue;
            else if (piece.isWhite == isWhite)
                break;
            if(piece.GetType() == typeof(Rook) || piece.GetType() == typeof(Queen))
            {
                return true;
            }

            break;
        }

        x = currentX;
        y = currentY;
        if(x + 1 <= 7 && y - 1 >= 0 && isWhite)
        {
            piece = BoardManager.Instance.Chessmans[x + 1, y - 1];
            if(piece != null && piece.isWhite != isWhite && piece.GetType() == typeof(Pawn))
                return true;
        }
        if(x + 1 <= 7 && y - 1 >= 0)
        {
            piece = BoardManager.Instance.Chessmans[x + 1, y - 1];
            if(piece != null && piece.isWhite != isWhite &&  piece.GetType() == typeof(King))
                return true;
        }
        while (x++ < 7 && y-- > 0)
        {
            piece = BoardManager.Instance.Chessmans[x, y];
            if (piece == null)
                continue;
            else if (piece.isWhite == isWhite)
                break;
            if(piece.GetType() == typeof(Bishup) || piece.GetType() == typeof(Queen))
            {
                return true;
            }

            break;
        }

        x = currentX;
        y = currentY;
        if(x + 1 <= 7 && y + 1 <= 7 && !isWhite)
        {
            piece = BoardManager.Instance.Chessmans[x + 1, y + 1];
            if(piece != null && piece.isWhite != isWhite && piece.GetType() == typeof(Pawn))
                return true;
        }
        if(x + 1 <= 7 && y + 1 <= 7)
        {
            piece = BoardManager.Instance.Chessmans[x + 1, y + 1];
            if(piece != null && piece.isWhite != isWhite &&  piece.GetType() == typeof(King))
                return true;
        }
        while (x++ < 7 && y++ < 7)
        {
            piece = BoardManager.Instance.Chessmans[x, y];
            if (piece == null)
                continue;
            else if (piece.isWhite == isWhite)
                break;
            if(piece.GetType() == typeof(Bishup) || piece.GetType() == typeof(Queen))
            {

                return true;
            }

            break;
        }

        x = currentX;
        y = currentY;
        if(x - 1 >= 0 && y - 1 >= 0 && isWhite)
        {
            piece = BoardManager.Instance.Chessmans[x - 1, y - 1];
            if(piece != null && piece.isWhite != isWhite && piece.GetType() == typeof(Pawn))
                return true;
        }
        if(x - 1 >= 0 && y - 1 >= 0)
        {
            piece = BoardManager.Instance.Chessmans[x - 1, y - 1];
            if(piece != null && piece.isWhite != isWhite &&  piece.GetType() == typeof(King))
                return true;
        }
        while (x-- > 0 && y-- > 0)
        {
            piece = BoardManager.Instance.Chessmans[x, y];
            if (piece == null)
                continue;
            else if (piece.isWhite == isWhite)
                break;
            if(piece.GetType() == typeof(Bishup) || piece.GetType() == typeof(Queen))
            {
                return true;
            }

            break;
        }

        x = currentX;
        y = currentY;
        if(x - 1 >= 0 && y + 1 <= 7 && !isWhite)
        {
            piece = BoardManager.Instance.Chessmans[x - 1, y + 1];
            if(piece != null && piece.isWhite != isWhite && piece.GetType() == typeof(Pawn))
                return true;
        }
        if(x - 1 >= 0 && y + 1 <= 7)
        {
            piece = BoardManager.Instance.Chessmans[x - 1, y + 1];
            if(piece != null && piece.isWhite != isWhite &&  piece.GetType() == typeof(King))
                return true;
        }
        while (x-- > 0 && y++ < 7)
        {
            piece = BoardManager.Instance.Chessmans[x, y];
            if (piece == null)
                continue;
            else if (piece.isWhite == isWhite)
                break;
            if(piece.GetType() == typeof(Bishup) || piece.GetType() == typeof(Queen))return true;

            break;
        }

        x = currentX;
        y = currentY;
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                if (Mathf.Abs(i) != Mathf.Abs(j) && i != 0 && j != 0)
                {
                    if (KnightThreat(x - i, y - j))
                        return true;
                }
            }
        }

        return false;
    }

    public bool KnightThreat(int x, int y)
    {
        if (x >= 0 && y >= 0 && x <= 7 && y <= 7)
        {
            Chessman piece = BoardManager.Instance.Chessmans[x, y];
            if (piece == null)
                return false;
            if (piece.isWhite == isWhite)
                return false;
            if(piece.GetType() == typeof(Knight))return true;  
        }

        return false;
    }

    public bool KingInDanger(int x, int y)
    {
        Chessman tmpChessman = BoardManager.Instance.Chessmans[x, y];
        int tmpCurrentX = currentX;
        int tmpCurrentY = currentY;
        BoardManager.Instance.Chessmans[currentX, currentY] = null;
        BoardManager.Instance.Chessmans[x, y] = this;
        this.SetPosition(x, y);
        bool result = false;
        if(isWhite)
            result = BoardManager.Instance.WhiteKing.InDanger();
        else
            result = BoardManager.Instance.BlackKing.InDanger();
        this.SetPosition(tmpCurrentX, tmpCurrentY);
        BoardManager.Instance.Chessmans[tmpCurrentX, tmpCurrentY] = this;
        BoardManager.Instance.Chessmans[x, y] = tmpChessman;
        return result;
    }
}
