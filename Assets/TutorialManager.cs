using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Piece[] movementLoadout;
    [SerializeField] private TextAsset introductionFile;
    [SerializeField] private TextAsset pieceMovementFile;

    private void Start()
    {
        print(introductionFile);
    }

    public void LoadoutPieces(Piece[] pieces)
    {

    }
}
