using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JournalPhoto : MonoBehaviour
{

    public string SubjectName;

    public string[] Requirements;
    public bool Scripted;

    // public DialoguePanel ADialoguePanel;
    // public DialogueScript ADialogueScript;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private JournalText textRes;

    private bool _unlocked;

    // Start is called before the first frame update
    void Start()
    {
        if (Scripted)
        {
            _unlocked = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTap()
    {
        if (_unlocked)
        {
            text.text = textRes.Read();
            // ADialoguePanel.gameObject.SetActive(true);
            // ADialoguePanel.Initialize(ADialogueScript);
            // ADialoguePanel.ShowDialogue();
        }
    }

    void OnEnable()
    {
        if (!Scripted)
        {
            UpdateImage();
        }
    }

    void UpdateImage()
    {
        string path = Path.Combine(Application.persistentDataPath, "JournalPhotos", SubjectName + ".png");
        Debug.Log(path);
        if (_unlocked = CheckRequirements())
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            GetComponent<Image>().sprite = sprite;
        }
    }

    bool CheckRequirements() //use this once we've replaced the bool array with a map, that can be more generic and also descriptive (ex: BodySeen, GuyConfessed, etc)
    {
        bool met = true;
        foreach (var req in Requirements)
        {
            met &= PlayerManager.Instance.CheckProgress(req);
            if (!met) break;
        }
        return met;
    }
}
