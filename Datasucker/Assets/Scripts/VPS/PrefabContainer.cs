using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

[CreateAssetMenu(fileName = "Prefabs", menuName = "ScriptableObjects/Prefabs", order = 2)]
public class PrefabContainer : ScriptableObject
{
    public List<GameObject> PrefabList;

    public Dictionary<string, GameObject> Prefabs;

    public void Init()
    {
        if (Prefabs != null) Prefabs.Clear();
        else Prefabs = new Dictionary<string, GameObject>();
        foreach (var p in PrefabList)
        {
            string key = p.name + "(Clone)";
            Prefabs.Add(key, p);
        }
    }
}
