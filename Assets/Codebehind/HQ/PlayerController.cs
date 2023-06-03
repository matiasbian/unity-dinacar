using UnityEngine;

namespace HQ
{
    class PlayerController: MonoBehaviour
    {
        const float STEERING = 0.05f;
        const float ACCELERATION = 8000f;
        const float BRAKE_POWER = 5.5f;
        const float MAX_SPEED = 200;

        // Move log X speed
        const float DIVIDER = 6.1f;


        public HqRenderer hQcamera;
        public ProjectedBody body;
        
        float mSpeed = 0;
        float x = 1;
        private void FixedUpdate()
        {
            body.speed = 0;
            if (Input.GetKey(KeyCode.RightArrow)) body.playerX += STEERING;
            if (Input.GetKey(KeyCode.LeftArrow)) body.playerX -= STEERING;

            HandleAcceleration();
            
            Debug.Log("X: " + x + " y " + mSpeed);

            if (Input.GetKey(KeyCode.Tab)) body.speed *= 3;
            if (Input.GetKey(KeyCode.W)) hQcamera.cameraHeight += 100;
            if (Input.GetKey(KeyCode.S)) hQcamera.cameraHeight -= 100;
        }

        void HandleAcceleration () 
        {
            float vertical = Input.GetAxis("Vertical");
   
            if (vertical > 0)
            {
                x = incX(x, vertical * Time.fixedDeltaTime);
            } 
            else if (vertical < 0)
            {
                x = incX(x, vertical * BRAKE_POWER * Time.fixedDeltaTime);
            }
            else 
            {
                x = incX(x, -1 * Time.fixedDeltaTime);
            }
        

            mSpeed = Accelerate(x);
            body.speed = Mathf.RoundToInt(mSpeed);
        }

        float Accelerate (float x) 
        {
            float logSpeed = Mathf.Log10(x) * ACCELERATION * Time.fixedDeltaTime;
            return Mathf.Min(logSpeed, MAX_SPEED);
        }
        

        float incX (float x, float addition) {
            if (x <= 1 && addition < 0) return x;
            
            return Mathf.Min(x + addition, MAX_SPEED / DIVIDER);
        }
    }
}
