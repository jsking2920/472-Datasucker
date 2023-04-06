using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueComponent : InteractableComponent
{

    private DialoguePanel _dialoguePanel;
    public DialogueScript ADialogueScript;

    // Start is called before the first frame update
    void Start()
    {
        _dialoguePanel = PlayerManager.Instance.DialoguePanel;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnObjectTapped()
    {
        base.OnObjectTapped();

        _dialoguePanel.gameObject.SetActive(true);
        _dialoguePanel.Initialize(ADialogueScript);
        _dialoguePanel.ShowDialogue();
    }
}
