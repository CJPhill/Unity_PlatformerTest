using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class CharController : MonoBehaviour
{
    private float xSpeed;
    [SerializeField]
    private float ySpeed;
    private float inputX;
    private float inputY;
    private Vector2 dir;
    private Vector2 pos;
    private Vector2 desiredVelocity;
    private Vector2 velocity;
    [SerializeField]
    private bool isGrounded;
    private Rigidbody2D body;


    [Header("X Physics")]
    public float maxSpeed;
    public float acceleration;
    public float deceleration;
    public float turnSpeed;
    public float maxSpeedChange;

    [Header("Y Physics")]
    public float jumpSpeed;
    public float gravity;
    public float maxGravity;

    [SerializeField]
    private InputActionReference move, jump;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 dir = new Vector2(inputX, inputY);
        desiredVelocity = new Vector2(dir.x, 0f) * maxSpeed;
        xSpeed = dir.x;

    }

    private void FixedUpdate()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
        Vector2 pos = transform.position;

        //gravityForce();
        jumpFunc();
        speedCheckX();

        pos.x += xSpeed;
        pos.y += ySpeed;

        transform.position = pos;


        //Work on the X movement
        if (dir.x != 0)
        {
            if (Mathf.Sign(dir.x) != Mathf.Sign(velocity.x))
            {
                maxSpeedChange = turnSpeed * Time.deltaTime;
            }
            else
            {
                maxSpeedChange = acceleration * Time.deltaTime;
            }
        }
        else
        {
            maxSpeedChange = deceleration * Time.deltaTime;
        }

        xSpeed = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        
    }

    private void OnMove()
    {
        Debug.Log("you moved");
    }

    public void OnJump()
    {
        Debug.Log("you jumped");

    }

    private void jumpFunc()
    {
        if (isGrounded && jump.action.inProgress)
        {
            
            //ySpeed = 0f;
            ySpeed = jumpSpeed;
            //isGrounded = false;
            
        }
        else if (isGrounded)
        {
            //ySpeed = 0f;
        }
        else if (!isGrounded)
        {
            ySpeed += gravity;
            if (ySpeed < maxGravity)
            {
                ySpeed = maxGravity;
            }
        }
    }

    /*
    private void gravityForce()
    {
        if (isGrounded)
        {
            ySpeed = 0f;
        }
        else if (!isGrounded)
        {
            ySpeed += gravity;
            if (ySpeed < maxGravity)
            {
                ySpeed = maxGravity;
            }
        }
    }
    */

    private void speedCheckX()
    {
        if (xSpeed > maxSpeed)
        {
            xSpeed = maxSpeed;
        }
        else if (xSpeed < -maxSpeed)
        {
            xSpeed = -maxSpeed;
        }
    }

    



    //Ground + Wall collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall"))
        {
            
            isGrounded = true;
            ySpeed = 0f;
            Debug.Log("you hit ground");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall"))
        {
            isGrounded = false;
            Debug.Log("you Left the ground");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("On the wall");
            
        }
    }

    /*
     * Notes from lecture to maybe be included
     * 
     * //Debug.DrawRay(pos, Vector3.down, Color.red, 10.0f);
        /*
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, 0.5f + ySpeed);
        
        if (!hit)
        {
            pos.y += ySpeed;
        }
        else
        {
            ySpeed = 0;
            Debug.Log(hit.collider.name);
            pos.y = hit.point.y + 0.5f;
        }
      */
}
