using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxXSpeed;
    public float maxYSpeed;
    public float accel;
    public float decel;
    public float stopRange = 0.1f;
    public float xSpeed = 0f;
    public float ySpeed = 0f;
    public float groundedCheckDis = 0.05f;
    public float jumpHeight;
    public float grav;
    public float jumpGravMod;

    public bool isGrounded = true;
    public bool canJump = true;

    public Transform groundedOrigin;

    public LayerMask groundLayer;

    private float xInput = 0f;
    private float jumpSpeed = 0f;
    private float jumpGrav

    private bool wantsJump = false;

    // Start is called before the first frame update
    void Start()
    {
        jumpGrav = grav * jumpGravMod;
        jumpSpeed = -2 * jumpGrav * jumpHeight;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        SetHorizontalSpeed();
    }

    private void FixedUpdate()
    {
        CheckGrounded();

        Move();
    }

    void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
    }

    void CheckJumpInput()
    {
        //ndk
    }

    void SetHorizontalSpeed()
    {
        if (xInput != 0)
        {
            if (xInput > 0)
            {
                if (xSpeed < 0)
                {
                    xSpeed = 0 + accel * Time.deltaTime;
                }
                if (xSpeed < maxXSpeed * xInput)
                {
                    xSpeed += accel * Time.deltaTime;
                }
                else
                {
                    xSpeed = maxXSpeed * xInput;
                }
            }
            else
            {
                if (xSpeed > 0)
                {
                    xSpeed = 0 + accel * Time.deltaTime;
                }
                if (xSpeed > maxXSpeed * xInput)
                {
                    xSpeed -= accel * Time.deltaTime;
                }
                else
                {
                    xSpeed = maxXSpeed * xInput;
                }
            }
        }
        else
        {
            if (xSpeed > stopRange)
            {
                xSpeed -= decel * Time.deltaTime;
            }
            else if (xSpeed < -stopRange)
            {
                xSpeed += decel * Time.deltaTime;
            }
            else
            {
                xSpeed = 0f;
            }
        }
    }

    Vector3 CreateMovVector()
    {
        //IF SLOPES ARE IMPLEMENTED MAKE SURE TO CHANGE THE X SPEED TO ALSO AFFECT THE Y SPEED
        Vector3 mov = new Vector3(xSpeed, ySpeed, 0f);
        return mov;
    }


    void Move()
    {
        Vector3 mov = CreateMovVector();
        gameObject.transform.Translate(mov);
    }

    void CheckGrounded()
    {
        isGrounded = Physics.Raycast(groundedOrigin.position, -gameObject.transform.up, groundedCheckDis, groundLayer);
    }
}
