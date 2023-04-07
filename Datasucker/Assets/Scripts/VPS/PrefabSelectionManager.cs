using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabSelectionManager : MonoBehaviour
{
    public PrefabContainer PrefabContainer;

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
        if (_current >= PrefabContainer.PrefabList.Count)
        {
            _current = 0;
        }

        manager.SetPrefab(Current());
        _displayText.text = Current().name;
    }

    public GameObject Current()
    {
        return PrefabContainer.PrefabList[_current];
    }
}
