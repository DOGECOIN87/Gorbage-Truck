using UnityEngine;
using TrashRunner.Core;

namespace TrashRunner.Player
{
    /// <summary>
    /// Controls the player's movement in the endless runner game.
    /// Handles lane switching, jumping, sliding, and forward movement with difficulty scaling.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerRunnerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float baseForwardSpeed = 5f;
        [SerializeField] private float laneWidth = 3f;
        [SerializeField] private float laneChangeSpeed = 10f;

        [Header("Jump & Gravity")]
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private float gravity = -20f;

        [Header("Slide Settings")]
        [SerializeField] private float slideDuration = 1f;

        [Header("References")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private DifficultyController difficultyController;
        [SerializeField] private PlayerInputController playerInputController;
        [SerializeField] private AudioManager audioManager;

        // Lane tracking
        private int currentLaneIndex = 1; // 0 = left, 1 = center, 2 = right
        private Vector3 targetLanePosition;

        // Vertical movement
        private float verticalVelocity;
        private bool isGrounded;

        // Slide state
        private bool isSliding;
        private float slideTimer;
        private float originalHeight;
        private Vector3 originalCenter;

        // Distance tracking
        private float cumulativeDistance;
        private const float DISTANCE_REPORT_THRESHOLD = 0.5f;

        private void Awake()
        {
            // Cache CharacterController dimensions for slide restoration
            if (characterController != null)
            {
                originalHeight = characterController.height;
                originalCenter = characterController.center;
            }
        }

        private void OnEnable()
        {
            // Subscribe to input events
            if (playerInputController != null)
            {
                playerInputController.OnMoveLeft += HandleMoveLeft;
                playerInputController.OnMoveRight += HandleMoveRight;
                playerInputController.OnJump += HandleJump;
                playerInputController.OnSlide += HandleSlide;
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from input events
            if (playerInputController != null)
            {
                playerInputController.OnMoveLeft -= HandleMoveLeft;
                playerInputController.OnMoveRight -= HandleMoveRight;
                playerInputController.OnJump -= HandleJump;
                playerInputController.OnSlide -= HandleSlide;
            }
        }

        private void Update()
        {
            if (gameManager == null || gameManager.CurrentState != GameManager.RunState.Running)
                return;

            // Calculate current speed from difficulty
            float currentSpeed = baseForwardSpeed;
            if (difficultyController != null)
            {
                currentSpeed = difficultyController.GetCurrentSpeed();
            }

            // Forward movement
            Vector3 forwardMovement = transform.forward * currentSpeed * Time.deltaTime;

            // Lane position lerping
            float targetX = (currentLaneIndex - 1) * laneWidth; // -laneWidth, 0, laneWidth
            targetLanePosition = new Vector3(targetX, transform.position.y, transform.position.z);
            
            float currentX = Mathf.Lerp(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime);
            Vector3 horizontalMovement = new Vector3(currentX - transform.position.x, 0, 0);

            // Apply gravity
            isGrounded = characterController.isGrounded;
            if (isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = -2f; // Small downward force to keep grounded
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }

            Vector3 verticalMovement = new Vector3(0, verticalVelocity * Time.deltaTime, 0);

            // Handle slide timer
            if (isSliding)
            {
                slideTimer -= Time.deltaTime;
                if (slideTimer <= 0)
                {
                    EndSlide();
                }
            }

            // Combine all movement and apply via CharacterController
            Vector3 totalMovement = forwardMovement + horizontalMovement + verticalMovement;
            characterController.Move(totalMovement);

            // Track distance and report to ScoreManager
            float distanceDelta = currentSpeed * Time.deltaTime;
            cumulativeDistance += distanceDelta;

            if (cumulativeDistance >= DISTANCE_REPORT_THRESHOLD)
            {
                if (scoreManager != null)
                {
                    scoreManager.AddDistance(cumulativeDistance);
                }
                cumulativeDistance = 0f;
            }
        }

        // Input handlers
        private void HandleMoveLeft()
        {
            if (currentLaneIndex > 0)
            {
                currentLaneIndex--;
            }
        }

        private void HandleMoveRight()
        {
            if (currentLaneIndex < 2)
            {
                currentLaneIndex++;
            }
        }

        private void HandleJump()
        {
            if (isGrounded && !isSliding)
            {
                verticalVelocity = jumpForce;
                
                if (audioManager != null)
                {
                    audioManager.PlayJump();
                }
            }
        }

        private void HandleSlide()
        {
            if (isGrounded && !isSliding)
            {
                StartSlide();
                
                if (audioManager != null)
                {
                    audioManager.PlaySlide();
                }
            }
        }

        /// <summary>
        /// Initiates a slide by reducing the character's height.
        /// </summary>
        private void StartSlide()
        {
            isSliding = true;
            slideTimer = slideDuration;

            // Reduce character controller height for sliding
            characterController.height = originalHeight * 0.5f;
            characterController.center = new Vector3(originalCenter.x, originalCenter.y * 0.5f, originalCenter.z);
        }

        /// <summary>
        /// Ends the slide and restores the character's original height.
        /// </summary>
        private void EndSlide()
        {
            isSliding = false;
            
            // Restore original dimensions
            characterController.height = originalHeight;
            characterController.center = originalCenter;
        }

        /// <summary>
        /// Resets the player to the starting position and state.
        /// Called by GameManager when starting a new run.
        /// </summary>
        public void ResetToStart()
        {
            currentLaneIndex = 1; // Center lane
            verticalVelocity = 0f;
            cumulativeDistance = 0f;
            
            if (isSliding)
            {
                EndSlide();
            }

            // Reset position
            SetLane(1);
        }

        /// <summary>
        /// Immediately sets the player to a specific lane.
        /// </summary>
        public void SetLane(int laneIndex)
        {
            currentLaneIndex = Mathf.Clamp(laneIndex, 0, 2);
            float targetX = (currentLaneIndex - 1) * laneWidth;
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
        }

        // Public properties for external access
        public bool IsGrounded => isGrounded;
        public bool IsSliding => isSliding;
        public int CurrentLane => currentLaneIndex;
    }
}
