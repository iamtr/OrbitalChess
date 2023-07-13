using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class ColorOptionDropdown : MonoBehaviour
{
	[SerializeField] private static TMP_Text selectedOptionText;

	private void Start()
	{
		AssertAllReferenceIsNotNull();
	}

	private void AssertAllReferenceIsNotNull()
	{
		Assert.IsNotNull(selectedOptionText);
	}

	public static void Dropdown(int index)
	{
		switch (index)
		{
			case 0:
				Piece.isBlackBelow = true;
				HighlightManager.isBlackBelow = true;
				Timer.isBlackBelow = true;
				BoardController.isBlackBelow = true;
				break;

			case 1:
				Piece.isBlackBelow = false;
				HighlightManager.isBlackBelow = false;
				Timer.isBlackBelow = false;
				BoardController.isBlackBelow = false;
				break;
		}
	}
}