using BlockAndGun.Player.Weapon;
using UnityEngine;

namespace BlockAndGun.Player.Input
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 4.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 6.0f;
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;
        [Tooltip("Crouching height")]
        public float CrouchingHeight = 0;
        [Tooltip("Crouching speed")]
        public float CrouchSpeed = 2.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;

        private PlayerInput playerInput;
        public PlayerInput.PlayerActions playerActions;

        private ActiveWeapon activeWeapon;
        private CharacterController characterController;
        private Vector3 playerVelocity;

        private float speed;
        private float standingHeight;
        private float crouchTimer;
        private float rotationVelocity;
        private float cinemachineTargetPitch;

        private bool isCrouching;
        private bool lerpCrouch;

        void Awake()
        {
            characterController = GetComponent<CharacterController>();
            activeWeapon = GetComponentInChildren<ActiveWeapon>();

            playerInput = new PlayerInput();

            playerActions = playerInput.Player;
            playerActions.Jump.performed += ctx => Jump();
            playerActions.Crouch.performed += ctx => Crouch();
            playerActions.Shoot.performed += ctx => activeWeapon.HandleShoot();
            playerActions.Reload.performed += ctx => activeWeapon.HandleReload();
            playerActions.SwitchWeapon.performed += ctx => activeWeapon.SwitchWeapon(playerActions.SwitchWeapon.ReadValue<Vector2>());

        }

        private void Start()
        {
            standingHeight = characterController.height;
            speed = MoveSpeed;

            SetCursorState(true);
        }

        void Update()
        {
            GroundedCheck();
            ProcessCrouch();

            //Dis au playerMotor de bouger en utilisant la valeur des inputs de Mouvement
            ProcessMove(playerActions.Move.ReadValue<Vector2>());
        }

        private void LateUpdate()
        {
            ProcessLook(playerActions.Look.ReadValue<Vector2>());
        }


        //Recois les informations de InputManager et les applique au personnage pour qu'il puisse bouger
        public void ProcessMove(Vector2 input)
        {
            Vector3 moveDirection = transform.TransformDirection(new Vector3(input.x, 0, input.y));

            if (Grounded && playerVelocity.y < 0)
                playerVelocity.y = -2f; // Évite l'accumulation de vélocité négative

            playerVelocity.y += Gravity * Time.deltaTime;

            Vector3 totalMovement = (speed * moveDirection) + playerVelocity;

            characterController.Move(totalMovement * Time.deltaTime);

        }
        public void ProcessLook(Vector2 input)
        {
            float mouseX = input.x;
            float mouseY = input.y;

            float deltaTimeMultiplier = 1.0f;

            cinemachineTargetPitch += mouseY * RotationSpeed * deltaTimeMultiplier;

            rotationVelocity = mouseX * RotationSpeed * deltaTimeMultiplier;

            // clamp our pitch rotation
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

            // Update Cinemachine camera target pitch
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * rotationVelocity);
        }

        public void Jump()
        {
            if (Grounded)
            {
                playerVelocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }
        }

        public void Crouch()
        {
            isCrouching = !isCrouching;
            crouchTimer = 0;
            lerpCrouch = true;
        }
        private void ProcessCrouch()
        {
            if (lerpCrouch)
            {
                crouchTimer += Time.deltaTime;
                float p = crouchTimer / 1;
                p += p;

                if (isCrouching)
                {
                    characterController.height = Mathf.Lerp(characterController.height, CrouchingHeight, p);
                    speed = CrouchSpeed;

                }
                else
                {
                    characterController.height = Mathf.Lerp(characterController.height, standingHeight, p);
                    speed = MoveSpeed;

                }

                if (Mathf.Approximately(characterController.height, isCrouching ? CrouchingHeight : standingHeight))
                {
                    lerpCrouch = false;
                    crouchTimer = 0f;
                }
            }
        }
        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnEnable()
        {
            playerActions.Enable();
        }

        private void OnDisable()
        {
            playerActions.Disable();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        public void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}