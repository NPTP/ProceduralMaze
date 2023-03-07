using System;
using Maze;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    [RequireComponent(typeof(InputAction), typeof(Rigidbody))]
    public class PlayerControls : MonoBehaviour
    {
        private const float MAX_TURN_RADIANS_DELTA = 10f;
        private const float MAX_TURN_MAGNITUDE_DELTA = 10f;

        public static event Action<PlayerControls> OnPlayerControlsEnabled; 
        public static event Action<PlayerControls> OnPlayerControlsDisabled; 
        public event Action<PlayerControls> OnInputActionPerformed; 

        [SerializeField] private float playerMoveSpeed;
        [SerializeField] private InputAction playerControlsInputAction;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform facingTransform;

        private Vector3 direction;
        private Vector3 lastNonZeroDirection;

        private void OnValidate()
        {
            if (playerControlsInputAction == null)
            {
                playerControlsInputAction = GetComponent<InputAction>();
            }

            if (rb == null || rb.gameObject != this.gameObject)
            {
                rb = GetComponent<Rigidbody>();
            }
        }

        private void OnEnable()
        {
            MazeGenerator.OnMazeGenerationCompleted += HandleMazeGenerationCompleted;
            MazeGenerator.OnPlayerEnteredEndBlock += HandlePlayerEnteredEndBlock;
        }
        
        private void OnDisable()
        {
            MazeGenerator.OnMazeGenerationCompleted -= HandleMazeGenerationCompleted;
            MazeGenerator.OnPlayerEnteredEndBlock -= HandlePlayerEnteredEndBlock;
            DisablePlayerControls();
        }
        
        private void Update()
        {
            Vector2 playerControlsValue = playerControlsInputAction.ReadValue<Vector2>();
            direction = new Vector3(playerControlsValue.x, 0, playerControlsValue.y).normalized;

            if (lastNonZeroDirection != Vector3.zero)
            {
                facingTransform.forward = Vector3.RotateTowards(facingTransform.forward, lastNonZeroDirection,
                    MAX_TURN_RADIANS_DELTA * Time.deltaTime, MAX_TURN_MAGNITUDE_DELTA * Time.deltaTime);
            }

            if (direction != Vector3.zero)
            {
                lastNonZeroDirection = direction;
            }
        }

        private void FixedUpdate()
        {
            Vector3 newXZVelocity = direction * (playerMoveSpeed * Time.deltaTime);
            rb.velocity = new Vector3(newXZVelocity.x, rb.velocity.y, newXZVelocity.z);
        }

        private void EnablePlayerControls()
        {
            playerControlsInputAction.Enable();
            playerControlsInputAction.performed += HandlePlayerControlsInputActionPerformed;
            OnPlayerControlsEnabled?.Invoke(this);
        }

        private void DisablePlayerControls()
        {
            playerControlsInputAction.Disable();
            playerControlsInputAction.performed -= HandlePlayerControlsInputActionPerformed;
            OnPlayerControlsDisabled?.Invoke(this);
        }
        
        private void HandleMazeGenerationCompleted(MazeGenerator mazeGenerator)
        {
            EnablePlayerControls();
        }
        
        private void HandlePlayerEnteredEndBlock(MazeGenerator mazeGenerator)
        {
            DisablePlayerControls();
        }

        private void HandlePlayerControlsInputActionPerformed(InputAction.CallbackContext context)
        {
            OnInputActionPerformed?.Invoke(this);
        }
    }
}