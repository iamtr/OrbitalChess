using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialGameController : GameController
{

	public TutorialManager tm;

	protected override void OnEnable()
	{
		base.OnEnable();
		OnRoundStart += tm.CheckCondition;
	}

	protected override void Start()
	{
		base.Start();
		currPlayer = PlayerType.Black;
		SetGameState(GameState.GameOver);
	}
	public override void SetPlayer()
	{
		return;
	}

	public override void HandleCheckAndCheckmate()
	{
		return;
	}
}

[System.Serializable]
public class Condition
{
	public PositionSO config;
	public List<Piece> pieces;
	public int position;
}