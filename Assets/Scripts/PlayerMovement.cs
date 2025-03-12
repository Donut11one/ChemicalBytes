using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float movementSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag = 7;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float fallMultiplier;
    public float ascendingMultiplier;

    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool isGrounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 movementDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        ApplyJumpGravity();
    }
    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f, whatIsGround);
        MyInput();
        SpeedControl();
        StateHandler();

        //handle drag
        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }

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

        //// if in air, add force with air multiplier
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

    private void Jump()
    {
        // make sure y velocity is 0
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); // Use forcemode.impulse b/c permitting 1 force at a time


    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void ApplyJumpGravity()
    {
        // Fall: apply our fall multiplier with unity's built in gravity
        if (rb.linearVelocity.y <0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * ascendingMultiplier * Time.fixedDeltaTime;
        }
    }
}
