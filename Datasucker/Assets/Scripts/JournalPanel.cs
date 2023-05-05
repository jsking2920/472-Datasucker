using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JournalPanel : MonoBehaviour
{
    [SerializeField] private AccuseToggle accuseToggle;
    [SerializeField] private TextMeshProUGUI text;

    void OnEnable()
    {
        PlayerManager.Instance.HasJournalOpen = true;
        accuseToggle.gameObject.SetActive(false);
        text.text = "";
    }

    void OnDisable()
    {
        PlayerManager.Instance.HasJournalOpen = false;
        accuseToggle.Check();
    }
}
