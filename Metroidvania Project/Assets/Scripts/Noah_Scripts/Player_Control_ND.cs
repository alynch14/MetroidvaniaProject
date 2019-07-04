using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class Player_Control_ND : MonoBehaviour
{
    //player controls
    public CharacterController2D.CharacterCollisionState2D flags;
    public float walkSpeed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float doubleJumpSpeed = 4.0f;
    public float wallJumpXAmount = 1.5f;
    public float wallJumpYAmount = 1.5f;
    public float wallRunAmount = 2f;
    public float slopeSlideSpeed = 4f;
    public float glideAmount = 2f;
    public float glideTimer = 2f;
    
    //player states
    public bool isGrounded;
    public bool isJumping;
    public bool isFacingRight;
    public bool doubleJumped;
    public bool wallJumped;
    public bool isWallRunning;
    public bool isSlopeSliding;
    public bool isGliding;
    
    //player abilities
    public bool canDoubleJump = true;
    public bool canWallJump = true;
    public bool canWallRun = true;
    public bool canRunAfterWallJump = true;
    public bool canGlide = true;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController2D characterController;
    private bool lastJumpWasLeft;
    private float slopeAngle;
    private Vector3 slopeGradient = Vector3.zero;
    private bool startGlide;
    private float currentGlideTimer;
    public LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController2D>();
        currentGlideTimer = glideTimer;
    }

    // Update is called once per frame
    void Update()
    {
        characterController = GetComponent<CharacterController2D>();
        if (wallJumped ==false)
        {
            moveDirection.x = Input.GetAxis("Horizontal");
            moveDirection.x *= walkSpeed;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.up, 2f, layerMask);
        if (hit)
        {
            slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            slopeGradient = hit.normal;
            if (slopeAngle > characterController.slopeLimit)
            {
                isSlopeSliding = true;
            }
            else
            {
                isSlopeSliding = false;
            }
        }
//player is on the ground
        if (isGrounded)
        {
            moveDirection.y = 0;
            isJumping = false;
            doubleJumped = false;
            currentGlideTimer = glideTimer;
            if (moveDirection.x < 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
                isFacingRight = false;
            }
            else if(moveDirection.x >0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                isFacingRight = true;
            }

            if (isSlopeSliding)
            {
                moveDirection = new Vector3(slopeGradient.x * slopeSlideSpeed, slopeGradient.y * slopeSlideSpeed, 0f);
            }
                if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpSpeed;
                isJumping = true;
                isWallRunning = true;
            }
        }
 //player is in the air
        else
        {
            if (Input.GetButtonUp("Jump"))
            {
                if (moveDirection.y > 0)
                {
                    moveDirection.y = moveDirection.y * 0.5f;
                }
            }
            if (Input.GetButtonDown("Jump"))
            {
                //double jump
                if (canDoubleJump) {
                    if (!doubleJumped)
                    {
                        moveDirection.y = doubleJumpSpeed;
                        doubleJumped = true;
                    }
                }
            }
        }

//Controls gliding for jetpack
        if (canGlide ==true && Input.GetAxis("Vertical") > 0.5f && characterController.velocity.y <0.2f)
        {
            if (currentGlideTimer >0)
            {
                isGliding = true;
                if (startGlide)
                {
                    moveDirection.y = 0;
                    startGlide = false;
                }
                moveDirection.y -= glideAmount * Time.deltaTime;
                currentGlideTimer -= Time.deltaTime;
            }
            else
            {
                isGliding = false;
                moveDirection.y -= gravity * Time.deltaTime;
            }
        }
        else
        {
            isGliding = false;
            startGlide = true;
            moveDirection.y -= gravity * Time.deltaTime;
        }

       moveDirection.y -= gravity * Time.deltaTime;
       characterController.move(moveDirection*Time.deltaTime);
       flags = characterController.collisionState;
       isGrounded = flags.below;

        if (flags.above)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        if (flags.left || flags.right)
        {
            if (canWallRun)
            {
                if (Input.GetAxis("Vertical") >0 && isWallRunning == true)
                {
                    moveDirection.y = jumpSpeed / wallRunAmount;
                    StartCoroutine(WallRunWaiter());
                }
            }
            if (canWallJump)
            {
                if (Input.GetButtonDown("Jump") || wallJumped== false && isGrounded==false)
                {
                    if(moveDirection.x < 0)
                    {
                        moveDirection.x = jumpSpeed * wallJumpXAmount;
                        moveDirection.y = jumpSpeed * wallJumpYAmount;
                        transform.eulerAngles = new Vector3(0, 180, 0);
                        lastJumpWasLeft = false;
                    }
                    else if (moveDirection.x > 0)
                    {
                        moveDirection.x = jumpSpeed * wallJumpXAmount;
                        moveDirection.y = jumpSpeed * wallJumpYAmount;
                        transform.eulerAngles = new Vector3(0, 0, 0);
                        lastJumpWasLeft = true;
                    }
                    StartCoroutine(WallJumpWaiter());
                }
            }
        }
        else
        {
            if(canRunAfterWallJump)
            {
                StopCoroutine(WallRunWaiter());
                isWallRunning = true;
            }
        }
    }
    //timer for wall jumping
    IEnumerator WallJumpWaiter()
    {
        wallJumped = true;
        yield return new WaitForSeconds(0.5f);
        wallJumped = false;
    }
    //timer for wall running
    IEnumerator WallRunWaiter()
    {
        isWallRunning = true;
        yield return new WaitForSeconds(0.5f);
        isWallRunning = false;
    }
}
