using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;

    public static PlayerManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
    }

    private void Start()
    {
        for (int i = 0; i < ProgList.Count; i++)
        {
            ProgList[i] = false;
        }
    }

    // Let's use a scriptable object later to help saving and all that
    public List<bool> ProgList;

    public bool HasJournalOpen = false;
    public bool IsTalking = false;
}
