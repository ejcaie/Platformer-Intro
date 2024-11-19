using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    public float apexHeight;
    public float apexTime;
    public float speed;
    private float jumpVelocity;
    private float gravity;
    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.
        Vector2 playerInput = new Vector2();
        playerInput.x = Input.GetAxis("Horizontal") * speed;
        MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        transform.position = transform.position + (Vector3)playerInput;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up.normalized * apexHeight);       
        }
        IsWalking();
        GetFacingDirection();
    }

    public bool IsWalking()
    {
        if (Input.GetAxis("Horizontal") != 0) return true;
        else return false;
    }
    public bool IsGrounded()
    {
        
        return false;
    }

    public FacingDirection GetFacingDirection()
    {
        if (Input.GetAxis("Horizontal") > 0) return FacingDirection.right;
        if (Input.GetAxis("Horizontal") < 0) return FacingDirection.left;
        else return FacingDirection.

    }
}
