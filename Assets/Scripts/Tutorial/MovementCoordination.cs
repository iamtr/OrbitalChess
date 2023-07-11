using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MovementCoordination : MonoBehaviour
{
	[SerializeField] private SetMove[] moves;

	[SerializeField] private TextAsset[] openingsFiles;

	[SerializeField] private TMP_Text openingsText;
	[SerializeField] private TMP_Text tutorialText;

	private string[] titles = {"Italian Game", "Sicilian Defense", "Ruy López Opening" };

	private int FileIndex = 0;
	private int LineIndex = 0;

	private string[][] lines;

	private SetMove currSetMoves;

	private void Awake()
	{
		ReadAndStoreFiles(openingsFiles);
		GameController.SetGameState(GameState.GameOver);
		LoadText();
		currSetMoves = moves[0];
		//ColorOptionDropdown.Dropdown(1);
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
		if (FileIndex == 0 && LineIndex == 0) return;
		if (LineIndex == 0)
		{
			FileIndex--;
			LineIndex = lines[FileIndex].Length - 1;
			currSetMoves = moves[FileIndex];
			currSetMoves.ExecuteAllMoves();
		}
		else
		{
			LineIndex--;
		}

		LoadText();

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
		var CurrFile = lines[FileIndex];
		if (LineIndex == CurrFile.Length - 1)
		{
			if (FileIndex == lines.Length - 1) return;
			currSetMoves.UndoAllPrevMoves();
			FileIndex++;
			LineIndex = 0;
			currSetMoves = moves[FileIndex];
		}
		else
		{
			LineIndex++;
		}
		LoadText();
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