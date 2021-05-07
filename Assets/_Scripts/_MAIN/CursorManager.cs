using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [SerializeField] private CursorVisual CursorPrefab;
    [SerializeField] private Vector2 Offset;
    private CursorVisual cursor;
    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Cursor");

        if (objs.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        Cursor.visible = false;
        Instance = this;
        cursor = Instantiate(CursorPrefab, transform);
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(cursor);
    }

    private void Update()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos += Offset;
        cursor.transform.position = pos;
    }

    public void HideCursor()
    {
        cursor.gameObject.SetActive(false);
    }

    public void ShowCursor()
    {
        cursor.gameObject.SetActive(true);
    }
}
