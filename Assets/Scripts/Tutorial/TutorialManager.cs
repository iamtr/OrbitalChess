using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Piece[] PawnAndKnightPiece;
    [SerializeField] private Piece[] BishopAndRookPiece;
    [SerializeField] private Piece[] QueenAndKingPiece;

    [SerializeField] private TMP_Text tutorialText;

    [SerializeField] private TextAsset introFile;
    [SerializeField] private TextAsset pieceMovementFile;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    private string[][] Lines;

    private int FileIndex = 0;
    private int LineIndex = 0;

    protected BoardController bc;

    // Index of interactive tutorial that we are on
    public int tutorialIndex = 0;

	public List<Condition> conditions;

	private void Start()
    {
        ReadAndStoreFiles(introFile, pieceMovementFile);
        tutorialText.text = Lines[FileIndex][LineIndex];
        bc = FindObjectOfType<BoardController>();
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
    
    /// <summary>
    /// Triggers the previous line
    /// </summary>
    public void TriggerPrevLine()
    {
        // BoardController.i.UnloadCurrentPieces();
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

        if (Lines[FileIndex][LineIndex].Contains("Let's try it out!"))
        {
            tutorialIndex--;
            TriggerTutorial();
        }

        tutorialText.text = Lines[FileIndex][LineIndex];
    }

    /// <summary>
    /// Triggers the next line
    /// </summary>
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
        if (Lines[FileIndex][LineIndex].Contains("Let's try it out!")) TriggerTutorial();
    }

    public void TriggerTutorial()
    {
        GameController.SetGameState(GameState.Play);
        bc.LoadPositionPresets(conditions[tutorialIndex].config);
        HideButtons();
    }

	public void CheckCondition()
	{
		Piece p = bc.GetPieceFromPos(conditions.First().position);
		if (p?.GetType() == conditions[tutorialIndex].piece.GetType() && p?.Player == conditions[tutorialIndex].piece.Player)
		{
			tutorialIndex++;
			ShowButtons();
			TriggerNextLine();
		}
	}

	public void ShowButtons()
    {
        prevButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
    }

	public void HideButtons()
	{
		prevButton.gameObject.SetActive(false);
		nextButton.gameObject.SetActive(false);
	}
}
