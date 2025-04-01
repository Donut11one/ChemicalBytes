using UnityEngine;

///<summary>
/// This script handles the players movement
/// Richard Ho A01349477
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float movementSpeed;

    // Base walking speed.
    public float walkSpeed;
    // Base sprinting speed.
    public float sprintSpeed;
    // Drag applied to the Rigidbody when grounded.
    public float groundDrag = 7;

    [Header("Jumping")]
    // Force applied to the player when jumping.
    public float jumpForce;
    // Cooldown time (seconds) before the player can jump again.
    public float jumpCooldown;
    // Multiplier applied when the player is in the air.
    public float airMultiplier;
    // Extra force multiplier applied to increase downward speed.
    public float fallMultiplier;
    // Extra force multiplier applied when ascending.
    public float ascendingMultiplier;
    // True if the player can currently jump.
    bool readyToJump;

    [Header("Crouching")]
    // Movement speed when crouching.
    public float crouchSpeed;
    // Y-scale of the player while crouched (for lowering the player height).
    public float crouchYScale;
    // The original Y-scale of the player before crouching.
    private float startYScale;

    [Header("Keybinds")]
    // Key used for jumping (default = Space).
    public KeyCode jumpKey = KeyCode.Space;
    // Key used for sprinting (default = Left Shift).
    public KeyCode sprintKey = KeyCode.LeftShift;
    // Key used for crouching (default = Left Control).
    public KeyCode crouchKey = KeyCode.LeftControl;
    // Toggle key for flying mode.
    public KeyCode flyToggleKey = KeyCode.M;

    [Header("Ground Check")]
    // The approximate height of the player, used for ground raycast checks.
    public float playerHeight;
    // Layer(s) considered "ground" for the player.
    public LayerMask whatIsGround;
    // True if the player is currently grounded.
    bool isGrounded;

    // Reference to the player's orientation transform (used for movement direction).
    public Transform orientation;
    // Horizontal input value from -1 to 1.
    float horizontalInput;
    // Vertical input value from -1 to 1.
    float verticalInput;
    // Vector representing the intended movement direction.
    Vector3 movementDirection;
    // Reference to the attached Rigidbody component.
    Rigidbody rb;
    // The current movement state of the player.
    public MovementState state;

    // New variables for flying mode.
    public float flySpeed = 5f;  // Constant rate for flying movement.
    private bool isFlying = false;
    private float flyVertical = 0f; // Vertical input for flying (1 for up, -1 for down).

    /// <summary>
    /// Enumeration of all possible movement states.
    /// </summary>
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    /// <summary>
    /// Unity's built-in Start method. Initializes values and freezes rotation on the Rigidbody.
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    /// <summary>
    /// Unity's built-in FixedUpdate method, called on physics ticks.
    /// Handles movement and applies custom gravity when not flying.
    /// </summary>
    private void FixedUpdate()
    {
        MovePlayer();
        if (!isFlying)
        {
            ApplyJumpGravity();
        }
    }

    /// <summary>
    /// Unity's built-in Update method.
    /// Checks if the player is grounded, processes input, handles speed limits, and updates states.
    /// Also toggles between fly and walking modes.
    /// </summary>
    private void Update()
    {
        // Toggle fly mode when M is pressed.
        if (Input.GetKeyDown(flyToggleKey))
        {
            isFlying = !isFlying;
            rb.useGravity = !isFlying; // Disable built-in gravity in fly mode.
            Debug.Log("Fly mode: " + isFlying);
            // Reset vertical velocity when switching modes.
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        }

        // Ground check is not needed in fly mode, but still used for walking.
        if (!isFlying)
            isGrounded = Physics.Raycast(rb.position, Vector3.down, playerHeight, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // Handle drag (only for walking)
        if (!isFlying)
        {
            if (isGrounded)
            {
                rb.linearDamping = groundDrag;
            }
            else
            {
                rb.linearDamping = 0;
            }
        }
    }

    /// <summary>
    /// Reads user input for movement. Processes different inputs for fly mode vs walking.
    /// </summary>
    private void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (isFlying)
        {
            // In fly mode, space moves up and left control moves down.
            flyVertical = 0f;
            if (Input.GetKey(KeyCode.Space))
                flyVertical = 1f;
            if (Input.GetKey(KeyCode.LeftControl))
                flyVertical = -1f;
        }
        else
        {
            // Walking mode: handle jump and crouch.
            if (Input.GetKey(jumpKey) && readyToJump && isGrounded)
            {
                readyToJump = false;
                Jump();
                Invoke(nameof(ResetJump), jumpCooldown);
            }

            if (Input.GetKeyDown(crouchKey))
            {
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            }

            if (Input.GetKeyUp(crouchKey))
            {
                transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            }
        }
    }

    /// <summary>
    /// Determines the player's current movement state (walking, sprinting, crouching, or air)
    /// and sets the appropriate movement speed. (Only used for walking mode.)
    /// </summary>
    private void StateHandler()
    {
        if (!isFlying)
        {
            if (Input.GetKey(crouchKey))
            {
                state = MovementState.crouching;
                movementSpeed = crouchSpeed;
            }
            else if (isGrounded && Input.GetKey(sprintKey))
            {
                state = MovementState.sprinting;
                movementSpeed = sprintSpeed;
            }
            else if (isGrounded)
            {
                state = MovementState.walking;
                movementSpeed = walkSpeed;
            }
            else
            {
                state = MovementState.air;
            }
        }
    }

    /// <summary>
    /// Moves the player. In walking mode, applies forces based on input.
    /// In fly mode, directly sets the velocity to a constant rate based on input.
    /// </summary>
    private void MovePlayer()
    {
        if (isFlying)
        {
            // Use the camera's forward and right for movement.
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;

            // Calculate movement based on input:
            // - verticalInput (W/S) and horizontalInput (A/D) follow the camera angle.
            // - flyVertical (set by Space and Left Control) adds independent vertical movement.
            Vector3 flyMovement = (cameraForward * verticalInput) + (cameraRight * horizontalInput) + (Vector3.up * flyVertical);

            // Set velocity directly for a constant movement rate.
            rb.linearVelocity = flyMovement.normalized * flySpeed;
        }
        else
        {
            // Walking mode: use the orientation (usually constrained to horizontal rotation).
            movementDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
            if (isGrounded)
            {
                rb.AddForce(movementDirection.normalized * movementSpeed * 10f, ForceMode.Force);
            }
            else
            {
                rb.AddForce(movementDirection.normalized * movementSpeed * airMultiplier, ForceMode.Force);
            }

            // If the player is grounded and not moving horizontally, stop sliding.
            if (movementSpeed == 0 && horizontalInput == 0)
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
        }
    }


    /// <summary>
    /// Restricts the player's horizontal velocity to the current movement speed so they cannot exceed their max speed.
    /// </summary>
    private void SpeedControl()
    {
        if (!isFlying)
        {
            Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (flatVelocity.magnitude > movementSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * movementSpeed;
                rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
            }
        }
    }

    /// <summary>
    /// Executes the jump by resetting vertical velocity and applying an upward force.
    /// </summary>
    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Resets jump availability, allowing the player to jump again.
    /// </summary>
    private void ResetJump()
    {
        readyToJump = true;
    }

    /// <summary>
    /// Applies custom gravity multipliers for smoother jumping and falling.
    /// (Not used in fly mode.)
    /// </summary>
    private void ApplyJumpGravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * ascendingMultiplier * Time.fixedDeltaTime;
        }
    }
}
