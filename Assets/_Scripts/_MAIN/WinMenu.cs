using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    [SerializeField] GameObject RestartButton;
    [SerializeField] GameObject NextButton;
    private void Start()
    {
        if (PlayerPrefs.GetInt("LastLevel") >= SceneManager.sceneCountInBuildSettings - 1)
        {
            NextButton.SetActive(false);
            RestartButton.SetActive(true);
        }    
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("LastLevel"));
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Next()
    {
        int target = PlayerPrefs.GetInt("LastLevel") + 1;
        PlayerPrefs.SetInt("LastLevel", target);
        SceneManager.LoadScene(target);
    }
}
