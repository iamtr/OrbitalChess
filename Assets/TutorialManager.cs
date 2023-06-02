using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Piece[] movementLoadout;

    [SerializeField] private TMP_Text tutorialText;

    [SerializeField] private TextAsset introFile;
    [SerializeField] private TextAsset pieceMovementFile;

    private string[][] Lines;

    private int FileIndex = 0;
    private int LineIndex = 0;

    private bool loadTextOnce = true;
    
    private void Start()
    {
        ReadAndStoreFiles(introFile, pieceMovementFile);
    }

    private void Update()
    {
        if (!loadTextOnce) return;
        tutorialText.text = Lines[FileIndex][LineIndex];
        loadTextOnce = false;
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

    public void LoadoutPieces(Piece[] pieces)
    {

    }
}
