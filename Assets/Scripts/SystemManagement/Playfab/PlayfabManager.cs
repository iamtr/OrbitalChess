using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class PlayfabManager : MonoBehaviour
{
	[Header("UI")]
	public TMP_Text messageText;

	public TMP_InputField emailInputField;
	public TMP_InputField passwordInputField;
	public GameObject loginPanel;
	public GameObject mainMenuPanel;

	public void RegisterButton()
	{
		var request = new RegisterPlayFabUserRequest
		{
			Email = emailInputField.text,
			Password = passwordInputField.text,
			RequireBothUsernameAndEmail = false
		};
		PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);

		var loginRequest = new LoginWithEmailAddressRequest
		{
			Email = emailInputField.text,
			Password = passwordInputField.text
		};
		PlayFabClientAPI.LoginWithEmailAddress(loginRequest, OnLoginSuccess, OnError);
	}

	private void OnRegisterSuccess(RegisterPlayFabUserResult result)
	{
		messageText.text = "Registered!";
		Debug.Log(result);
	}

	public void LoginButton()
	{
		var request = new LoginWithEmailAddressRequest
		{
			Email = emailInputField.text,
			Password = passwordInputField.text
		};
		PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
	}

	private void OnLoginSuccess(LoginResult result)
	{
		messageText.text = "Logged in!";
		Debug.Log("Logged in!");
		loginPanel.SetActive(false);
		mainMenuPanel.SetActive(true);
	}

	public void ResetPasswordButton()
	{
		var request = new SendAccountRecoveryEmailRequest
		{
			Email = emailInputField.text,
			TitleId = PlayFabSettings.TitleId
		};
		PlayFabClientAPI.SendAccountRecoveryEmail(request, OnResetPasswordSuccess, OnError);
	}

	public void OnResetPasswordSuccess(SendAccountRecoveryEmailResult result)
	{
		messageText.text = "Password reset email sent!";
		Debug.Log("Password reset email sent!");
	}

	public void OnError(PlayFabError error)
	{
		messageText.text = error.ErrorMessage;
		Debug.Log(error.GenerateErrorReport());
	}
}