using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BlitzUIManager : UIManager
{
	[Header("Blitz UI")]
	public GameObject board;
	public GameObject gameCanvas;
	public GameObject modeSelectCanvas;

    protected override void AssertAllReferenceIsNotNull()
    {
        base.AssertAllReferenceIsNotNull();
		Assert.IsNotNull(board);
		Assert.IsNotNull(gameCanvas);
		Assert.IsNotNull(modeSelectCanvas);
	}

    public void StartBlitzGame()
	{
		modeSelectCanvas.SetActive(false);
		board.SetActive(true);
		gameCanvas.SetActive(true);
		Timer.isGameStart = true;
	}
}
