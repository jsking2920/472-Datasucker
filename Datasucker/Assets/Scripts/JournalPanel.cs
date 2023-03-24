using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalPanel : MonoBehaviour
{
    void OnEnable()
    {
        ProgressManager.Instance.HasJournalOpen = true;
    }

    void OnDisable()
    {
        ProgressManager.Instance.HasJournalOpen = false;
    }
}
