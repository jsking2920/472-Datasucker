using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DemoCharacter : InteractableComponent
{

    public GameObject dialogueCanvasPrefab;
    
    private Animation anim;

    private GameObject currentDialogueCanvas;
    private bool isTalking = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnObjectTapped()
    {
        base.OnObjectTapped();

        if (!isTalking)
        {
            isTalking = true;
            currentDialogueCanvas = Instantiate(dialogueCanvasPrefab);
            anim.Play("mixamo.com");
        }
        else
        {
            isTalking = false;
            Destroy(currentDialogueCanvas);
        }
    }
}
