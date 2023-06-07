using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HQ;

public class RenderEnemyCars : MonoBehaviour
{
    TrackObject trackObject;

    // Update is called once per frame
    void Update()
    {
        trackObject = GetComponent<HqRenderer>().track;
        trackObject.UpdateCars();
    }
}
