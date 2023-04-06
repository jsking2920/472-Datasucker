using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class JournalPhoto : MonoBehaviour
{

    public string SubjectName;

    public int[] Requirements;

    public DialoguePanel ADialoguePanel;
    public DialogueScript ADialogueScript;

    private bool _unlocked;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTap()
    {
        if (_unlocked)
        {
            ADialoguePanel.gameObject.SetActive(true);
            ADialoguePanel.Initialize(ADialogueScript);
            ADialoguePanel.ShowDialogue();
        }
    }

    void OnEnable()
    {
        UpdateImage();
    }

    void UpdateImage()
    {
        string path = Path.Combine(Application.persistentDataPath, "JournalPhotos", SubjectName + ".png");
        if (_unlocked = File.Exists(path))
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
        return true;
    }
}
