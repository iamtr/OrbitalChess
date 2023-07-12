using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class ModeSelectButton : MonoBehaviour
{
	[SerializeField] private float minutesPerPlayer;
	[SerializeField] private float secondsAdded;
	public TMP_Text selectedModeText;
	public TMP_Text buttonText;

    private void Start()
    {
		AssertAllReferenceIsNotNull();
    }

    private void AssertAllReferenceIsNotNull()
	{
		Assert.IsNotNull(selectedModeText);
		Assert.IsNotNull(buttonText);
	}

	public void SetMode()
	{
		Timer.startMinutes = minutesPerPlayer;
		Timer.secondsToAddAfterMove = secondsAdded;
		selectedModeText.text = "Mode chosen: " + buttonText.text;
	}
}