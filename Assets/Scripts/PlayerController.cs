using Unity.Mathematics;
using UnityEngine;

public enum FacingDirection
{
    left, right
}

public enum PlayerState
{
    idle, walking, jumping, dead, dash
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    private FacingDirection currentDirection = FacingDirection.right;
    public PlayerState currentState = PlayerState.idle;
    public PlayerState previousState = PlayerState.idle;

    [Header("Horizontal")]
    public float maxSpeed = 5f;
    public float accelerationTime = 0.25f;
    public float decelerationTime = 0.15f;
    public float terminalSpeed = 10f;
    public float coyoteJumpTime = 0.25f;
    public float coyoteTimer = 0f;
    public float dashValue = 10f;
    public float dashTimer = 0f;
    public float dashDuration = 1f;
    public float dashResetTime = 3f;

    [Header("Vertical")]
    public float apexHeight = 3f;
    public float apexTime = 0.5f;

    [Header("Ground Checking")]
    public float groundCheckOffset = 0.5f;
    public Vector2 groundCheckSize = new(0.4f, 0.1f);
    public LayerMask groundCheckMask;

    [Header("Wall Checking")]
    public Vector2 wallCheckSize = new(1.3f, 1f);

    private float accelerationRate;
    private float decelerationRate;
    private float gravity;
    private float initialJumpSpeed;

    private bool isGrounded = false;
    private bool isDead = false;
    private bool hasJumped = true;
    private bool onWall = false;
    private bool hasWallJumped = false;

    private Vector2 velocity;
    private Quaternion upright = new Quaternion(0, 0, 0, 0);

    public void Start()
    {
        body.gravityScale = 0;

        accelerationRate = maxSpeed / accelerationTime;
        decelerationRate = maxSpeed / decelerationTime;

        gravity = -2 * apexHeight / (apexTime * apexTime);
        initialJumpSpeed = 2 * apexHeight / apexTime;
    }

    public void Update()
    {
        previousState = currentState;

        CheckForGround();
        CheckForWall();

        Vector2 playerInput = new Vector2();
        playerInput.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.C) && dashTimer > dashResetTime) DashReset();
        if (currentState == PlayerState.dash) DashUpdate();
        dashTimer = dashTimer + 1 * Time.deltaTime;

        if (isDead)
        {
            currentState = PlayerState.dead;
        }

        switch(currentState)
        {
            case PlayerState.dead:
                break;
            case PlayerState.idle:
                if (!isGrounded) currentState = PlayerState.jumping;
                else if (velocity.x != 0) currentState = PlayerState.walking;
                break;
            case PlayerState.walking:
                if (!isGrounded) currentState = PlayerState.jumping;
                else if (velocity.x == 0) currentState = PlayerState.idle;
                break;
            case PlayerState.jumping:
                if (isGrounded)
                {
                    if (velocity.x != 0) currentState = PlayerState.walking;
                    else currentState = PlayerState.idle;
                }
                break;
            case PlayerState.dash:
                if (dashTimer < dashDuration) { }

                else if (isGrounded && velocity.x != 0) currentState = PlayerState.walking;
                else if (isGrounded && velocity.x == 0) currentState = PlayerState.idle;
                else if (!isGrounded) currentState = PlayerState.jumping;
                break;
        }

        MovementUpdate(playerInput);
        JumpUpdate();

        if (!isGrounded && currentState != PlayerState.dash)
        {
            coyoteTimer = coyoteTimer + 1 * Time.deltaTime;
            velocity.y += gravity * Time.deltaTime;
            if (velocity.y < -terminalSpeed) velocity.y = -terminalSpeed;
        }
        else
            velocity.y = 0;

        if (isGrounded == true)
        {
            coyoteTimer = 0;
            hasJumped = false;

            if (transform.rotation != upright)
            {
                transform.rotation = upright;
            }
        }

        body.velocity = velocity;
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        if (currentState != PlayerState.dash)
        {
            if (playerInput.x < 0)
                currentDirection = FacingDirection.left;
            else if (playerInput.x > 0)
                currentDirection = FacingDirection.right;
        }

        if (playerInput.x != 0)
        {
            velocity.x += accelerationRate * playerInput.x * Time.deltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        }
        else
        {
            if (velocity.x > 0)
            {
                velocity.x -= decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Max(velocity.x, 0);
            }
            else if (velocity.x < 0)
            {
                velocity.x += decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Min(velocity.x, 0);
            }
        }
    }

    private void JumpUpdate()
    {
        if (isGrounded && Input.GetButton("Jump") && hasJumped == false || coyoteTimer <= coyoteJumpTime && Input.GetButton("Jump") && hasJumped == false)
        {
            hasJumped = true;
            hasWallJumped = false;
            velocity.y = initialJumpSpeed;
            isGrounded = false;

            print($"In coyote - {isGrounded}");
        }
        else if (onWall && Input.GetButton("Jump") && hasWallJumped == false)
        {
            hasWallJumped = true;
            velocity.x = -velocity.x;
            velocity.y = initialJumpSpeed;
            isGrounded = false;

            print($"In wall - {isGrounded}");
        }
    }
    private void DashReset()
    {
        currentState = PlayerState.dash;
        dashTimer = 0;
    }
    private void DashUpdate()
    {
        if (currentDirection == FacingDirection.right)
        {
            transform.position = transform.position + Vector3.right * dashValue * Time.deltaTime;
        }
        else
        {
            transform.position = transform.position + Vector3.left * dashValue * Time.deltaTime;
        }
    }


    private void CheckForGround()
    {
        isGrounded = Physics2D.OverlapBox(transform.position + Vector3.down * groundCheckOffset, groundCheckSize, 0, groundCheckMask);
        print($"In Check - {isGrounded}");
    }

    private void CheckForWall()
    {
        onWall = Physics2D.OverlapBox(transform.position, wallCheckSize, 0, groundCheckMask);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.down * groundCheckOffset, groundCheckSize);
        Gizmos.DrawWireCube(transform.position, wallCheckSize);
    }

    public bool IsWalking()
    {
        return velocity.x != 0;
    }
    public bool IsGrounded()
    {
        return isGrounded;
    }

    public FacingDirection GetFacingDirection()
    {
        return currentDirection;
    }
}
