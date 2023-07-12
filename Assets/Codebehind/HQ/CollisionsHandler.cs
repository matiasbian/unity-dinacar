using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HQ;

public class CollisionsHandler : SingletonMonoBehaviour<CollisionsHandler>
{

    //CONSTS
    const int TRIP_POS_CHECK_OFFSET = 8;
    const float OBSTACLE_X_OFFSET = 0.1f;
    public float CAR_X_OFFSET = .2f;
    public float ENEMY_CAR_X_OFFSET = 0.3f;
    public float ENEMY_CAR_Y_OFFSET = 1f;

    //VARS
    public ProjectedBody body;
    //Testing vars
    public float segment, x;
    public CarModifier.Lane[] carX;
    //Actions
    public System.Action OnCarCollision, OnObstacleCollision;

    TrackObject track;
    // Start is called before the first frame update
    void Start()
    {
        track = CurrentTrackHandler.Instance.currentTrack;
        carX = new CarModifier.Lane[track.Cars.Length];
    }

    // Update is called once per frame
    void Update()
    {
        segment = body.GetTripPosition();
        x = body.GetPlayerX();
        CheckCarsCollision();
        CheckObstaclesCollision();
    }

    public float GetRoadMultiplier () {
        if (x < -1.05f || x > 1.0f) {
            return 0.4f;
        } else {
            return 1;
        }
    }

    public float GetSteeringMultiplier () {
        float multiplier = GetRoadMultiplier() * 2f;
        return Mathf.Min(multiplier, 1f);
    }

    void CheckObstaclesCollision () {
        var obstacles = track.GetObstacles();
        foreach (var obstacle in obstacles) {
            if (InRangeY(obstacle.y - TRIP_POS_CHECK_OFFSET, body.GetTripPosition(), ENEMY_CAR_Y_OFFSET)) {
                if (InRangeX(obstacle.x, body.GetPlayerX(), OBSTACLE_X_OFFSET)) {
                    Debug.Log("Obstacle collision " + obstacle.y);
                    OnObstacleCollision?.Invoke();
                }
            }
        }
    }

    void CheckCarsCollision () {
        float len = track.Cars.Length;
        for (int i = 0; i < len; i++) {
            var car = track.Cars[i];
            carX[i] = car.spriteX;
            var carPos = track.GetCarUpdatedPos(car);
            if (car.disabled) continue;
            
            if (InRangeY(carPos - TRIP_POS_CHECK_OFFSET, body.GetTripPosition(), ENEMY_CAR_Y_OFFSET)) {
                if (InRange(carX[i], body.GetPlayerX())) {
                    OnCarCollision?.Invoke();
                }
            }
        }
    }

    #region aux
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

    bool InRangeX (float obstaclePos, float playerXPos, float RANGE) {
        return playerXPos >= obstaclePos - RANGE && playerXPos <= obstaclePos + RANGE;
    }

    bool InRangeY (float enemyCarPos, float playerYPos, float RANGE) {
        return playerYPos >= enemyCarPos - RANGE && playerYPos <= enemyCarPos + RANGE;
    }
    #endregion
}
