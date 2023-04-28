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
        float delta = 0.1f;

        Vector3 axes = GetDeltaAxes(axis, delta);

        manager.UpdatePrefabRotation(axes);
    }

    public void UpdatePosition(int axis)
    {
        float delta = 0.1f;

        Vector3 axes = GetDeltaAxes(axis, delta);

        manager.UpdatePrefabPosition(axes);
    }

    public void UpdateScale(bool positive)
    {
        float delta = positive ? 0.1f : 0.1f;

        Vector3 axes = new Vector3(delta, delta, delta);

        manager.UpdatePrefabScale(axes);
    }
}
