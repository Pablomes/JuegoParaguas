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
    public float coyoteTime = .1f;

    public bool isGrounded = true;
    public bool blockedRight = false;
    public bool blockedLeft = false;
    public bool blockedTop = false;
    public bool jumpPressed = false;

    public Transform self;
    public Transform sprite;

    public Transform[] groundedOrigin = new Transform[3];
    public Transform[] rightCheckOrigins = new Transform[3];
    public Transform[] leftCheckOrigins = new Transform[3];
    public Transform[] upCheckOrigins = new Transform[3];

    public LayerMask groundLayer;

    public UmbrellaController umbrella;

    private float xInput = 0f;
    private float jumpSpeed = 0f;
    private float jumpGrav;
    private float jumpBufferTimer;
    private float jumpTimer;
    private float jumpXAccel;
    private float jumpXDecel;
    [SerializeField]
    private float coyoteTimer;

    private Vector2 actualPos;

    [SerializeField]
    private bool wantsJump = false;
    [SerializeField]
    private bool wantsJumpTemp = false;
    [SerializeField]
    private bool isJumping = false;
    [SerializeField]
    private bool isJumpingTemp = false;
    [SerializeField]
    private bool isCoyote = false;

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
        //print(jumpSpeed);

        CheckInput();
        SetSpriteDirection();
        CheckJumpInput();
        SetHorizontalSpeed();
        SetVerticalSpeed();
        CheckCoyote();
        StartJump();
    }

    void FixedUpdate()
    {
        actualPos = new Vector2(self.transform.position.x, self.transform.position.y);

        //StartJump();

        /*
        CheckGrounded();

        if (xInput != 0f & !blockedTop)
        {
            if (xInput > 0f)
            {
                CheckRightCollisions();
            }
            else
            {
                CheckLeftCollisions();
            }
        }

        //CheckGrounded();

        if (!isGrounded)
        {
            CheckTopCollisions();
        }
        

        //DO NOT WORK PROPERLY. TRY TO FIND A WAY TO GET UNITY TO HANDLE COLLSIIONS. CAN A KINEMATIC RIGIDBODY HAVE COLLISIONS HANDLED BY UNITY?
        //USE HIT.POINT TO GET THE ACTUAL POINT COORDINATES
        //THAT SHOULD WORK
        //CheckLeftCollisions();
        //CheckRightCollisions();
        //CheckTopCollisions();
        */

        Move();
    }

    void SetSpriteDirection()
    {
        if (xInput > 0)
        {
            sprite.transform.localScale = new Vector3(1, sprite.transform.localScale.y, sprite.transform.localScale.z);
        }
        else if (xInput < 0)
        {
            sprite.transform.localScale = new Vector3(-1, sprite.transform.localScale.y, sprite.transform.localScale.z);
        }
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
                blockedLeft = false;
                
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

                if (blockedRight)
                {
                    xSpeed = 0f;
                }
            }
            else
            {
                blockedRight = false;

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

                if (blockedLeft)
                {
                    xSpeed = 0f;
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

        if (blockedTop)
        {
            StopJump(-0.05f);
        }
    }

    void StartJump()
    {
        if (wantsJump)
        {
            if (isGrounded || isCoyote)
            {
                Jump();
                wantsJump = false;
                isGrounded = false;
                isCoyote = false;
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
                    jumpBufferTimer -= Time.deltaTime;
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
        
        float y_pos_min = CheckGround();
        float y_pos_max = CheckTopColl();
        float x_pos_max = CheckRightColl();
        float x_pos_min = CheckLeftColl();

        float new_x_pos = actualPos.x + xSpeed;
        float new_y_pos = actualPos.y + ySpeed;

        new_x_pos = Mathf.Clamp(new_x_pos, x_pos_min, x_pos_max);
        new_y_pos = Mathf.Clamp(new_y_pos, y_pos_min, y_pos_max);

        //Debug.Log(new_x_pos);
        //Debug.Log(new_y_pos);
        Debug.Log(actualPos.y);

        Vector3 mov = new Vector3(new_x_pos - actualPos.x, new_y_pos - actualPos.y, 0f);
        

        //Vector3 mov = new Vector3(xSpeed, ySpeed, 0f);
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
            //print(hit.transform.position);
            //print(hit.point.y);

            Vector2 surfacePoint = hit.collider.ClosestPoint(hit.point);
            Vector2 point = hit.point;
            float dist = 0f;

            while (surfacePoint == point)
            {
                point = new Vector2(point.x, point.y + groundedCheckDis * 0.1f);
                dist += groundedCheckDis * 0.1f;
                surfacePoint = hit.collider.ClosestPoint(point);

                if (surfacePoint == point & dist > groundedCheckDis + 1f)
                {
                    break;
                }
            }

            //print(surfacePoint.y);

            if (!isJumping)
            {
                isGrounded = true;
                blockedTop = false;
                //float heightPosition = hit.transform.position.y + (self.transform.position.y - groundedOrigin.position.y);

                //float y_pos = Mathf.Clamp(self.transform.position.y, hit.transform.position.y + self.lossyScale.y / 2, Mathf.Infinity);
                if (surfacePoint != point)
                {
                    float y_pos = Mathf.Clamp(self.transform.position.y, surfacePoint.y, Mathf.Infinity);
                    self.transform.position = new Vector3(self.transform.position.x, y_pos, self.transform.position.z);
                }
                //float y_pos = Mathf.Clamp(self.transform.position.y, surfacePoint.y, Mathf.Infinity);
                //self.transform.position = new Vector3(self.transform.position.x, y_pos, self.transform.position.z);


                //self.transform.position = new Vector3(self.transform.position.x, hit.transform.position.y + self.lossyScale.y / 2, self.transform.position.z);
            };
            
        }
        else
        {
            if (isGrounded & !isJumping)
            {
                coyoteTimer = coyoteTime;
                isCoyote = true;
            }
            isGrounded = false;
        }
    }

    float CheckGround()
    {
        float y_pos_min;
        RaycastHit2D hit = new RaycastHit2D();
        //isGrounded = Physics2D.Raycast(groundedOrigin.position, -gameObject.transform.up, groundedCheckDis, groundLayer);
        foreach (Transform groundOrigin in groundedOrigin)
        {
            hit = Physics2D.Raycast(groundOrigin.position, -self.transform.up, groundedCheckDis, groundLayer, -Mathf.Infinity, Mathf.Infinity);
            if (hit)
            {
                break;
            }
        }

        if (hit)
        {
            Vector2 surfacePoint = hit.collider.ClosestPoint(hit.point);
            Vector2 point = hit.point;
            float dist = 0f;

            while (surfacePoint == point)
            {
                point = new Vector2(point.x, point.y + groundedCheckDis * 0.1f);
                dist += groundedCheckDis * 0.1f;
                surfacePoint = hit.collider.ClosestPoint(point);

                if (surfacePoint == point & dist > groundedCheckDis + 1f)
                {
                    break;
                }
            }


            /*
            if (!isJumping)
            {
                isGrounded = true;

                if (surfacePoint != point)
                {
                    y_pos_min = surfacePoint.y;
                }
                else
                {
                    y_pos_min = -Mathf.Infinity;
                }
            }
            */
            isGrounded = true;

            if (surfacePoint != point)
            {
                y_pos_min = surfacePoint.y;
            }
            else
            {
                y_pos_min = -Mathf.Infinity;
            }
        }
        else
        {
            if (isGrounded & !isJumping)
            {
                coyoteTimer = coyoteTime;
                isCoyote = true;
            }
            isGrounded = false;
            y_pos_min = -Mathf.Infinity;
        }

        return y_pos_min;
    }

    void CheckCoyote()
    {
        if (isCoyote)
        {
            if (coyoteTimer > 0)
            {
                coyoteTimer -= Time.deltaTime;
            }
            else
            {
                isCoyote = false;
            }
        }
    }

    void CheckRightCollisions()
    {
        RaycastHit2D hit = new RaycastHit2D();

        Transform[] localRightCheckOrigins;

        if (isGrounded)
        {
            localRightCheckOrigins = new Transform[] { rightCheckOrigins[0], rightCheckOrigins[2] };
        }
        else
        {
            localRightCheckOrigins = rightCheckOrigins;
        }

        foreach (Transform rightOrigin in localRightCheckOrigins)
        {
            hit = Physics2D.Raycast(rightOrigin.position, self.transform.right, groundedCheckDis, groundLayer, -Mathf.Infinity, Mathf.Infinity);
            if (hit)
            {
                break;
            }
        }

        if (hit)
        {
            //FINISH THIS

            Vector2 surfacePoint = hit.collider.ClosestPoint(hit.point);
            Vector2 point = hit.point;
            float dist = 0f;

            while (surfacePoint == point)
            {
                point = new Vector2(point.x - groundedCheckDis * 0.1f, point.y);
                dist += groundedCheckDis * 0.1f;
                surfacePoint = hit.collider.ClosestPoint(point);
                if (dist > groundedCheckDis + 1f & surfacePoint == point)
                {
                    break;
                }
            }


            blockedRight = true;
            if (surfacePoint != point)
            {
                float x_pos = Mathf.Clamp(self.transform.position.x, -Mathf.Infinity, surfacePoint.x - self.lossyScale.x / 2);
                self.transform.position = new Vector3(x_pos, self.transform.position.y, self.transform.position.z);
            }
            //float x_pos = Mathf.Clamp(self.transform.position.x, -Mathf.Infinity, surfacePoint.x);
            //self.transform.position = new Vector3(x_pos, self.transform.position.y, self.transform.position.z);

        }
        else
        {
            blockedRight = false;
        }
    }

    float CheckRightColl()
    {
        float x_pos_max;
        RaycastHit2D hit = new RaycastHit2D();

        Transform[] localRightCheckOrigins;

        if (isGrounded)
        {
            localRightCheckOrigins = new Transform[] { rightCheckOrigins[0], rightCheckOrigins[2] };
        }
        else
        {
            localRightCheckOrigins = rightCheckOrigins;
        }

        foreach (Transform rightOrigin in localRightCheckOrigins)
        {
            hit = Physics2D.Raycast(rightOrigin.position, self.transform.right, groundedCheckDis, groundLayer, -Mathf.Infinity, Mathf.Infinity);
            if (hit)
            {
                break;
            }
        }

        if (hit)
        {
            Vector2 surfacePoint = hit.collider.ClosestPoint(hit.point);
            Vector2 point = hit.point;
            float dist = 0f;

            while (surfacePoint == point)
            {
                point = new Vector2(point.x - groundedCheckDis * 0.1f, point.y);
                dist += groundedCheckDis * 0.1f;
                surfacePoint = hit.collider.ClosestPoint(point);
                if (dist > groundedCheckDis + 1f & surfacePoint == point)
                {
                    break;
                }
            }

            //IF SYSTEM DOES NOT WORK PUT THIS LINE INSIDE THE FOLLOWING IF STATEMENT THE SAME WAY AS IN TEH CHECKGROUNDED FUNCTION
            // ||
            // VV
            blockedRight = true;
            if (surfacePoint != point)
            {
                x_pos_max = surfacePoint.x - self.lossyScale.x / 2;
            }
            else
            {
                x_pos_max = Mathf.Infinity;
            }
        }
        else
        {
            x_pos_max = Mathf.Infinity;
            blockedRight = false;
        }

        return x_pos_max;
    }

    void CheckLeftCollisions()
    {
        RaycastHit2D hit = new RaycastHit2D();
        Transform[] localLeftCheckOrigins;

        if (isGrounded)
        {
            localLeftCheckOrigins = new Transform[] { leftCheckOrigins[0], leftCheckOrigins[2] };
        }
        else
        {
            localLeftCheckOrigins = leftCheckOrigins;
        }

        foreach (Transform leftOrigin in localLeftCheckOrigins)
        {
            hit = Physics2D.Raycast(leftOrigin.position, -self.transform.right, groundedCheckDis, groundLayer, -Mathf.Infinity, Mathf.Infinity);
            if (hit)
            {
                break;
            }
        }

        if (hit)
        {
            Vector2 surfacePoint = hit.collider.ClosestPoint(hit.point);
            Vector2 point = hit.point;
            float dist = 0f;

            while (surfacePoint == point)
            {
                point = new Vector2(point.x + groundedCheckDis * 0.1f, point.y);
                dist += groundedCheckDis * 0.1f;
                surfacePoint = hit.collider.ClosestPoint(point);

                if (dist > groundedCheckDis + 1f & surfacePoint == point)
                {
                    break;
                }
            }


            blockedLeft = true;
            if (surfacePoint != point)
            {
                float x_pos = Mathf.Clamp(self.transform.position.x, surfacePoint.x + self.lossyScale.x / 2, Mathf.Infinity);
                self.transform.position = new Vector3(x_pos, self.transform.position.y, self.transform.position.z);
            }
            //float x_pos = Mathf.Clamp(self.transform.position.x, surfacePoint.x, Mathf.Infinity);
            //self.transform.position = new Vector3(x_pos, self.transform.position.y, self.transform.position.z);

        }
        else
        {
            blockedLeft = false;
        }
    }

    float CheckLeftColl()
    {
        float x_pos_min;
        RaycastHit2D hit = new RaycastHit2D();
        Transform[] localLeftCheckOrigins;

        if (isGrounded)
        {
            localLeftCheckOrigins = new Transform[] { leftCheckOrigins[0], leftCheckOrigins[2] };
        }
        else
        {
            localLeftCheckOrigins = leftCheckOrigins;
        }

        foreach (Transform leftOrigin in localLeftCheckOrigins)
        {
            hit = Physics2D.Raycast(leftOrigin.position, -self.transform.right, groundedCheckDis, groundLayer, -Mathf.Infinity, Mathf.Infinity);
            if (hit)
            {
                break;
            }
        }

        if (hit)
        {
            Vector2 surfacePoint = hit.collider.ClosestPoint(hit.point);
            Vector2 point = hit.point;
            float dist = 0f;

            while (surfacePoint == point)
            {
                point = new Vector2(point.x + groundedCheckDis * 0.1f, point.y);
                dist += groundedCheckDis * 0.1f;
                surfacePoint = hit.collider.ClosestPoint(point);

                if (dist > groundedCheckDis + 1f & surfacePoint == point)
                {
                    break;
                }
            }

            blockedLeft = true;
            if (surfacePoint != point)
            {
                x_pos_min = surfacePoint.x + self.lossyScale.x / 2;
            }
            else
            {
                x_pos_min = -Mathf.Infinity;
            }
        }
        else
        {
            x_pos_min = -Mathf.Infinity;
            blockedLeft = false;
        }

        return x_pos_min;
    }

    void CheckTopCollisions()
    {
        RaycastHit2D hit = new RaycastHit2D();
        Transform[] localUpCheckOrigins;

        if (blockedLeft)
        {
            localUpCheckOrigins = new Transform[] { upCheckOrigins[0], upCheckOrigins[1] };
        }
        else if (blockedLeft)
        {
            localUpCheckOrigins = new Transform[] { upCheckOrigins[0], upCheckOrigins[2] };
        }
        else
        {
            localUpCheckOrigins = upCheckOrigins;
        }

        foreach (Transform topOrigin in localUpCheckOrigins)
        {
            hit = Physics2D.Raycast(topOrigin.position, self.transform.up, groundedCheckDis, groundLayer, -Mathf.Infinity, Mathf.Infinity);
            if (hit)
            {
                break;
            }
        }

        if (hit)
        {
            
            Vector2 surfacePoint = hit.collider.ClosestPoint(hit.point);
            Vector2 point = hit.point;
            float dist = 0f;

            while (surfacePoint == point)
            {
                point = new Vector2(point.y - groundedCheckDis * 0.1f, point.y);
                dist += groundedCheckDis * 0.1f;
                surfacePoint = hit.collider.ClosestPoint(point);

                if (dist > groundedCheckDis + 1f & surfacePoint == point)
                {
                    break;
                }
            }
            


            blockedTop = true;

            if (surfacePoint != point)
            {
                float y_pos = Mathf.Clamp(self.transform.position.y, -Mathf.Infinity, surfacePoint.y - self.lossyScale.y);
                self.transform.position = new Vector3(self.transform.position.x, y_pos, self.transform.position.z);
            }

            //MIGHT CAUSE PLAYER TO GET STUCK ON ROOFS
            //float y_pos = Mathf.Clamp(self.transform.position.y, -Mathf.Infinity, surfacePoint.y);
            //self.transform.position = new Vector3(self.transform.position.x, y_pos, self.transform.position.z);

        }
        else
        {
            blockedTop = false;
        }
    }

    float CheckTopColl()
    {
        float y_pos_max;
        RaycastHit2D hit = new RaycastHit2D();

        foreach (Transform topOrigin in upCheckOrigins)
        {
            hit = Physics2D.Raycast(topOrigin.position, self.transform.up, groundedCheckDis, groundLayer, -Mathf.Infinity, Mathf.Infinity);
            if (hit)
            {
                break;
            }
        }

        if (hit)
        {
            Vector2 surfacePoint = hit.collider.ClosestPoint(hit.point);
            Vector2 point = hit.point;
            float dist = 0f;

            while (surfacePoint == point)
            {
                point = new Vector2(point.y - groundedCheckDis * 0.1f, point.y);
                dist += groundedCheckDis * 0.1f;
                surfacePoint = hit.collider.ClosestPoint(point);

                if (dist > groundedCheckDis + 1f & surfacePoint == point)
                {
                    break;
                }
            }

            blockedTop = true;

            if (surfacePoint != point)
            {
                y_pos_max = surfacePoint.y - self.lossyScale.y;
            }
            else
            {
                y_pos_max = Mathf.Infinity;
            }
        }
        else
        {
            y_pos_max = Mathf.Infinity;
            blockedTop = false;
        }

        return y_pos_max;
    }
}
