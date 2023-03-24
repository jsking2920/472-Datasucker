using Niantic.ARDK.Utilities.Input.Legacy;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;

public class DialoguePanel : MonoBehaviour
{
    public TextMeshProUGUI TextObject;
    public DialogueScript Dialogue;

    // Update is called once per frame
    private void Update()
    {
        if (PlatformAgnosticInput.touchCount > 0)
        {
            Touch touch = PlatformAgnosticInput.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                OnTapPanel(touch);
            }
        }
    }

    public void Initialize(DialogueScript dialogueScript)
    {
        Dialogue = dialogueScript;
        Dialogue.Initialize();
    }

    void OnEnable()
    {
        ProgressManager.Instance.IsTalking = true;
    }

    void OnDisable()
    {
        ProgressManager.Instance.IsTalking = false;
    }

    private void OnTapPanel(Touch touch)
    {
        // May want to allow no responses (meaning no links). For now at least one response like "ok" is required for this to work.
        // If there are links:
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(TextObject, touch.position, null);
        if (linkIndex != -1)
        {
            // This should work if the tmp creators are sane individuals. If for some reason indices are out of order,
            // we should actually use the link id at this index instead of just passing the index.
            if (Dialogue.Respond(linkIndex))
            {
                ShowDialogue();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void ShowDialogue()
    {
        TextObject.text = Dialogue.Read();
    }
}
