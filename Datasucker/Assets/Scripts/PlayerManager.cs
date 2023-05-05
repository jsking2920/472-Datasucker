using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public bool IsAccusing = false;

    [SerializeField] private GameObject jailCell;



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

    public void Accuse(GameObject gameObject) {
        IsTalking = true;
        StartCoroutine(FinishAccusing(gameObject));
    }

    private IEnumerator FinishAccusing(GameObject guy)
    {
        bool win = gameObject.name == "Police(Clone)" || gameObject.name == "Police";
        float duration = 0.7f;
        Vector3 a = guy.transform.position + new Vector3(0,3,0);
        Vector3 b = guy.transform.position;
        GameObject cell = Instantiate(jailCell, a, Quaternion.identity);

        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            cell.transform.position = Vector3.Lerp(a, b, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        cell.transform.position = b;

        guy.GetComponent<Animator>().Play("Jailed");
        yield return new WaitForSeconds(2);

        if (win)
        {
            SceneManager.LoadScene("Success");
        }
        else
        {
            SceneManager.LoadScene("Fail");
        }
    }

    public void SetAccusing(bool isAccusing)
    {
        IsAccusing = isAccusing;
    }
}
