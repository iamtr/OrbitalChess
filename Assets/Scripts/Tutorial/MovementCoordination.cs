using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MovementCoordination : MonoBehaviour
{
	[SerializeField] private SetMove[] moves;

	[SerializeField] private TextAsset[] openingsFiles;

	[SerializeField] private TMP_Text openingsText;
	[SerializeField] private TMP_Text tutorialText;

	[SerializeField] private Button prevButton;
	[SerializeField] private Button nextButton;

	private GameObject pieces;

	private string[] titles = {"Italian Game", "Sicilian Defense", "Ruy López Opening" };

	private int FileIndex = 0;
	private int LineIndex = 0;

	private string[][] lines;

	private SetMove currSetMoves;

	private void Awake()
	{
		pieces = GameObject.Find("Pieces");
		ReadAndStoreFiles(openingsFiles);
		currSetMoves = moves[0];
		
		ColorOptionDropdown.FlipBoard(true);
	}

    private void Start()
    {
		prevButton.gameObject.SetActive(false);
		nextButton.gameObject.SetActive(false);
	}

    public void ReadAndStoreFiles(params TextAsset[] files)
	{
		lines = new string[files.Length][];
		var splitFile = new string[] { "\r\n", "\r", "\n" };
		int index = 0;
		foreach (TextAsset file in files)
		{
			lines[index] = file.text.Split(splitFile, System.StringSplitOptions.RemoveEmptyEntries);
			index++;
		}
	}

	public void TriggerPrevLine()
	{
		// BoardController.i.UnloadCurrentPieces();
		LineIndex--;
		LoadText();

		if (LineIndex == 0)
        {
			prevButton.gameObject.SetActive(false);
			return;
		}

		nextButton.gameObject.SetActive(true);

		if (lines[FileIndex][LineIndex].Contains("@"))
		{
			currSetMoves.PreviousMove();
			TriggerPrevLine();
		}
	}

	/// <summary>
	/// Triggers the next line
	/// </summary>
	public void TriggerNextLine()
	{
		LineIndex++;
		LoadText();

		var CurrFile = lines[FileIndex];
		if (LineIndex == CurrFile.Length - 1)
		{
			nextButton.gameObject.SetActive(false);
			return;
		}

		prevButton.gameObject.SetActive(true);

		if (lines[FileIndex][LineIndex].Contains("@"))
		{
			currSetMoves.ExecuteMove();
			TriggerNextLine();
		}
	}

	public void LoadText()
    {
		openingsText.text = titles[FileIndex];
		tutorialText.text = lines[FileIndex][LineIndex];
	}

	public void LoadOpening(int index)
    {
		pieces.gameObject.SetActive(true);
		prevButton.gameObject.SetActive(false);
		nextButton.gameObject.SetActive(true);
		currSetMoves.UndoAllPrevMoves();
		FileIndex = index;
		LineIndex = 0;
		currSetMoves = moves[FileIndex];
		LoadText();
    }
}

[System.Serializable]
public class MoveSimulator
{
    [SerializeField] public int start;
	[SerializeField] public int end;
	[SerializeField] public MoveFlag flag;
}