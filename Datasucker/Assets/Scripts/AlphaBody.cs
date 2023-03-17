using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaBody : InteractableComponent
{
    public DialoguePanel ADialoguePanel;
    public DialogueScript ADialogueScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnObjectTapped()
    {
        base.OnObjectTapped();

        ADialoguePanel.gameObject.SetActive(true);
        ADialoguePanel.Initialize(ADialogueScript);
        ADialoguePanel.ShowDialogue();
    }
}
