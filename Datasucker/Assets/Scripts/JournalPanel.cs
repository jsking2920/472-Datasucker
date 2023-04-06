using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalPanel : MonoBehaviour
{
    void OnEnable()
    {
        PlayerManager.Instance.HasJournalOpen = true;
    }

    void OnDisable()
    {
        PlayerManager.Instance.HasJournalOpen = false;
    }
}
