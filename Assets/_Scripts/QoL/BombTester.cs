#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public static class BombTester
{
    static float radius = .65f;
    static int xSize = 75;
    static int ySize = 35;
    static float cooldown = .1f;
    static bool toggled = false;
    static double timer;

    [MenuItem("Bombs/Test Bombs _g")]
    public static void Toggle()
    {
        if (toggled)
        {
            timer = EditorApplication.timeSinceStartup;
            toggled = false;
            EditorApplication.update -= Update;
        }
        else
        {
            toggled = true;
            EditorApplication.update += Update;
        }
    }

    public static void Update()
    {
        if (timer < EditorApplication.timeSinceStartup)
        {
            timer = EditorApplication.timeSinceStartup + cooldown;
            TestAllPos();
        }
    }
    public static void TestAllPos()
    {
        Vector3 toTest = Vector3.zero;
        Color toPaint;
        for (int x = -xSize/2; x < xSize/2; x++)
        {
            for (int y = -ySize/2; y < ySize/2; y++)
            {
                toTest.Set(x * Bomb.gridSize, y * Bomb.gridSize, 0);
                toPaint = Color.gray;
                if (TestPos(toTest))
                    toPaint = Color.white;

                Debug.DrawRay(toTest, Vector2.up * .1f, toPaint, 1f);
            }
        }
    }

    static RaycastHit2D[] hits;
    public static bool TestPos(Vector2 pos)
    {
        bool res = false;
        hits = Physics2D.CircleCastAll(pos, radius, Vector2.zero, 0);
        foreach (var hit in hits)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("BombMask"))
            {
                res = true;
            }
            else if (hit.transform.gameObject.layer == 8 || hit.transform.gameObject.layer == 3)
            {
                return false;
            }
        }
        return res;
    }
}
#endif