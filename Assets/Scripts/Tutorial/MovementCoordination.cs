using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MovementCoordination : MonoBehaviour
{
	[SerializeField] private SetMoves[] moves;

	[SerializeField] private TextAsset[] openingsFiles;

	[SerializeField] private TMP_Text openingsText;
	[SerializeField] private TMP_Text tutorialText;

	private string[] titles = {"Italian Game", "Sicilian Defense", "" };

	private int FileIndex = 0;
	private int LineIndex = 0;

	private string[][] lines;

	private void Awake()
	{
		ReadAndStoreFiles(openingsFiles);
		GameController.SetGameState(GameState.GameOver);
		openingsText.text = titles[FileIndex];
		tutorialText.text = lines[FileIndex][LineIndex];
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
			moves[FileIndex].ExecuteAllMoves();
		}
		else
		{
			LineIndex--;
		}

		if (lines[FileIndex][LineIndex].Contains("@"))
		{
			moves[FileIndex].PreviousMove();
			TriggerPrevLine();
		}

		openingsText.text = titles[FileIndex];
		tutorialText.text = lines[FileIndex][LineIndex];
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
			moves[FileIndex].UndoAllPrevMoves();
			FileIndex++;
			LineIndex = 0;
		}
		else
		{
			LineIndex++;
		}
		openingsText.text = titles[FileIndex];
		tutorialText.text = lines[FileIndex][LineIndex];
		if (lines[FileIndex][LineIndex].Contains("@"))
		{
			moves[FileIndex].ExecuteMove();
			TriggerNextLine();
		}
	}
}

[System.Serializable]
public class MoveSimulator
{
    [SerializeField] public int start;
	[SerializeField] public int end;
	[SerializeField] public MoveFlag flag;
}