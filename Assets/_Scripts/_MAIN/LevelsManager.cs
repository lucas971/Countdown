using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelsManager : MonoBehaviour
{
    public static LevelsManager Instance;

    [SerializeField] List<LevelInfo> levels;

    private string currentLevel;

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("LevelsManager");

        if (objs.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public string GetNext()
    {
        bool found = false;
        foreach(LevelInfo info in levels)
        {
            if (found)
                return info.LevelName;

            if (info.LevelName == currentLevel)
                found = true;
        }

        return currentLevel;
    }

    public List<LevelInfo> GetLevels()
    {
        return levels;
    }

    public void SetLevel(string level)
    {
        currentLevel = level;
    }

    public string GetLevel()
    {
        return currentLevel;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            Debug.Log(currentLevel);
            Instantiate(Resources.Load("Levels/" + currentLevel));
        }
    }
}
