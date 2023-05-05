using Niantic.ARDK.Utilities.Input.Legacy;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
    public TextMeshProUGUI TextObject;
    public GameObject ResponsePanel;
    private DialogueScript _dialogue;

    private GameObject speaker;
    
    private AudioSource voiceBox;
    public float volume = 0.7f;
    [SerializeField] private AudioClip[] voices;
    [SerializeField] private AccuseToggle accuseToggle;
    [SerializeField] private Image profileImage;

    public void Initialize(DialogueScript dialogueScript)
    {
        _dialogue = dialogueScript;
        _dialogue.Initialize();
        voiceBox = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        PlayerManager.Instance.IsTalking = true;
    }

    void OnDisable()
    {
        PlayerManager.Instance.IsTalking = false;
    }

    public void Respond(int index) {
        if (_dialogue.Respond(index))
        {
            ShowDialogue();
        }
        else   
        {
            gameObject.SetActive(false);
            accuseToggle.Check();
            speaker.GetComponent<Animator>()?.Play("Idle");
        }
    }

    public void ShowDialogue()
    {
        Sprite profileSprite = _dialogue.ProfileSprite;
        if (profileSprite == null)
        {
            profileImage.gameObject.SetActive(false);
        }
        else
        {
            profileImage.gameObject.SetActive(true);
            profileImage.sprite = profileSprite;
        }
        string animFlag;
        List<string> panelText = _dialogue.Read(out animFlag);
        TextObject.text = panelText[0];
        if (panelText.Count < 2) // Special case if no responses provided (should write ok manually but if we forget this covers us)
        {
            GetChildButton(0).GetComponentInChildren<TextMeshProUGUI>().text = "Ok";
            // Hide buttons besides first one
            for (int i = 1; i < ResponsePanel.transform.childCount; i++)
            {
                GetChildButton(i).gameObject.SetActive(false);
            }
        }
        else 
        {
            for (int i = 0; i < ResponsePanel.transform.childCount; i++)
            {
                bool buttonShouldShow = i < panelText.Count - 1  && !panelText[i+1].Equals(""); //xiao changed this thing
                Transform childButton = GetChildButton(i);
                childButton.gameObject.SetActive(buttonShouldShow);
                if (buttonShouldShow)
                {
                    GetChildButton(i).GetComponentInChildren<TextMeshProUGUI>().text = panelText[i+1];
                }
            }
        }
        
        if (_dialogue.HasVoice)
        {
            PlayVoice();
        }

        if (speaker != null && animFlag != "")
        {
            speaker.GetComponent<Animator>()?.Play(animFlag);
        }
    }

    private void PlayVoice()
    {
        int voice = Random.Range(0,4);
        voiceBox?.PlayOneShot(voices[voice], volume);
        Debug.Log(voice);
    }

    private Transform GetChildButton(int index)
    {
        if (index < 2)
        {
            return ResponsePanel.transform.GetChild(0).GetChild(index);
        }
        else
        {
            return ResponsePanel.transform.GetChild(1).GetChild(index - 2);
        }
    }
}
