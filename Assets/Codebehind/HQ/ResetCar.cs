using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCar : MonoBehaviour
{
    const float WAITING_TIME = 1f;
    // Start is called before the first frame update
    public System.Action OnResetFinished;
    HQ.ProjectedBody body;

    float waitime = 0;
    void Start()
    {
        body = GetComponent<HQ.ProjectedBody>();
    }

    void OnDisable()
    {
        waitime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitime < WAITING_TIME) {
            waitime += Time.deltaTime;
            return;
        }

        body.playerX = Mathf.MoveTowards(body.playerX, 0, Time.deltaTime * 2);
        if (Mathf.Abs(body.playerX) < 0.01f) {
            OnResetFinished?.Invoke();
            enabled = false;
        }
    }
}
