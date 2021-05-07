using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    public void PlayAgain()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("LastLevel"));
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
