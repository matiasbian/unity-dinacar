using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Speedometer : MonoBehaviour
{
    [SerializeField] Image mark;
    [SerializeField] TextMeshProUGUI speedText;
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
        float MAX_SPEED_MULTIPLIER = (speed / player.MAX_SPEED) * 0.8f;
        mark.rectTransform.eulerAngles = new Vector3(0, 0,  120 -  (speed * MAX_SPEED_MULTIPLIER));
        speedText.text = Mathf.RoundToInt(speed).ToString();
    }
}
