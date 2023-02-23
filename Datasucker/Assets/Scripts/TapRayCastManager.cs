using System.Collections;
using System.Collections.Generic;
using Niantic.ARDK.Utilities.Input.Legacy;
using UnityEngine;

public class TapRayCastManager : MonoBehaviour
{
    public GameObject characterPrefab;

    private float count = 0;
    private GameObject thisCharacter;

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
        count += 1;
        if (count == 1)
        {
            thisCharacter = Instantiate(characterPrefab);
            thisCharacter.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 6;
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
