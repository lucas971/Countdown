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
        if (LevelsManager.Instance.GetNext() == LevelsManager.Instance.GetLevel())
        {
            NextButton.SetActive(false);
            RestartButton.SetActive(true);
        }    
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(1);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Next()
    {
        LevelsManager.Instance.SetLevel(LevelsManager.Instance.GetNext());
        SceneManager.LoadScene(1);
    }
}
