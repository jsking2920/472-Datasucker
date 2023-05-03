using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressEntry
{
    public string Condition;
    public bool IsMet;
}

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

    private void Start()
    {
        Progress = new Dictionary<string, bool>();
        foreach (ProgressEntry prog in _startProgress) 
        {
            UpdateProgress(prog.Condition, prog.IsMet);
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

    // Let's use a scriptable object later to help saving and all that
    public DialoguePanel DialoguePanel;

    public Dictionary<string, bool> Progress;

    public bool HasJournalOpen = false;
    public bool IsTalking = false;
    public bool HasPopupOpen = false;



    // Serialized tuple list to construct dictionary on start
    [SerializeField]
    private List<ProgressEntry> _startProgress;

    public void UpdateProgress(string condition, bool met) {
        if (!Progress.TryAdd(condition, met))
        {
            Progress[condition] = met;
        }
    }

    public bool CheckProgress(string condition) {
        return Progress.ContainsKey(condition) && Progress[condition];
    }
}
