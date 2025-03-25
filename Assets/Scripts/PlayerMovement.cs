using UnityEngine;

///<summary>
/// This script handles the players movement
/// Richard Ho A01349477
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// The active speed for the player movement (calculated by state).
    /// </summary>
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
    /// Handles movement and applies custom gravity.
    /// </summary>
    private void FixedUpdate()
    {
        MovePlayer();
        ApplyJumpGravity();
    }

    /// <summary>
    /// Unity's built-in Update method.  
    /// Checks if the player is grounded, processes input, handles speed limits, and updates states.
    /// </summary>
    private void Update()
    {
        isGrounded = Physics.Raycast(rb.position, Vector3.down, whatIsGround);
        Debug.Log(rb.position);
        MyInput();
        SpeedControl();
        StateHandler();

        // handle drag
        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }

    /// <summary>
    /// Reads user input for movement, jumping, and crouching.
    /// </summary>
    private void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouching
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouching
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    /// <summary>
    /// Determines the player's current movement state (walking, sprinting, crouching, or air).
    /// Sets the appropriate movement speed.
    /// </summary>
    private void StateHandler()
    {
        // Mode - Crouching
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            movementSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        if (isGrounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            movementSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (isGrounded)
        {
            state = MovementState.walking;
            movementSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
        }
    }

    /// <summary>
    /// Applies force to the player Rigidbody for movement on ground or in air.
    /// If grounded, applies greater force; if in air, uses the air multiplier.
    /// </summary>
    private void MovePlayer()
    {
        // calculate movement direction
        // This ensures you always move in the direction you are facing
        movementDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // this applies movement to the player body
        if (isGrounded)
        {
            rb.AddForce(movementDirection.normalized * movementSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            rb.AddForce(movementDirection.normalized * movementSpeed * airMultiplier, ForceMode.Force);
        }

        // if player is grounded and not moving, stop them from sliding
        if (movementSpeed == 0 && horizontalInput == 0)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    /// <summary>
    /// Restricts the player's velocity to the current movement speed so they cannot exceed their max speed.
    /// </summary>
    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // if the player is moving faster than the movement speed, limit it to their max speed
        if (flatVelocity.magnitude > movementSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * movementSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }

    /// <summary>
    /// Executes the jump by resetting vertical velocity and applying an upward force.
    /// </summary>
    private void Jump()
    {
        // make sure y velocity is 0
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); // Use ForceMode.Impulse because it applies force immediately
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
    /// </summary>
    private void ApplyJumpGravity()
    {
        // Fall: apply our fall multiplier with unity's built in gravity
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
