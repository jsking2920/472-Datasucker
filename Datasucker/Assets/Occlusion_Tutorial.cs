using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.ARDK.Utilities.Input.Legacy;
using Niantic.ARDK.AR;
using Niantic.ARDK.Utilities;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.AR.HitTest;
public class Occlusion_Tutorial : MonoBehaviour
{
    //we need the camera for the hit test
    public Camera _camera;

    //our character (yeti)
    public GameObject _character;

    //we need the session for the hit test.
    IARSession _session;

    void Start()
    {
        //we will need to catch the session in for our hit test function.
        ARSessionFactory.SessionInitialized += OnSessionInitialized;
    }

    //callback for the session starting.
    private void OnSessionInitialized(AnyARSessionInitializedArgs args)
    {
        //only run once guard
        ARSessionFactory.SessionInitialized -= OnSessionInitialized;

        //save the session.
        _session = args.Session;
    }

    //per frame update
    void Update()
    {
        //if there is a touch call our function
        if (PlatformAgnosticInput.touchCount <= 0) { return; }

        var touch = PlatformAgnosticInput.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            TouchBegan(touch);
        }
    }
    private void TouchBegan(Touch touch)
    {
        //check we have a valid frame.
        var currentFrame = _session.CurrentFrame;
        if (currentFrame == null)
        {
            return;
        }

        if (_camera == null)
            return;

        //do a hit test at at that screen point
        var hitTestResults =
            currentFrame.HitTest
            (
                _camera.pixelWidth,
                _camera.pixelHeight,
                touch.position,
                ARHitTestResultType.ExistingPlaneUsingExtent |
                ARHitTestResultType.EstimatedHorizontalPlane
            );

        if (hitTestResults.Count == 0)
            return;

        //move our character to the touch hit location
        // Set the cursor object to the hit test result's position
        _character.transform.position = hitTestResults[0].WorldTransform.ToPosition();

        // Orient the cursor object to look at the user, but remain flat on the "ground", aka
        // only rotate about the y-axis
        _character.transform.LookAt
        (
            new Vector3
            (
                currentFrame.Camera.Transform[0, 3],
                _character.transform.position.y,
                currentFrame.Camera.Transform[2, 3]
            )
        );
    }
}