using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class ColorOptionDropdown : MonoBehaviour
{
	[SerializeField] private TMP_Text selectedOptionText;

	private void Start()
	{
		AssertAllReferenceIsNotNull();
	}

	private void AssertAllReferenceIsNotNull()
	{
		Assert.IsNotNull(selectedOptionText);
	}

	public void Dropdown(int index)
	{
		switch (index)
		{
			case 0:
				Piece.isBlackBelow = true;
				HighlightManager.isBlackBelow = true;
				Timer.isBlackBelow = true;
				selectedOptionText.text = "Black";
				break;

			case 1:
				Piece.isBlackBelow = false;
				HighlightManager.isBlackBelow = false;
				Timer.isBlackBelow = false;
				selectedOptionText.text = "White";
				break;
		}
	}
}