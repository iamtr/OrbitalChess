//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class EnPassant : MonoBehaviour
//{
//    [SerializeField] private TurnCountdown[] turnCountdowns;
//    [SerializeField] private TurnCountdown TurnCountdown;

//    private int id;
//    private int numOfPawns = 16;

//    private BoardController BoardController.i;
//    private Transform TurnCountdownTransform;

//    //Singleton
//    public static EnPassant i { get; private set; }

//    private void Start()
//    {
//        TurnCountdownTransform = GameObject.Find("TurnCountdowns").transform;

//        if (i != null && i != this) Destroy(this);
//        else i = this;
//    }

//    public TurnCountdown InstantiateTurnCountdown()
//    {
//        if (id == numOfPawns)
//        {
//            id = 0;
//        }
//        TurnCountdown turnCountdown = Instantiate(TurnCountdown);
//        turnCountdowns[id] = turnCountdown;
//        turnCountdowns[id].transform.parent = TurnCountdownTransform;
//        turnCountdowns[id].gameObject.SetActive(false);
//        id += 1;
//        return turnCountdown;
//    }

//    public void InvokeEveryTimer()
//    {
//        foreach (TurnCountdown timer in turnCountdowns)
//        {
//            timer.InvokeTimer();
//        }
//    }

//    public bool CheckEnPassant(Piece piece)
//    {
//        if (piece is Pawn)
//        {
//            Pawn pawn = (Pawn)piece;
//            if (pawn.getTurnCountdown().IsJustMoved() && pawn.GetTwoStepBool())
//            {
//                return true;
//            }
//        }
//        return false;
//    }

//    public void SetHighlightEnPassant(int x, int y)
//    {
//        int pos = BoardController.i.i.ConvPos(x, y);
//        BoardController.i.SetHighlightColor(pos, Color.yellow);
//    }

//    public void MoveEnPassantPiece(int x, int y, Piece piece)
//    {
//        int newPos = BoardController.i.i.ConvPos(x, y);
//        int oldPos = piece.CurrPos;
//        int enemyPos = BoardController.i.i.ConvPos(x, BoardController.i.ConvXY(oldPos)[1]);

//        piece.InvokeOnBeforeMove();
//        piece.SetCoords(x, y);
//        BoardController.i.DestroyOpponentPiece(piece, enemyPos);
//        BoardController.i.SetPiecePos(piece, newPos);
//        BoardController.i.SetPieceNull(oldPos);
//        piece.InvokeOnAfterMove();
//    }
//}
