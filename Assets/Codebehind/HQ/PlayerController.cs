﻿using UnityEngine;

namespace HQ
{
    class PlayerController: MonoBehaviour
    {
        [Header("Car specs")]
        [SerializeField] float STEERING = 5f;
        [SerializeField] float ACCELERATION = 10f;
        [SerializeField] float BRAKE_POWER = 8.5f;
        [SerializeField] float DEACCELERATION_POWER = 2f;
        [SerializeField] public float MAX_SPEED = 200;

        // Move log X speed
        const float DIVIDER = 6.1f;

        [Header("References")]
        public HqRenderer hQcamera;
        public ProjectedBody body;
        
        float mSpeed = 0;
        float lastSpeed = 0;
        Animator animator;

        //events
        public delegate void OnSpeedChange(float speed);
        public event OnSpeedChange onSpeedChange;

        /// </summary>
        void Start()
        {
            animator = GetComponent<Animator>();
        }

        void OnEnable()
        {
            CollisionsHandler.Instance.OnCarCollision += OnCarCollision;
        }

        void OnDisable()
        {
            CollisionsHandler.Instance.OnCarCollision -= OnCarCollision;
        }

        void OnCarCollision () {
            mSpeed = mSpeed / 4;
        }

        private void FixedUpdate()
        {
            body.speed = 0;
            
            Steering();
            HandleAcceleration();
            
            if (Input.GetKey(KeyCode.Tab)) body.speed *= 3;
            if (Input.GetKey(KeyCode.W)) hQcamera.cameraHeight += 100;
            if (Input.GetKey(KeyCode.S)) hQcamera.cameraHeight -= 100;
        }

        void HandleAcceleration () 
        {
            float vertical = Input.GetAxis("Vertical");

            if (vertical > 0 && mSpeed <= GetMaxSpeed()) {
                mSpeed = Accelerate(mSpeed, vertical, CollisionsHandler.Instance.GetRoadMultiplier());
            } else if (vertical < 0) {
                mSpeed = Brake(mSpeed, vertical);
            } else if (mSpeed > 0) {
                mSpeed = Accelerate(mSpeed, - DEACCELERATION_POWER, 1 / CollisionsHandler.Instance.GetRoadMultiplier());
            }

            if (lastSpeed != mSpeed) {
                onSpeedChange?.Invoke(mSpeed);
                lastSpeed = mSpeed;
            }
        
            body.speed = Mathf.RoundToInt(mSpeed);

        }

        void Steering () {
            float horizontal = Input.GetAxisRaw("Horizontal");
            body.playerX += STEERING * horizontal * Time.fixedDeltaTime;
            animator.SetFloat("Steering", horizontal);
        }

        float Accelerate (float currentSpeed, float vertical, float multiplier) 
        {
            float logSpeed = currentSpeed + (ACCELERATION * Time.fixedDeltaTime * vertical * multiplier);

            return logSpeed;
        }

        float GetMaxSpeed () {
            return MAX_SPEED * CollisionsHandler.Instance.GetRoadMultiplier();
        }

        float Brake (float currentSpeed, float vertical) 
        {
            float logSpeed = currentSpeed + ((BRAKE_POWER * Time.fixedDeltaTime * vertical) / currentSpeed * 1000);
            return Mathf.Max(logSpeed, 0);
        }

        float GetMultiplier(float vertical) {
            if (vertical > 0) return 1;
            if (vertical < 0) return - BRAKE_POWER;
            return - DEACCELERATION_POWER;
        }
    }
}
