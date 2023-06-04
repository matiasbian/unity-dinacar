using UnityEngine;

namespace HQ
{
    class PlayerController: MonoBehaviour
    {
        [Header("Car specs")]
        [SerializeField] float STEERING = 5f;
        [SerializeField] float ACCELERATION = 10f;
        [SerializeField] float BRAKE_POWER = 8.5f;
        [SerializeField] float DEACCELERATION_POWER = 2f;
        [SerializeField] float MAX_SPEED = 200;

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
        private void FixedUpdate()
        {
            body.speed = 0;
            
            Steering();
            HandleAcceleration();
            
            Debug.Log(" y " + mSpeed);

            if (Input.GetKey(KeyCode.Tab)) body.speed *= 3;
            if (Input.GetKey(KeyCode.W)) hQcamera.cameraHeight += 100;
            if (Input.GetKey(KeyCode.S)) hQcamera.cameraHeight -= 100;
        }

        void HandleAcceleration () 
        {
            float vertical = Input.GetAxis("Vertical");

            if (vertical > 0) {
                mSpeed = Accelerate(mSpeed, vertical);
            } else if (vertical < 0) {
                mSpeed = Brake(mSpeed, vertical);
            } else {
                mSpeed = Accelerate(mSpeed, -1);
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

        float Accelerate (float currentSpeed, float vertical) 
        {
            float logSpeed = currentSpeed + (ACCELERATION * Time.fixedDeltaTime * vertical);
            return Mathf.Clamp(logSpeed, 0, MAX_SPEED);
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
