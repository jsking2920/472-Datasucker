using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueComponent : InteractableComponent
{

    private DialoguePanel _dialoguePanel;
    public DialogueScript ADialogueScript;
    public bool Accusable;

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
        if (Accusable && PlayerManager.Instance.IsAccusing) {
            PlayerManager.Instance.Accuse(gameObject);
            return;
        }
        base.OnObjectTapped();

        _dialoguePanel.gameObject.SetActive(true);
        _dialoguePanel.Initialize(ADialogueScript, gameObject);
        _dialoguePanel.ShowDialogue();
    }
}
