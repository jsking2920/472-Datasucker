using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalPanel : MonoBehaviour
{
    [SerializeField] private AccuseToggle accuseToggle;

    void OnEnable()
    {
        PlayerManager.Instance.HasJournalOpen = true;
        accuseToggle.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        PlayerManager.Instance.HasJournalOpen = false;
        accuseToggle.Check();
    }
}
