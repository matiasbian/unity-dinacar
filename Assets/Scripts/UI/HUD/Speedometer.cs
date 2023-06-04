using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    [SerializeField] Image mark;
    [SerializeField] HQ.PlayerController player;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        player.onSpeedChange += OnSpeedChange;
    }
    
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        player.onSpeedChange -= OnSpeedChange;
    }

    void OnSpeedChange(float speed)
    {
        mark.rectTransform.eulerAngles = new Vector3(0, 0,  120 -  (speed * 0.7f));
    }
}
