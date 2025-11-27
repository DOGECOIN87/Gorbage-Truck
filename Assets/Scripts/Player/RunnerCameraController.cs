using UnityEngine;
using TrashRunner.Core;

namespace TrashRunner.Player
{
    /// <summary>
    /// Controls the camera to follow the player with smooth movement.
    /// Optionally adjusts field of view based on player speed for enhanced sense of speed.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class RunnerCameraController : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);
        [SerializeField] private float smoothSpeed = 5f;

        [Header("FOV Settings")]
        [SerializeField] private bool useSpeedBasedFOV = true;
        [SerializeField] private float baseFOV = 60f;
        [SerializeField] private float maxFOV = 75f;
        [SerializeField] private float maxSpeedForFOV = 20f;

        [Header("Optional References")]
        [SerializeField] private DifficultyController difficultyController;

        private Camera cameraComponent;
        private Vector3 velocity = Vector3.zero;

        private void Awake()
        {
            // Cache Camera component
            cameraComponent = GetComponent<Camera>();
            
            if (cameraComponent != null)
            {
                cameraComponent.fieldOfView = baseFOV;
            }
        }

        private void LateUpdate()
        {
            if (playerTransform == null)
                return;

            // Calculate target position based on player position and offset
            Vector3 targetPosition = playerTransform.position + offset;

            // Smooth camera movement using SmoothDamp for natural motion
            transform.position = Vector3.SmoothDamp(
                transform.position, 
                targetPosition, 
                ref velocity, 
                1f / smoothSpeed
            );

            // Alternative: Use Lerp for simpler but less natural smoothing
            // transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

            // Adjust FOV based on speed if enabled
            if (useSpeedBasedFOV && cameraComponent != null)
            {
                AdjustFOVBasedOnSpeed();
            }
        }

        /// <summary>
        /// Adjusts the camera's field of view based on the current game speed.
        /// Higher speeds result in wider FOV for enhanced sense of motion.
        /// </summary>
        private void AdjustFOVBasedOnSpeed()
        {
            float currentSpeed = 0f;

            // Get current speed from difficulty controller if available
            if (difficultyController != null)
            {
                currentSpeed = difficultyController.GetCurrentSpeed();
            }

            // Calculate FOV based on speed ratio
            float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeedForFOV);
            float targetFOV = Mathf.Lerp(baseFOV, maxFOV, speedRatio);

            // Smoothly transition to target FOV
            cameraComponent.fieldOfView = Mathf.Lerp(
                cameraComponent.fieldOfView, 
                targetFOV, 
                smoothSpeed * Time.deltaTime
            );
        }

        /// <summary>
        /// Sets the player transform to follow.
        /// </summary>
        public void SetPlayer(Transform player)
        {
            playerTransform = player;
        }

        /// <summary>
        /// Resets the camera to immediately snap to the player position.
        /// </summary>
        public void ResetCamera()
        {
            if (playerTransform != null)
            {
                transform.position = playerTransform.position + offset;
                velocity = Vector3.zero;
                
                if (cameraComponent != null)
                {
                    cameraComponent.fieldOfView = baseFOV;
                }
            }
        }
    }
}
