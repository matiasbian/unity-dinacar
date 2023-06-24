using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HQ;

public class RenderEnemyCars : MonoBehaviour
{
    TrackObject trackObject;

    // Update is called once per frame
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        trackObject = GetComponent<HqRenderer>().track;
    }
    void FixedUpdate()
    {
        trackObject.UpdateCars(Time.fixedDeltaTime);
    }
}
