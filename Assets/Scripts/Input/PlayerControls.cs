using System;
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
            playerControlsInputAction.Enable();
            playerControlsInputAction.performed += HandlePlayercontrolsInputActionPerformed;
            OnPlayerControlsEnabled?.Invoke(this);
        }

        private void OnDisable()
        {
            playerControlsInputAction.Disable();
            playerControlsInputAction.performed -= HandlePlayercontrolsInputActionPerformed;
            OnPlayerControlsDisabled?.Invoke(this);
        }

        private void HandlePlayercontrolsInputActionPerformed(InputAction.CallbackContext context)
        {
            OnInputActionPerformed?.Invoke(this);
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
            rb.velocity = direction * (playerMoveSpeed * Time.deltaTime);
        }
    }
}