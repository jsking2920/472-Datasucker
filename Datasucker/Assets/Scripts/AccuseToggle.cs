using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuseToggle : MonoBehaviour
{
    [SerializeField]
    private List<string> requirements;

    public bool Check()
    {
        foreach (string req in requirements)
        {
            if (!PlayerManager.Instance.Progress[req])
            {
                return false;
            }
        }
        gameObject.SetActive(true);
        return true;
    }
}
