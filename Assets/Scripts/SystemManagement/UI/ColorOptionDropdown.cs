using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class ColorOptionDropdown : MonoBehaviour
{
	[SerializeField] private TMP_Text selectedOptionText;
	[SerializeField] private string selectedColor = "Black";

	private void Start()
	{
		AssertAllReferenceIsNotNull();

		SetSelectedColorText();
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
				FlipBoard(true);
				selectedColor = "Black";
				SetSelectedColorText();
				break;

			case 1:
				FlipBoard(false);
				selectedColor = "White";
				SetSelectedColorText();
				break;
		}
	}

	public void SetSelectedColorText()
    {
		selectedOptionText.text = selectedColor;
    }

	public static void FlipBoard(bool boolean)
    {
		Piece.isBlackBelow = boolean;
		HighlightManager.isBlackBelow = boolean;
		Timer.isBlackBelow = boolean;
		BoardController.isBlackBelow = boolean;
	}
}