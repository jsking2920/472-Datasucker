using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayspotAnchorTransformer : MonoBehaviour
{
    public Niantic.ARDKExamples.WayspotAnchors.WayspotPlacementManager manager;

    private Vector3 GetDeltaAxes(int axis, float delta)
    {
        Vector3 axes = Vector3.zero;
        switch(axis)
        {
            case 0: axes.x = delta;
            break;

            case 1: axes.y = delta;
            break;
            
            case 2: axes.z = delta;
            break;

            case 3: axes.x = -delta;
            break;

            case 4: axes.y = -delta;
            break;

            case 5: axes.z = -delta;
            break;
        }

        return axes;
    }

    public void UpdateRotation(int axis)
    {
        float delta = 5f;

        Vector3 axes = GetDeltaAxes(axis, delta);

        manager.UpdatePrefabRotation(axes);
    }

    public void UpdatePosition(int axis)
    {
        float delta = 0.05f;

        Vector3 axes = GetDeltaAxes(axis, delta);

        manager.UpdatePrefabPosition(axes);
    }
}
