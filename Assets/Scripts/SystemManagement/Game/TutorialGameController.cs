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

    protected override void OnDisable()
    {
        base.OnDisable();
		OnRoundStart -= tm.CheckCondition;
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

	public static void EnableCondition(Condition condition)
    {
		OnRoundEnd += condition.decreaseNumberOfMoves;
    }
	public static void DisableCondition(Condition condition)
	{
		OnRoundEnd -= condition.decreaseNumberOfMoves;
	}
}


[System.Serializable]
public class Condition
{
	public PositionSO config;
	public List<Piece> pieces;
	public int position;
	public int numberOfMovesLeft;
	[SerializeField] private int numberOfMovesRequired;

	public void decreaseNumberOfMoves()
    {
		numberOfMovesLeft--;
    }
	
	public void ResetNumberOfMoves()
    {
		numberOfMovesLeft = numberOfMovesRequired;
	}
}