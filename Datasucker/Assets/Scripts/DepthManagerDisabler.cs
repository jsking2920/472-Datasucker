using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthManagerDisabler : MonoBehaviour
{

    private Niantic.ARDK.Extensions.ARDepthManager depthManager;

    void Start()
    {
        depthManager = GetComponent<Niantic.ARDK.Extensions.ARDepthManager>();
    }

    void Update()
    {
        depthManager.OcclusionTechnique = transform.forward.y < 0.72 ? Niantic.ARDK.Extensions.ARDepthManager.OcclusionMode.Auto : Niantic.ARDK.Extensions.ARDepthManager.OcclusionMode.None;
    }
}
