using System.Collections;
using System.Collections.Generic;
using Niantic.ARDK.Utilities.Input.Legacy;
using UnityEngine;

public class TapRayCastManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlatformAgnosticInput.touchCount > 0)
        {
            Touch touch = PlatformAgnosticInput.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                OnTapScreen(touch);
            }
        }
    }

    void OnTapScreen(Touch touch)
    {
        // If journal is up or dialogue is up just return
        if (PlayerManager.Instance.HasJournalOpen || PlayerManager.Instance.IsTalking)
        {
            return;
        }
        Ray raycast = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit raycastHit;
        if (Physics.Raycast(raycast, out raycastHit))
        {
            if (raycastHit.collider.GetComponent<InteractableComponent>() != null)
            {
                raycastHit.collider.GetComponent<InteractableComponent>().OnObjectTapped();
            }
        }
    }
}
