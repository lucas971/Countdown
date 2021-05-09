using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Transform Layout;
    [SerializeField] private GameObject LevelButton;

    private void Start()
    {
        int index = 0;
        foreach (LevelInfo info in LevelsManager.Instance.GetLevels())
        {
            GameObject newButton = Instantiate(LevelButton, Layout);
            newButton.name = "Level" + index.ToString();
            newButton.transform.GetChild(0).GetComponent<Image>().sprite = info.LevelImage;
            newButton.GetComponent<Button>().onClick.AddListener(delegate { LoadLevel(info.LevelName); });
            index++;
        }
    }
    public void LoadLevel(string level)
    {
        LevelsManager.Instance.SetLevel(level);
        SceneManager.LoadScene(1);
    }
}
