using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabSelectionManager : MonoBehaviour
{
    public List<GameObject> Prefabs;

    [SerializeField]
    private Text _displayText;

    [SerializeField]
    private Niantic.ARDKExamples.WayspotAnchors.WayspotPlacementManager manager;

    private int _current;

    private void Start()
    {
        manager.SetPrefab(Current());
    }

    public void Next()
    {
        _current += 1;
        if (_current >= Prefabs.Count)
        {
            _current = 0;
        }

        manager.SetPrefab(Current());
        _displayText.text = Current().name;
    }

    public GameObject Current()
    {
        return Prefabs[_current];
    }
}
