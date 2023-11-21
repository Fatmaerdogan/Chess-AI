using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessAI : MonoBehaviour
{
    private List<Chessman> ActiveChessmans;
    private Chessman[,] Chessmans;
    private int[] EnPassant;
    private Chessman[,] ActualChessmansReference;
    private Chessman ActualWhiteKing;
    private Chessman ActualBlackKing;
    private Chessman ActualWhiteRook1;
    private Chessman ActualWhiteRook2;
    private Chessman ActualBlackRook1;
    private Chessman ActualBlackRook2;
    private int[] ActualEnPassant;
    private Stack< State> History;
    private int maxDepth;
    private Chessman NPCSelectedChessman = null;
    private int moveX = -1;
    private int moveY = -1;
    private int winningValue = 0;
    private long totalTime = 0;
    private long totalRun = 0;
    public  long averageResponseTime = 0;

    string detail, board;

    private void Start()
    {
        Events.NPCMove += NPCMove;
    }

    private void OnDestroy()
    {
        Events.NPCMove -= NPCMove;
    }
    public void NPCMove()
    {
        System.Diagnostics.Stopwatch stopwatch  = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        History = new Stack< State>();
        ActualChessmansReference = BoardManager.Instance.Chessmans;
        ActualWhiteKing = BoardManager.Instance.WhiteKing;
        ActualBlackKing = BoardManager.Instance.BlackKing;
        ActualWhiteRook1 = BoardManager.Instance.WhiteRook1;
        ActualWhiteRook2 = BoardManager.Instance.WhiteRook2;
        ActualBlackRook1 = BoardManager.Instance.BlackRook1;
        ActualBlackRook2 = BoardManager.Instance.BlackRook2;
        ActualEnPassant = BoardManager.Instance.EnPassant;

        ActiveChessmans = new List<Chessman>();
        Chessmans = new Chessman[8, 8];

        for(int x=0; x<8; x++)
            for(int y=0; y<8; y++)
            {
                if(ActualChessmansReference[x, y] != null)
                {
                    Chessman currChessman = ActualChessmansReference[x, y].Clone();
                    ActiveChessmans.Add(currChessman);
                    Chessmans[x, y] = currChessman;
                }
                else
                {
                    Chessmans[x, y] = null;
                }
            }

        Shuffle(ActiveChessmans);

        EnPassant = new int[2]{ActualEnPassant[0], ActualEnPassant[0]};
        BoardManager.Instance.Chessmans = Chessmans;
        BoardManager.Instance.WhiteKing = Chessmans[ActualWhiteKing.currentX, ActualWhiteKing.currentY];
        BoardManager.Instance.BlackKing = Chessmans[ActualBlackKing.currentX, ActualBlackKing.currentY];
        if(ActualWhiteRook1 != null) BoardManager.Instance.WhiteRook1 = Chessmans[ActualWhiteRook1.currentX, ActualWhiteRook1.currentY];
        if(ActualWhiteRook2 != null) BoardManager.Instance.WhiteRook2 = Chessmans[ActualWhiteRook2.currentX, ActualWhiteRook2.currentY];
        if(ActualBlackRook1 != null) BoardManager.Instance.BlackRook1 = Chessmans[ActualBlackRook1.currentX, ActualBlackRook1.currentY];
        if(ActualBlackRook2 != null) BoardManager.Instance.BlackRook2 = Chessmans[ActualBlackRook2.currentX, ActualBlackRook2.currentY];
        BoardManager.Instance.EnPassant = EnPassant;
        Think();
        BoardManager.Instance.Chessmans = ActualChessmansReference;
        BoardManager.Instance.WhiteKing = ActualWhiteKing;
        BoardManager.Instance.BlackKing = ActualBlackKing;
        BoardManager.Instance.WhiteRook1 = ActualWhiteRook1;
        BoardManager.Instance.WhiteRook2 = ActualWhiteRook2;
        BoardManager.Instance.BlackRook1 = ActualBlackRook1;
        BoardManager.Instance.BlackRook2 = ActualBlackRook2;
        BoardManager.Instance.EnPassant = ActualEnPassant;
        BoardManager.Instance.SelectedChessman = BoardManager.Instance.Chessmans[NPCSelectedChessman.currentX, NPCSelectedChessman.currentY];
        BoardManager.Instance.allowedMoves = BoardManager.Instance.SelectedChessman.PossibleMoves();
        Events.MoveChessman?.Invoke(moveX, moveY);
        stopwatch.Stop();
        totalTime += stopwatch.ElapsedMilliseconds;
        totalRun++;
        averageResponseTime = totalTime / totalRun;
    }

    private void Think()
    {
        maxDepth = 5;
        int depth = maxDepth-1;
        winningValue = AlphaBeta(depth, true, System.Int32.MinValue, System.Int32.MaxValue);
    }

    private int MiniMax(int depth, bool isMax)
    {
        if(depth == 0 || isGameOver())
        {
            int value = StaticEvaluationFunction();
            
            return value;
        }
        if(isMax)
        {
            int hValue = System.Int32.MinValue;
            foreach(Chessman chessman in ActiveChessmans.ToArray())
            {
                if(chessman.isWhite) continue;

                bool[,] allowedMoves = chessman.PossibleMoves();
                for(int x=0; x<8; x++)
                {
                    for(int y=0; y<8; y++)
                    {
                        if(allowedMoves[x, y])
                        {
                            Move(chessman, x, y, depth);
                            int thisMoveValue = MiniMax(depth-1, !isMax);

                            if(hValue < thisMoveValue) 
                            {
                                hValue = thisMoveValue;
                                if(depth == maxDepth-1)
                                {
                                    NPCSelectedChessman = chessman;
                                    moveX = x;
                                    moveY = y;
                                }
                            }
                            Undo(depth);
                        }
                    }
                }
            }

            return hValue;
        }
        else
        {
            int hValue = System.Int32.MaxValue;
            foreach(Chessman chessman in ActiveChessmans.ToArray())
            {
                if(!chessman.isWhite) continue;

                bool[,] allowedMoves = chessman.PossibleMoves();
                for(int x=0; x<8; x++)
                {
                    for(int y=0; y<8; y++)
                    {
                        if(allowedMoves[x, y])
                        {
                            Move(chessman, x, y, depth);
                            int thisMoveValue = MiniMax(depth-1, !isMax);

                            if(hValue > thisMoveValue) 
                            {
                                hValue = thisMoveValue;
                            }
                            Undo(depth);
                        }
                    }
                }
            }

            return hValue;
        }
    }

    private int AlphaBeta(int depth, bool isMax, int alpha, int beta)
    {
        if(depth == 0 || isGameOver())
        {
            int value = StaticEvaluationFunction();
            
            return value;
        }
        if(isMax)
        {
            int hValue = System.Int32.MinValue;
            foreach(Chessman chessman in ActiveChessmans.ToArray())
            {
                if(chessman.isWhite) continue;

                bool[,] allowedMoves = chessman.PossibleMoves();
                for(int x=0; x<8; x++)
                {
                    for(int y=0; y<8; y++)
                    {
                        if(allowedMoves[x, y])
                        {
                            Move(chessman, x, y, depth);
                            int thisMoveValue = AlphaBeta(depth-1, !isMax, alpha, beta);
                            Undo(depth);

                            if(hValue < thisMoveValue) 
                            {
                                hValue = thisMoveValue;
                                if(depth == maxDepth-1)
                                {
                                    NPCSelectedChessman = chessman;
                                    moveX = x;
                                    moveY = y;
                                }
                            }

                            if(hValue > alpha) 
                                alpha = hValue;

                            if(beta <= alpha)
                                break;
                        }
                    }

                    if(beta <= alpha)
                        break;
                }

                if(beta <= alpha)
                    break;
            }

            return hValue;
        }
        else
        {
            int hValue = System.Int32.MaxValue;
            foreach(Chessman chessman in ActiveChessmans.ToArray())
            {
                if(!chessman.isWhite) continue;

                bool[,] allowedMoves = chessman.PossibleMoves();
                for(int x=0; x<8; x++)
                {
                    for(int y=0; y<8; y++)
                    {
                        if(allowedMoves[x, y])
                        {
                            Move(chessman, x, y, depth);
                            int thisMoveValue = AlphaBeta(depth-1, !isMax, alpha, beta);
                            Undo(depth);

                            if(hValue > thisMoveValue) 
                            {
                                hValue = thisMoveValue;
                            }

                            if(hValue < beta) 
                                beta = hValue;

                            if(beta <= alpha)
                                break;
                        }
                    }

                    if(beta <= alpha)
                        break;
                }

                if(beta <= alpha)
                    break;
            }

            return hValue;
        }
    }
    private int StaticEvaluationFunction()
    {
        int TotalScore = 0;
        int curr = 0;
        foreach(Chessman chessman in ActiveChessmans)
        {
            if(chessman.GetType() == typeof(King))
                curr = 900;
            if(chessman.GetType() == typeof(Queen))
                curr = 90;
            if(chessman.GetType() == typeof(Rook))
                curr = 50;
            if(chessman.GetType() == typeof(Bishup))
                curr = 30;
            if(chessman.GetType() == typeof(Knight))
                curr = 30;
            if(chessman.GetType() == typeof(Pawn))
                curr = 10;

            if(chessman.isWhite)
                TotalScore -= curr;
            else
                TotalScore += curr;
        }
        return TotalScore;
    }

    private bool isGameOver()
    {
        int currScore = StaticEvaluationFunction();
        if((currScore < -290 ) || (currScore > 290))
            return true;
        return false;
    }

    private void Move(Chessman chessman, int x, int y, int depth)
    {
        (Chessman chessman, (int x, int y) oldPosition, (int x, int y) newPosition, bool isMoved) movedChessman;
        (Chessman chessman, (int x, int y) Position) capturedChessman = (null, (-1, -1));
        (int x, int y) EnPassantStatus;
        (bool wasPromotion, Chessman promotedChessman) PromotionMove = (false, null);
        (bool wasCastling, bool isKingSide) CastlingMove;

        movedChessman.chessman = chessman;
        movedChessman.oldPosition = (chessman.currentX, chessman.currentY);
        movedChessman.newPosition = (x, y);
        movedChessman.isMoved = chessman.isMoved;

        EnPassantStatus = (EnPassant[0], EnPassant[1]);
        Chessman opponent = Chessmans[x, y];
        if(opponent != null)
        {
            capturedChessman.chessman = opponent;
            capturedChessman.Position = (x, y);

            Chessmans[x, y] = null;
            ActiveChessmans.Remove(opponent);
        }
        if (EnPassant[0] == x && EnPassant[1] == y && chessman.GetType() == typeof(Pawn))
        {           
            if(chessman.isWhite)
            {
                opponent = Chessmans[x, y + 1];

                capturedChessman.chessman = opponent;
                capturedChessman.Position = (x, y + 1);
                Chessmans[x, y + 1] = null;
            }
            else
            {
                opponent = Chessmans[x, y - 1];

                capturedChessman.chessman = opponent;
                capturedChessman.Position = (x, y - 1);
                Chessmans[x, y - 1] = null;
            }

            ActiveChessmans.Remove(opponent);
        }
        EnPassant[0] = EnPassant[1] = -1;
        if(chessman.GetType() == typeof(Pawn))
        {
            if (y == 7 || y == 0)
            {
                ActiveChessmans.Remove(chessman);
                Chessmans[x, y] = gameObject.AddComponent<Queen>(); 
                Chessmans[x, y].SetPosition(x, y);
                Chessmans[x, y].isWhite = chessman.isWhite;
                chessman = Chessmans[x, y];
                ActiveChessmans.Add(chessman);

                PromotionMove = (true, chessman);
            }

            if (chessman.currentY == 1 && y == 3)
            {
                EnPassant[0] = x;
                EnPassant[1] = y - 1;
            }
            if (chessman.currentY == 6 && y == 4)
            {
                EnPassant[0] = x;
                EnPassant[1] = y + 1;
            }
        }
        CastlingMove = (false, false);
        if(chessman.GetType() == typeof(King) && System.Math.Abs(x - chessman.currentX) == 2)
        {          
            if(x - chessman.currentX < 0)
            {
                Chessmans[x + 1, y] = Chessmans[x - 1, y];
                Chessmans[x - 1, y] = null;
                Chessmans[x + 1, y].SetPosition(x + 1, y);
                Chessmans[x + 1, y].isMoved = true;

                CastlingMove = (true, true);
            }
            else
            {
                Chessmans[x - 1, y] = Chessmans[x + 2, y];
                Chessmans[x + 2, y] = null;
                Chessmans[x - 1, y].SetPosition(x - 1, y);
                Chessmans[x - 1, y].isMoved = true;

                CastlingMove = (true, false);
            }
        }
        Chessmans[chessman.currentX, chessman.currentY] = null;
        Chessmans[x, y] = chessman;
        chessman.SetPosition(x, y);
        chessman.isMoved = true;
        State currentState = new State();
        currentState.SetState(movedChessman, capturedChessman, EnPassantStatus, PromotionMove, CastlingMove, depth);
        History.Push(currentState);
    }

    private void Undo(int depth)
    {
        State currentState = History.Pop();

        if(depth != currentState.depth)
        {
            Debug.Log("Depth not matched!!!");
            return;
        }
        var movedChessman = currentState.movedChessman;
        var capturedChessman = currentState.capturedChessman;
        var EnPassantStatus = currentState.EnPassantStatus;
        var PromotionMove = currentState.PromotionMove;
        var CastlingMove = currentState.CastlingMove;
        EnPassant[0] = EnPassantStatus.x;
        EnPassant[1] = EnPassantStatus.y;
        Chessman chessman = movedChessman.chessman;
        chessman.isMoved = movedChessman.isMoved;
        chessman.SetPosition(movedChessman.oldPosition.x, movedChessman.oldPosition.y);
        Chessmans[movedChessman.oldPosition.x, movedChessman.oldPosition.y] = chessman;
        Chessmans[movedChessman.newPosition.x, movedChessman.newPosition.y] = null;
        if(PromotionMove.wasPromotion)
        {
            ActiveChessmans.Remove(PromotionMove.promotedChessman);
            ActiveChessmans.Add(chessman);
        }
        var opponent = capturedChessman;
        if(opponent.chessman != null)
        {
            Chessmans[opponent.Position.x, opponent.Position.y] = opponent.chessman;
            opponent.chessman.SetPosition(opponent.Position.x, opponent.Position.y);
            ActiveChessmans.Add(opponent.chessman);
        }
        if(CastlingMove.wasCastling)
        {
            int x = movedChessman.newPosition.x;
            int y = movedChessman.newPosition.y;
            if(CastlingMove.isKingSide)
            {
                Chessmans[x - 1, y] = Chessmans[x + 1, y];
                Chessmans[x + 1, y] = null;
                Chessmans[x - 1, y].SetPosition(x - 1, y);
                Chessmans[x - 1, y].isMoved = false;
            }
            else
            {
                Chessmans[x + 2, y] = Chessmans[x - 1, y];
                Chessmans[x - 1, y] = null;
                Chessmans[x + 2, y].SetPosition(x + 2, y);
                Chessmans[x + 2, y].isMoved = false;
            }
        }
    }

    public void Shuffle(List<Chessman> list)  
    {  
        System.Random rng = new System.Random();

        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            Chessman value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }

}
