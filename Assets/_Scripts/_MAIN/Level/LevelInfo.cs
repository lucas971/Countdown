using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "LevelData", order = 1)]
public class LevelInfo : ScriptableObject
{
    public Sprite LevelImage;
    public string LevelName;
}
