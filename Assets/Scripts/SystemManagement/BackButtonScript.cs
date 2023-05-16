using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonScript : MonoBehaviour
{
    public int gameStartScene;

    public void backMenu()
    {
        SceneManager.LoadScene(gameStartScene);
    }
}
