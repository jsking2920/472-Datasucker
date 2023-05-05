using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuseToggle : MonoBehaviour
{
    [SerializeField]
    private List<string> requirements;
    [SerializeField]
    private PopupPanel popupPanel;
    [SerializeField]
    private TextAsset textAsset;

    private bool seen = false;

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
        if (!seen)
        {
            popupPanel.Display(textAsset);
            seen = true;
        }
        return true;
    }
}
