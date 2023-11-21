using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { set; get; }

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = TILE_SIZE / 2;

    private Camera cam;

    public List<GameObject> ChessmanPrefabs;

    private List<GameObject> ActiveChessmans;

    public Chessman[,] Chessmans{ set; get; }

    public Chessman SelectedChessman;

    public Chessman WhiteKing;
    public Chessman BlackKing;
    public Chessman WhiteRook1;
    public Chessman WhiteRook2;
    public Chessman BlackRook1;
    public Chessman BlackRook2;

    public bool[,] allowedMoves;
    public int[] EnPassant { set; get; }

    private int selectionX = -1;
    private int selectionY = -1;

    public bool isWhiteTurn = true;

    private void Start()
    {
        Instance = this;
        cam = FindObjectOfType<Camera>();
        ActiveChessmans = new List<GameObject>();
        Chessmans = new Chessman[8, 8];
        EnPassant = new int[2] { -1, -1 };
        SpawnAllChessmans();

        Events.MoveChessman += MoveChessman;
    }
    private void OnDestroy()
    {
        Events.MoveChessman -= MoveChessman;
    }
    private void Update()
    {
        UpdateSelection();
        DrawChessBoard();
        if(Input.GetMouseButtonDown(0) && isWhiteTurn)
        {
            if (selectionX >= 0 && selectionY >= 0 && selectionX <= 7 && selectionY <= 7)
            {
                if (SelectedChessman == null)
                {
                    SelectChessman();
                }
                else
                {
                    MoveChessman(selectionX, selectionY);
                }
            }
        }
        else if(!isWhiteTurn)
        {
            Events.NPCMove?.Invoke();
        }
        
    }

    private void SelectChessman()
    {
        if (Chessmans[selectionX, selectionY] == null) return;
        if (Chessmans[selectionX, selectionY].isWhite != isWhiteTurn) return;
        SelectedChessman = Chessmans[selectionX, selectionY];
        Events.SetTileYellow?.Invoke(selectionX, selectionY);
        allowedMoves = SelectedChessman.PossibleMoves();
        Events.HighlightPossibleMoves?.Invoke(allowedMoves, isWhiteTurn);
    }

    public void MoveChessman(int x, int y)
    {
        if(allowedMoves[x,y])
        {
            Chessman opponent = Chessmans[x, y];

            if(opponent != null)
            {
                ActiveChessmans.Remove(opponent.gameObject);
                Destroy(opponent.gameObject);

            }
            if (EnPassant[0] == x && EnPassant[1] == y && SelectedChessman.GetType() == typeof(Pawn))
            {
                if(isWhiteTurn)
                    opponent = Chessmans[x, y + 1];
                else
                    opponent = Chessmans[x, y - 1];

                ActiveChessmans.Remove(opponent.gameObject);
                Destroy(opponent.gameObject);

            }
            EnPassant[0] = EnPassant[1] = -1;
            if(SelectedChessman.GetType() == typeof(Pawn))
            {
                if (y == 7)
                {
                    ActiveChessmans.Remove(SelectedChessman.gameObject);
                    Destroy(SelectedChessman.gameObject);
                    SpawnChessman(10, new Vector3(x, 0, y));
                    SelectedChessman = Chessmans[x, y];
                }
                if (y == 0)
                {
                    ActiveChessmans.Remove(SelectedChessman.gameObject);
                    Destroy(SelectedChessman.gameObject);
                    SpawnChessman(4, new Vector3(x, 0, y));
                    SelectedChessman = Chessmans[x, y];
                }
                if (SelectedChessman.currentY == 1 && y == 3)
                {
                    EnPassant[0] = x;
                    EnPassant[1] = y - 1;
                }
                if (SelectedChessman.currentY == 6 && y == 4)
                {
                    EnPassant[0] = x;
                    EnPassant[1] = y + 1;
                }
            }
            if(SelectedChessman.GetType() == typeof(King) && System.Math.Abs(x - SelectedChessman.currentX) == 2)
            {
                if(x - SelectedChessman.currentX < 0)
                {
                    Chessmans[x + 1, y] = Chessmans[x - 1, y];
                    Chessmans[x - 1, y] = null;
                    Chessmans[x + 1, y].SetPosition(x + 1, y);
                    Chessmans[x + 1, y].transform.position = new Vector3(x + 1, 0, y);
                    Chessmans[x + 1, y].isMoved = true;
                }
                else
                {
                    Chessmans[x - 1, y] = Chessmans[x + 2, y];
                    Chessmans[x + 2, y] = null;
                    Chessmans[x - 1, y].SetPosition(x - 1, y);
                    Chessmans[x - 1, y].transform.position = new Vector3(x - 1, 0, y);
                    Chessmans[x - 1, y].isMoved = true;
                }
            }
            Chessmans[SelectedChessman.currentX, SelectedChessman.currentY] = null;
            Chessmans[x, y] = SelectedChessman;
            SelectedChessman.SetPosition(x, y);
            SelectedChessman.transform.position = new Vector3(x, 0, y);
            SelectedChessman.isMoved = true;
            isWhiteTurn = !isWhiteTurn;
        }
        SelectedChessman = null;
        Events.DisableAllHighlights?.Invoke();
        if(isWhiteTurn)
        {
            if(WhiteKing.InDanger())
                Events.SetTileCheck?.Invoke(WhiteKing.currentX, WhiteKing.currentY);
        }
        else
        {
            if(BlackKing.InDanger())
                Events.SetTileCheck?.Invoke(BlackKing.currentX, BlackKing.currentY);
        }
        isCheckmate();
    }

    private void UpdateSelection()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 25.0f, LayerMask.GetMask("ChessPlane")))
        {
            selectionX = (int)(hit.point.x + 0.5f);
            selectionY = (int)(hit.point.z + 0.5f);
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }

    private void DrawChessBoard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;
        Vector3 offset = new Vector3(0.5f, 0f, 0.5f);
        for (int i=0; i<=8; i++)
        {
            Vector3 start = Vector3.forward * i - offset;
            Debug.DrawLine(start, start + widthLine);
            for(int j=0; j<=8; j++)
            {
                start = Vector3.right * i - offset;
                Debug.DrawLine(start, start + heightLine);
            }
        }
        

        // Draw Selection
        if(selectionX >= 0 && selectionY >= 0 && selectionX <= 7 && selectionY <= 7)
        {
            Debug.DrawLine(
                Vector3.forward * selectionY + Vector3.right * selectionX - offset,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1) - offset
                );
            Debug.DrawLine(
                Vector3.forward * (selectionY + 1) + Vector3.right * selectionX - offset,
                Vector3.forward * selectionY + Vector3.right * (selectionX + 1) - offset
                );
        }
    }

    private void SpawnChessman(int index, Vector3 position)
    {
        GameObject ChessmanObject = Instantiate(ChessmanPrefabs[index], position, ChessmanPrefabs[index].transform.rotation) as GameObject;
        ChessmanObject.transform.SetParent(this.transform);
        ActiveChessmans.Add(ChessmanObject);

        int x = (int)(position.x);
        int y = (int)(position.z);
        Chessmans[x, y] = ChessmanObject.GetComponent<Chessman>();
        Chessmans[x, y].SetPosition(x, y);

    }
    
    private void SpawnAllChessmans()
    {
        // Spawn White Pieces
        for (int i = 0; i < 8; i++)
        {
            if (i > 4) SpawnChessman(7-i, new Vector3(i, 0, 7));
            else SpawnChessman(i, new Vector3(i, 0, 7));
        }
        for(int i=0; i<8; i++)
        {
            SpawnChessman(5, new Vector3(i, 0, 6));
        }

        // Spawn Black Pieces
        for (int i = 0; i < 8; i++)
        {
            if (i > 4) SpawnChessman(6 + (7-i), new Vector3(i, 0, 0));
            else SpawnChessman(6+i, new Vector3(i, 0, 0));
        }
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(11, new Vector3(i, 0, 1));
        }

        WhiteKing = Chessmans[3, 7];
        BlackKing = Chessmans[3, 0];

        WhiteRook1 = Chessmans[0, 7];
        WhiteRook2 = Chessmans[7, 7];
        BlackRook1 = Chessmans[0, 0];
        BlackRook2 = Chessmans[7, 0];
    }



    private void isCheckmate()
    {
        bool hasAllowedMove = false;
        foreach(GameObject chessman in ActiveChessmans)
        {
            if(chessman.GetComponent<Chessman>().isWhite != isWhiteTurn)
                continue;

            bool[,] allowedMoves = chessman.GetComponent<Chessman>().PossibleMoves();

            for(int x=0; x<8; x++)
            {
                for(int y=0; y<8; y++)
                {
                    if(allowedMoves[x, y])
                    {
                        hasAllowedMove = true;
                        break;
                    }
                }
                if(hasAllowedMove) break;
            }
        }

        if(!hasAllowedMove) 
        {
            Events.HighlightCheckmate?.Invoke(isWhiteTurn);
            Events.GameFinish?.Invoke();
        }
    }
}
