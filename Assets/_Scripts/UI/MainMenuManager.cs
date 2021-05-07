using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Texture2D CursorTex;

    private void Start()
    {
        CursorSet(CursorTex);
    }

    public void LoadLevel(int level)
    {
        PlayerPrefs.SetInt("LastLevel", level + 2);
        SceneManager.LoadScene(level + 2);
    }

    void CursorSet(Texture2D tex)
    {
        CursorMode mode = CursorMode.ForceSoftware;
        Vector2 hotSpot = Vector2.zero;
        Cursor.SetCursor(tex, hotSpot, mode);
    }

}
