using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void LoadLevel(int level)
    {
        PlayerPrefs.SetInt("LastLevel", level + 2);
        SceneManager.LoadScene(level + 2);
    }
}
