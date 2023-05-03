using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMesh;
    public void Display(TextAsset textAsset)
    {
        textMesh.text = textAsset.text;
        gameObject.SetActive(true);
        PlayerManager.Instance.HasPopupOpen = true;
    }

    public void Close()
    {
        PlayerManager.Instance.HasPopupOpen = false;
        gameObject.SetActive(false);
    }
}
