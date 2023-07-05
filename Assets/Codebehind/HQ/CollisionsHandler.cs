using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HQ;

public class CollisionsHandler : SingletonMonoBehaviour<CollisionsHandler>
{

    //CONSTS
    const int TRIP_POS_CHECK_OFFSET = 8;
    public float CAR_X_OFFSET = .2f;
    public float ENEMY_CAR_X_OFFSET = 0.3f;
    public float ENEMY_CAR_Y_OFFSET = 1f;

    //VARS
    public ProjectedBody body;
    //Testing vars
    public float segment, x;
    public CarModifier.Lane[] carX;
    //Actions
    public System.Action OnCarCollision;

    // Start is called before the first frame update
    void Start()
    {
        carX = new CarModifier.Lane[body.track.Cars.Length];
    }

    // Update is called once per frame
    void Update()
    {
        segment = body.GetTripPosition();
        x = body.GetPlayerX();
        CheckCarsCollision();
    }

    public float GetRoadMultiplier () {
        if (x < -1.05f || x > 0.66f) {
            return 0.5f;
        } else {
            return 1;
        }
    }

    void CheckCarsCollision () {
        float len = body.track.Cars.Length;
        for (int i = 0; i < len; i++) {
            var car = body.track.Cars[i];
            carX[i] = car.spriteX;
            var carPos = body.track.GetCarUpdatedPos(car);
            if (car.disabled) continue;
            
            if (InRangeY(carPos - TRIP_POS_CHECK_OFFSET, body.GetTripPosition(), ENEMY_CAR_Y_OFFSET)) {
                if (InRange(carX[i], body.GetPlayerX())) {
                    Debug.Log("Collision");
                    OnCarCollision?.Invoke();
                }
            }
        }
    }

    bool InRange(CarModifier.Lane pos, float playerX) {
        switch (pos) {
            case CarModifier.Lane.Left:
                return playerX > -1.14f && playerX < -0.23f;
            case CarModifier.Lane.Middle:
                return playerX > -0.42f && playerX < 0.42f;
            case CarModifier.Lane.Right:
                return playerX > 0.3f && playerX < 1.26f;
            default:
                return false;
        }
    }

    bool InRangeY (float enemyCarPos, float playerYPos, float RANGE) {
        return playerYPos >= enemyCarPos - RANGE && playerYPos <= enemyCarPos + RANGE;
    }
}
