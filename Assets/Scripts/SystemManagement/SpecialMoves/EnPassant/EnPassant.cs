using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnPassant : MonoBehaviour
{
    [SerializeField] private Timer[] timers;
    public Timer Timer;

    private int id;
    private int numOfPawns = 16;

    private BoardController bc;

    //Singleton
    public static EnPassant i { get; private set; }

    private void Start()
    {
        bc = GameObject.Find("Board").GetComponent<BoardController>();

        if (i != null && i != this) Destroy(this);
        else i = this;
    }

    public Timer InstantiateTimer()
    {
        if (id == numOfPawns)
        {
            id = 0;
        }
        Timer timer = Instantiate(Timer);
        timers[id] = timer;
        id += 1;
        return timer;
    }

    public void InvokeEveryTimer()
    {
        foreach (Timer timer in timers)
        {
            timer.InvokeTimer();
        }
    }

    public bool CheckEnPassant(Piece piece)
    {
        if (piece is Pawn)
        {
            Pawn pawn = (Pawn)piece;
            if (pawn.getTimer().IsJustMoved())
            {
                return true;
            }
        }
        return false;
    }

    public void SetHighlightEnPassant(int x, int y)
    {
        int pos = bc.ConvertToPos(x, y);
        bc.SetHighlightColor(pos, Color.yellow);
    }

    public void MoveEnPassantPiece(int x, int y, Piece piece)
    {
        int newPos = bc.ConvertToPos(x, y);
        int oldPos = piece.CurrPos;
        int enemyPos = bc.ConvertToPos(x, bc.ConvertToXY(oldPos)[1]);
        Debug.Log(enemyPos);

        piece.InvokeOnBeforeMove();
        piece.SetCoords(x, y);
        bc.DestroyOpponentPiece(piece, enemyPos);
        bc.SetPiecePos(piece, newPos);
        bc.DestroyPiece(oldPos);
        piece.InvokeOnAfterMove();
    }

}
