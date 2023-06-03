using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Piece[] PawnAndKnightPiece;
    [SerializeField] private Piece[] BishopAndRookPiece;
    [SerializeField] private Piece[] QueenAndKingPiece;

    [SerializeField] private TMP_Text tutorialText;

    [SerializeField] private TextAsset introFile;
    [SerializeField] private TextAsset pieceMovementFile;

    private string[][] Lines;

    private int FileIndex = 0;
    private int LineIndex = 0;
    
    private void Start()
    {
        ReadAndStoreFiles(introFile, pieceMovementFile);
        tutorialText.text = Lines[FileIndex][LineIndex];
        BoardController.i.UnloadCurrentPieces();
    }

    public void ReadAndStoreFiles(params TextAsset[] files)
    {
        Lines = new string[files.Length][];
        var splitFile = new string[] { "\r\n", "\r", "\n" };
        int index = 0;
        foreach (TextAsset file in files)
        {
            Lines[index] = file.text.Split(splitFile, System.StringSplitOptions.RemoveEmptyEntries);
            index++;
        }
    }
    
    public void TriggerPrevLine()
    {
        BoardController.i.UnloadCurrentPieces();
        if (FileIndex == 0 && LineIndex == 0) return;
        if (LineIndex == 0)
        {
            FileIndex--;
            LineIndex = Lines[FileIndex].Length - 1;
        }
        else
        {
            LineIndex--;
        }
        tutorialText.text = Lines[FileIndex][LineIndex];
    }

    public void TriggerNextLine()
    {
        var CurrFile = Lines[FileIndex];
        if (LineIndex == CurrFile.Length - 1)
        {
            if (FileIndex == Lines.Length - 1) return;
            FileIndex++;
            LineIndex = 0;
        } else
        {
            LineIndex++;
        }
        tutorialText.text = Lines[FileIndex][LineIndex];
    }
}
