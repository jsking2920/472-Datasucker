using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class JournalPhoto : MonoBehaviour
{

    public string SubjectName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        UpdateImage();
    }

    void UpdateImage()
    {
        string path = Path.Combine(Application.persistentDataPath, "JournalPhotos", SubjectName + ".png");
        if (File.Exists(path))
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            GetComponent<Image>().sprite = sprite;
        }
    }
}
