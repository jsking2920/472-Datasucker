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
        depthManager.enabled = Mathf.Pow(transform.forward.y, 2) < 0.5;
    }
}
