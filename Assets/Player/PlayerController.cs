using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxXSpeed;
    public float maxYSpeed;
    public float accel;
    public float decel;
    public float jumpAccelMod;
    public float stopRange = 0.1f;
    public float xSpeed = 0f;
    public float ySpeed = 0f;
    public float groundedCheckDis = 0.05f;
    public float jumpHeight;
    public float jumpTime;
    public float minJumpMod;
    public float grav;
    public float jumpGravMod;
    public float jumpBufferTime = .1f;
    public float maxFallSpeed = 0.35f;

    public bool isGrounded = true;
    public bool jumpPressed = false;

    public Transform self;
    public Transform sprite;

    public Transform[] groundedOrigin = new Transform[3];

    public LayerMask groundLayer;

    private float xInput = 0f;
    private float jumpSpeed = 0f;
    private float jumpGrav;
    private float jumpBufferTimer;
    private float jumpTimer;
    private float jumpXAccel;
    private float jumpXDecel;

    [SerializeField]
    private bool wantsJump = false;
    [SerializeField]
    private bool wantsJumpTemp = false;
    [SerializeField]
    private bool isJumping = false;
    [SerializeField]
    private bool isJumpingTemp = false;

    // Start is called before the first frame update
    void Start()
    {
        //jumpGrav = grav * jumpGravMod;
        //jumpSpeed = Mathf.Sqrt(2 * jumpGrav * jumpHeight);

        jumpSpeed = (2 * jumpHeight) / jumpTime;
        jumpGrav = (2 * jumpHeight) / (jumpTime * jumpTime);
        grav = jumpGrav / jumpGravMod;

        jumpXAccel = accel * jumpAccelMod;
        jumpXDecel = decel * jumpAccelMod;
    }
    // Update is called once per frame
    void Update()
    {
        print(jumpSpeed);

        CheckInput();
        CheckJumpInput();
        SetHorizontalSpeed();
        SetVerticalSpeed();
    }

    void FixedUpdate()
    {
        StartJump();

        CheckGrounded();

        Move();
    }

    void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
    }

    void CheckJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            wantsJump = true;
            jumpPressed = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumpingTemp = false;
            jumpPressed = false;

            if (isJumping)
            {
                if (jumpTimer < (jumpTime - (jumpTime * minJumpMod)))
                {
                    StopJump(0.05f);
                }

                //StopJump(0.05f);
            }

        }
    }

    void SetHorizontalSpeed()
    {
        //USE FOR WHEN GLIDE MECHANIC INTRODUCED

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

    void SetHorizontalSpeedAir()
    {
        //IDK IF I LIKE THIS BUT JUST TO TRY MOMENTUM

        if (xInput != 0)
        {
            if (xInput > 0)
            {
                if (xSpeed < 0)
                {
                    xSpeed += jumpXAccel * Time.deltaTime;
                }
                if (xSpeed < maxXSpeed * xInput)
                {
                    xSpeed += jumpXAccel * Time.deltaTime;
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
                    xSpeed -= jumpXAccel * Time.deltaTime;
                }
                if (xSpeed > maxXSpeed * xInput)
                {
                    xSpeed -= jumpXAccel * Time.deltaTime;
                }
                else
                {
                    xSpeed = maxXSpeed * xInput;
                }
            }
        }
        else
        {
            if (xSpeed > stopRange * jumpAccelMod)
            {
                xSpeed -= jumpXDecel * Time.deltaTime;
            }
            else if (xSpeed < -stopRange * jumpAccelMod)
            {
                xSpeed += jumpXDecel * Time.deltaTime;
            }
            else
            {
                xSpeed = 0f;
            }
        }
    }

    void SetVerticalSpeed()
    {
        if (isJumping)
        {
            ySpeed -= jumpGrav * Time.deltaTime;
            if (jumpTimer < 0)
            {
                StopJump(0f);
            }
            else if (jumpTimer < (jumpTime - (jumpTime * minJumpMod)) & !jumpPressed)
            {
                StopJump(0.05f);
            }
            else
            {
                jumpTimer -= Time.deltaTime;
            }

        }
        else
        {
            if (!isGrounded)
            {
                //ySpeed -= grav * Time.deltaTime;

                if (ySpeed > -maxFallSpeed)
                {
                    ySpeed -= grav * Time.deltaTime;
                }
                else
                {
                    ySpeed = -maxFallSpeed;
                }
            }
            else
            {
                ySpeed = 0f;
            }
        }
    }

    void StartJump()
    {
        if (wantsJump)
        {
            if (isGrounded)
            {
                Jump();
                wantsJump = false;
                isGrounded = false;
            }
            else
            {
                wantsJumpTemp = true;
                isJumpingTemp = true;
                jumpBufferTimer = jumpBufferTime;
                wantsJump = false;
            }
        }
        else if (wantsJumpTemp)
        {
            if (isGrounded)
            {
                Jump();
                //isJumping = isJumpingTemp;
                isJumpingTemp = false;
                isGrounded = false;
                wantsJumpTemp = false;
            }
            else
            {
                if (jumpBufferTimer < 0f)
                {
                    wantsJumpTemp = false;
                }
                else
                {
                    jumpBufferTimer -= Time.fixedDeltaTime;
                }
            }
        }
    }

    void StopJump(float afterSpeed)
    {
        //USE ONLY IF IT FEELS GOOD AFTER JUMP IS NOT PRESSED ANY MORE

        ySpeed = afterSpeed;
        isJumping = false;
    }

    void Jump()
    {
        ySpeed = jumpSpeed;
        isJumping = true; 
        jumpTimer = jumpTime;
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
        self.transform.Translate(mov);
    }

    void CheckGrounded()
    {
        RaycastHit2D hit = new RaycastHit2D();
        //isGrounded = Physics2D.Raycast(groundedOrigin.position, -gameObject.transform.up, groundedCheckDis, groundLayer);
        foreach(Transform groundOrigin in groundedOrigin)
        {
            hit = Physics2D.Raycast(groundOrigin.position, -self.transform.up, groundedCheckDis, groundLayer, -Mathf.Infinity, Mathf.Infinity);
            if (hit)
            {
                break;
            }
        }
        //hit = Physics2D.Raycast(groundedOrigin.position, -self.transform.up, groundedCheckDis, groundLayer, -Mathf.Infinity, Mathf.Infinity);

        if (hit)
        {
            print(hit.transform.position);
            if (!isJumping)
            {
                isGrounded = true;
                //float heightPosition = hit.transform.position.y + (self.transform.position.y - groundedOrigin.position.y);

                float y_pos = Mathf.Clamp(self.transform.position.y, hit.transform.position.y + self.lossyScale.y / 2, Mathf.Infinity);
                self.transform.position = new Vector3(self.transform.position.x, y_pos, self.transform.position.z);


                //self.transform.position = new Vector3(self.transform.position.x, hit.transform.position.y + self.lossyScale.y / 2, self.transform.position.z);
            };
            
        }
        else
        {
            isGrounded = false;
        }
    }
}
