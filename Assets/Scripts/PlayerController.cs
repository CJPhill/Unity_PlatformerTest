using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{


    [Header("X Movement")]
    public float speed = 10;
    public float acceleration = 1f;
    public float deceleration = 1f;
    public float velPower = 1f;
    public float frictionAmount = 1f;

    [Header("Y Movement")]
    public float jumpVelocity = 15;
    public float fallMultiplier = 6f;
    public float jumpCutMultiplier = 4f;

    [Header("Jump Buffering")]
    public float jumpBufferTime;
    public float jumpCoyoteTime;

    //Timers for Jumps
    private float lastGroundedTime;
    private float lastJumpTime;
    
    //Bools for jumping
    private bool isGrounded;
    private bool isJumping;
    private bool jumpInputReleased;

    //Info for wallslide
    [Header("WallSlideInfo")]
    public float wallSlidingSpeed = 1;
    private bool isWalled;
    private bool isWallSliding;

    //Info for wallJump
    private bool isWallJumping; //check for a wall jump
    private float wallJumpingDirection; //the direction the wall jump will occur
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter; //Timer for wall jumping
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f); //power that the wall jump will be at

    [Header("Dashing")]
    public float dashingVelocity = 14f;
    public float dashingTime = 0.5f;
    private Vector2 dashingDir;
    private bool isDashing;
    private bool canDash = true;

    
    private Rigidbody2D rb;
    private TrailRenderer trailRenderer;
    private SpriteRenderer spriteRenderer;

    //Input Manager
    [Header("Player Inputs")]
    [SerializeField]
    private InputActionReference move, jump;

    public RoomManager roomManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        roomManager = GameObject.FindObjectOfType<RoomManager>();
    }


    private void Update()
    {

        
        //Better Jumping Physics
        if (rb.velocity.y < 0 )
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } 
        else if (rb.velocity.y > 0 && !jump.action.inProgress)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (jumpCutMultiplier - 1) * Time.deltaTime;
        }
        else if (isDashing && !isJumping)
        {
            rb.velocity = Vector2.zero;
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        
        

        
    }

    private void FixedUpdate()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        Vector2 dir = new Vector2 (inputX, inputY);

        Walk(dir);
        friction();
        WallSlide();
        Timer();
        DashIndicator();


        

    }

    public void Walk(Vector2 dir)
    {
        float targetSpeed = dir.x * speed;
        float speedDiff = targetSpeed - rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);
        rb.AddForce(movement * Vector2.right);
    }

    public void friction()
    {
        if (isGrounded && !(move.action.inProgress)) 
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmount));
            amount *= Mathf.Sign(rb.velocity.x);
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }

    
    public void OnJump()
    { 
        if (lastGroundedTime > 0 && lastJumpTime > 0 && !isJumping)
        //if (isGrounded) 
        {
            lastJumpTime = jumpBufferTime;
            Debug.Log("Your jumping");
            rb.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
            lastGroundedTime = 0;
            lastJumpTime = 0;
            isJumping = true;
            jumpInputReleased = false;
            
            
        }
        else
        {
            Debug.Log("No double jump");
        }
        

    }

    private void OnDash()
    {
        if (canDash && !jump.action.IsInProgress()){
            isDashing = true;
            canDash = false;
            trailRenderer.emitting = true;
            rb.velocity = Vector2.zero;
            dashingDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (dashingDir == Vector2.zero)
            {
                dashingDir = new Vector2(transform.localScale.x, 0);
            }
            StartCoroutine(StopDashing());
        }
        if (isDashing)
        {
            rb.velocity = dashingDir.normalized * dashingVelocity;
            return;
        }
    }

    private void Timer()
    {
        lastGroundedTime += Time.deltaTime;
        lastJumpTime += Time.deltaTime;
    }

    
    /*private bool isWalled()
    {
        return true;
        //Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }
    */

    private void WallSlide()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (isWalled && !isGrounded && (horizontal != 0f))
        {
            Debug.Log("Your sliding dumbass");
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            //Debug.Log("isWalled = " + isWalled + " isGrounded = " + isGrounded + " horizontal = " + horizontal);
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }
        //Add more if statements over here
    }

    private void DashIndicator()
    {
        if (canDash)
        {
            spriteRenderer.color = Color.green;
        }
        else
        {
            spriteRenderer.color = Color.red;
        }
    }


    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashingTime);
        trailRenderer.emitting = false;
        isDashing = false;
    }

    //Wallsliding and rejection of double jumping with OnCollisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) 
        {
            //Debug.Log("Touching Ground");
            isGrounded = true;
            isJumping = false;
            lastGroundedTime = jumpCoyoteTime;
            canDash = true;
        }
        if (collision.gameObject.CompareTag("Hazard"))
        {
            GameManager.Instance.roomManager.RespawnPlayer();
            Debug.Log("Touching Hazard");

        }
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            isWalled = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //Leads to a double dash if you dash from Ground
        /*
        if (collision.gameObject.CompareTag("Ground")){
            canDash = true;
        }
        */

        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("touching wall");
            isWalled = true;
           
        }
        else
        {
            //Debug.Log("Not touching wall");
            isWalled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("WinTile"))
        {
            Debug.Log("You Won!");
            GameManager.Instance.ShowWin();
        }
    }

    //Quiting the game
    private void OnQuit(InputValue inputValue)
    {
        GameManager.Instance.ShowPause();
    }
    






}
