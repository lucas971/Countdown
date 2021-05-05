using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomManager : MonoBehaviour
{
    public static BoomManager Instance;

    private List<BoomObject> boomObjects;
    private List<BoomObject> explosionObjects;

    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        Instance = this;
        boomObjects = new List<BoomObject>();
        explosionObjects = new List<BoomObject>();
    }

    public void AddBoomObject(BoomObject newObject)
    {
        boomObjects.Add(newObject);
    }

    public List<BoomObject> GetObjectsInExplosion(Vector2 pos, float radius)
    {
        explosionObjects.Clear();
        foreach (BoomObject b in boomObjects)
        {
            if (((Vector2)b.transform.position - pos).magnitude <= radius)
            {
                explosionObjects.Add(b);
            }
        }
        return explosionObjects;
    }


    
}
