using Niantic.ARDK.Utilities.Input.Legacy;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;

public class DialoguePanel : MonoBehaviour
{
    public TextMeshProUGUI TextObject;
    public GameObject ResponsePanel;
    private DialogueScript _dialogue;
    
    private AudioSource voiceBox;
    public float volume = 0.7f;
    [SerializeField] private AudioClip[] voices;
    [SerializeField] private AccuseToggle accuseToggle;

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
        }
    }

    public void ShowDialogue()
    {
        List<string> panelText = _dialogue.Read();
        TextObject.text = panelText[0];
        if (panelText.Count < 2) // Special case if no responses provided (should write ok manually but if we forget this covers us)
        {
            ResponsePanel.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Ok";
            // Hide buttons besides first one
            for (int i = 1; i < ResponsePanel.transform.childCount; i++)
            {
                ResponsePanel.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else 
        {
            for (int i = 0; i < ResponsePanel.transform.childCount; i++)
            {
                bool buttonShouldShow = i < panelText.Count - 1  && !panelText[i+1].Equals(""); //xiao changed this thing
                Transform childButton = ResponsePanel.transform.GetChild(i);
                childButton.gameObject.SetActive(buttonShouldShow);
                if (buttonShouldShow)
                {
                    ResponsePanel.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = panelText[i+1];
                }
            }
        }
        
        if (_dialogue.HasVoice)
        {
            PlayVoice();
        }
    }

    private void PlayVoice()
    {
        int voice = Random.Range(0,4);
        voiceBox?.PlayOneShot(voices[voice], volume);
        Debug.Log(voice);
    }
}
