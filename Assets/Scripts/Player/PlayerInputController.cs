using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace TrashRunner.Player
{
    /// <summary>
    /// Handles player input from both keyboard and touch/swipe gestures.
    /// Translates input into gameplay events for lane movement, jumping, and sliding.
    /// </summary>
    public class PlayerInputController : MonoBehaviour
    {
        // Events for player actions
        public event Action OnMoveLeft;
        public event Action OnMoveRight;
        public event Action OnJump;
        public event Action OnSlide;

        [Header("Input Settings")]
        [SerializeField] private InputActionAsset runnerInputAsset;

        [Header("Swipe Settings")]
        [SerializeField] private float minSwipeDistance = 50f;
        [SerializeField] private float maxSwipeTime = 1f;
        [SerializeField] private float directionThreshold = 0.5f;

        // Input actions
        private InputAction moveLeftAction;
        private InputAction moveRightAction;
        private InputAction jumpAction;
        private InputAction slideAction;
        private InputActionMap gameplayMap;

        // Touch/swipe tracking
        private Vector2 swipeStartPos;
        private float swipeStartTime;
        private bool isSwipeInProgress;

        private void Awake()
        {
            // Get the Gameplay action map
            gameplayMap = runnerInputAsset.FindActionMap("Gameplay");

            // Retrieve actions by name
            moveLeftAction = gameplayMap.FindAction("MoveLeft");
            moveRightAction = gameplayMap.FindAction("MoveRight");
            jumpAction = gameplayMap.FindAction("Jump");
            slideAction = gameplayMap.FindAction("Slide");
        }

        private void OnEnable()
        {
            // Subscribe to action callbacks
            if (moveLeftAction != null)
            {
                moveLeftAction.performed += OnMoveLeftPerformed;
                moveLeftAction.Enable();
            }

            if (moveRightAction != null)
            {
                moveRightAction.performed += OnMoveRightPerformed;
                moveRightAction.Enable();
            }

            if (jumpAction != null)
            {
                jumpAction.performed += OnJumpPerformed;
                jumpAction.Enable();
            }

            if (slideAction != null)
            {
                slideAction.performed += OnSlidePerformed;
                slideAction.Enable();
            }

            // Enable the action map
            gameplayMap?.Enable();
        }

        private void OnDisable()
        {
            // Unsubscribe from action callbacks
            if (moveLeftAction != null)
            {
                moveLeftAction.performed -= OnMoveLeftPerformed;
                moveLeftAction.Disable();
            }

            if (moveRightAction != null)
            {
                moveRightAction.performed -= OnMoveRightPerformed;
                moveRightAction.Disable();
            }

            if (jumpAction != null)
            {
                jumpAction.performed -= OnJumpPerformed;
                jumpAction.Disable();
            }

            if (slideAction != null)
            {
                slideAction.performed -= OnSlidePerformed;
                slideAction.Disable();
            }

            // Disable the action map
            gameplayMap?.Disable();
        }

        private void Update()
        {
            // Handle touch swipe detection for mobile input
            DetectSwipe();
        }

        // Input action callbacks
        private void OnMoveLeftPerformed(InputAction.CallbackContext context)
        {
            OnMoveLeft?.Invoke();
        }

        private void OnMoveRightPerformed(InputAction.CallbackContext context)
        {
            OnMoveRight?.Invoke();
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            OnJump?.Invoke();
        }

        private void OnSlidePerformed(InputAction.CallbackContext context)
        {
            OnSlide?.Invoke();
        }

        /// <summary>
        /// Detects swipe gestures on touch devices and translates them to movement commands.
        /// </summary>
        private void DetectSwipe()
        {
            // Only process touch input on mobile devices
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        swipeStartPos = touch.position;
                        swipeStartTime = Time.time;
                        isSwipeInProgress = true;
                        break;

                    case TouchPhase.Ended:
                        if (isSwipeInProgress)
                        {
                            ProcessSwipe(touch.position);
                            isSwipeInProgress = false;
                        }
                        break;

                    case TouchPhase.Canceled:
                        isSwipeInProgress = false;
                        break;
                }
            }
        }

        /// <summary>
        /// Processes a completed swipe gesture and invokes the appropriate event.
        /// </summary>
        private void ProcessSwipe(Vector2 endPos)
        {
            float swipeTime = Time.time - swipeStartTime;
            
            // Check if swipe was within time limit
            if (swipeTime > maxSwipeTime)
                return;

            Vector2 swipeDelta = endPos - swipeStartPos;
            float swipeDistance = swipeDelta.magnitude;

            // Check if swipe distance meets minimum threshold
            if (swipeDistance < minSwipeDistance)
                return;

            // Normalize the swipe direction
            Vector2 swipeDirection = swipeDelta.normalized;

            // Determine primary direction based on threshold
            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
            {
                // Horizontal swipe
                if (swipeDirection.x > directionThreshold)
                {
                    OnMoveRight?.Invoke();
                }
                else if (swipeDirection.x < -directionThreshold)
                {
                    OnMoveLeft?.Invoke();
                }
            }
            else
            {
                // Vertical swipe
                if (swipeDirection.y > directionThreshold)
                {
                    OnJump?.Invoke();
                }
                else if (swipeDirection.y < -directionThreshold)
                {
                    OnSlide?.Invoke();
                }
            }
        }
    }
}
