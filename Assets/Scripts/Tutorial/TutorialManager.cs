using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TMP_Text tutorialText;

    [SerializeField] private TextAsset[] tutorialFiles;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button tryAgainButton;

    private string[][] Lines;

    private int FileIndex = 0;
    private int LineIndex = 0;

    protected BoardController bc;

    // Index of interactive tutorial that we are on
    public int tutorialIndex = 0;

	public List<Condition> conditions;

	private void Start()
    {
        ReadAndStoreFiles(tutorialFiles);
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
        conditions[tutorialIndex].ResetNumberOfMoves();
        GameController.SetGameState(GameState.Play);
        bc.LoadPositionPresets(conditions[tutorialIndex].config);
        HideButtons();
        TutorialGameController.EnableCondition(conditions[tutorialIndex]);
    }

	public void CheckCondition()
	{
		Piece p = bc.GetPieceFromPos(conditions[tutorialIndex].position);
        
        if (IsConditionFulfill(p, conditions[tutorialIndex].pieces) && conditions[tutorialIndex].numberOfMovesLeft >= 0)
		{
            TutorialGameController.DisableCondition(conditions[tutorialIndex]);
            GameController.SetGameState(GameState.GameOver);
            tutorialIndex++;
			ShowButtons();
			TriggerNextLine();
            return;
		}
        if (conditions[tutorialIndex].numberOfMovesLeft == 0)
        {
            TutorialGameController.DisableCondition(conditions[tutorialIndex]);
            GameController.SetGameState(GameState.GameOver);
            Debug.Log("Unable to complete the tutorial in the required amount of moves!");
            tryAgainButton.gameObject.SetActive(true);
            conditions[tutorialIndex].ResetNumberOfMoves();
            return;
        }
    }

    public bool IsConditionFulfill(Piece p, List<Piece> pieces)
    {
        foreach(Piece piece in pieces){
            if (p?.GetType() == piece.GetType() && p?.Player == piece.Player)
            {
                return true;
            }
        }
        return false;
    }

    public static void SetPawnHasMoved(Piece piece)
    {
        if(piece?.GetType() != typeof(Pawn))
        {
            Debug.Log("Piece set is not a pawn!");
            return;
        }
        Pawn pawn = (Pawn)piece;
        pawn.SetPawnBoolean();
    }

    public static void SetPawnTwoStep(Piece piece)
    {
        if (piece?.GetType() != typeof(Pawn))
        {
            Debug.Log("Piece set is not a pawn!");
            return;
        }
        Pawn pawn = (Pawn)piece;
        pawn.TwoStep = true;
    }

    public void TryAgainOnClick()
    {
        TriggerTutorial();
        tryAgainButton.gameObject.SetActive(false);
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
