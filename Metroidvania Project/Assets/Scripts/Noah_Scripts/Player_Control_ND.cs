using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class Player_Control_ND : MonoBehaviour
{

    #region player controls
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
    public float creepSpeed = 3.0f;
    public float powerJumpSpeed = 10.0f;
    public float stompSpeed = 4.0f;
    #endregion
    #region player states
    public bool isGrounded;
    public bool isJumping;
    public bool isFacingRight;
    public bool doubleJumped;
    public bool wallJumped;
    public bool isWallRunning;
    public bool isSlopeSliding;
    public bool isGliding;
    public bool isDucking;
    public bool isCreeping;
    public bool isPowerJumping;
    public bool isStomping;
    #endregion
    #region player abilities
    public bool canDoubleJump = true;
    public bool canWallJump = true;
    public bool canWallRun = true;
    public bool canRunAfterWallJump = true;
    public bool canGlide = true;
    public bool canPowerJump = true;
    public bool canStomp = true;
    #endregion
    #region Private variables
    private CharacterController2D.CharacterCollisionState2D flags;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController2D characterController;
    private bool lastJumpWasLeft;
    private float slopeAngle;
    private Vector3 slopeGradient = Vector3.zero;
    private bool startGlide;
    private float currentGlideTimer;
    public LayerMask layerMask;
    private BoxCollider2D boxCollider;
    private Vector2 originalBoxCollider;
    private Vector3 frontTopCorner;
    private Vector3 backTopCorner;
    private Animator animator;
    private bool ableToWallRun;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController2D>();
        currentGlideTimer = glideTimer;
        boxCollider = GetComponent<BoxCollider2D>();
        originalBoxCollider = boxCollider.size;
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //sets the player movement for X
        if (wallJumped == false)
        {
            moveDirection.x = Input.GetAxis("Horizontal");
            if (isCreeping)
            {
                moveDirection.x *= creepSpeed;
            }
            else
            {
                moveDirection.x *= walkSpeed;
            }
        }
        #region Ground angle check
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
        #endregion
        #region Player on ground
        //player is on the ground
        if (isGrounded)
        {
            moveDirection.y = 0;
            isJumping = false;
            doubleJumped = false;
            isStomping = false;
            currentGlideTimer = glideTimer;
            if (moveDirection.x < 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
                isFacingRight = false;
            }
            else if (moveDirection.x > 0)
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
                if (canPowerJump && isDucking)
                {
                    moveDirection.y = jumpSpeed + powerJumpSpeed;
                    StartCoroutine("PowerJumpWaiter");
                }
                else
                {
                    moveDirection.y = jumpSpeed;
                    isJumping = true;
                }
                ableToWallRun = true;
            }
        }
        #endregion
        #region Player in the air
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
                if (canDoubleJump)
                {
                    if (!doubleJumped)
                    {
                        moveDirection.y = doubleJumpSpeed;
                        doubleJumped = true;
                    }
                }
            }
        }
        #endregion
        #region Gravity Calculations
        //Controls gliding for jetpack
        if (canGlide == true && Input.GetAxis("Vertical") > 0.5f && characterController.velocity.y < 0.2f)
        {
            if (currentGlideTimer > 0)
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
        else if (canStomp && isDucking && !isPowerJumping)
        {
            moveDirection.y -= gravity * Time.deltaTime + stompSpeed;
            isStomping = true;
        }
        else
        {
            isGliding = false;
            startGlide = true;
            moveDirection.y -= gravity * Time.deltaTime;
        }
        #endregion
        //update the CharacterController
        characterController.move(moveDirection * Time.deltaTime);
        flags = characterController.collisionState;

        #region Ducking and creeping
        frontTopCorner = new Vector3(transform.position.x + boxCollider.size.x / 2, transform.position.y + boxCollider.size.y / 2, 0);
        backTopCorner = new Vector3(transform.position.x - boxCollider.size.x / 2, transform.position.y + boxCollider.size.y / 2, 0);
        RaycastHit2D hitFrontCeiling = Physics2D.Raycast(frontTopCorner, Vector2.up, 2f, layerMask);
        RaycastHit2D hitBackCeiling = Physics2D.Raycast(backTopCorner, Vector2.up, 2f, layerMask);
        if (Input.GetAxis("Vertical") < 0 && moveDirection.x == 0)
        {
            if (!isDucking && !isCreeping)
            {
                boxCollider.size = new Vector2(boxCollider.size.x, boxCollider.size.y / 2);
                transform.position = new Vector3(transform.position.x, transform.position.y - (originalBoxCollider.y / 4), 0);
                characterController.recalculateDistanceBetweenRays();
            }
            isCreeping = false;
            isDucking = true;
        }
        else if (Input.GetAxis("Vertical") < 0 && (moveDirection.x < 0 || moveDirection.x > 0))
        {
            if (!isDucking && !isCreeping)
            {
                boxCollider.size = new Vector2(boxCollider.size.x, boxCollider.size.y / 2);
                characterController.recalculateDistanceBetweenRays();
            }
            isCreeping = true;
            isDucking = false;
        }
        else
        {
            if ((!hitFrontCeiling.collider && !hitBackCeiling) && (isDucking || isCreeping))
            {
                boxCollider.size = new Vector2(boxCollider.size.x, originalBoxCollider.y);
                transform.position = new Vector3(transform.position.x, transform.position.y + (originalBoxCollider.y / 4), 0);
                characterController.recalculateDistanceBetweenRays();
                isCreeping = false;
                isDucking = false;
            }

        }
        #endregion
        if (flags.above)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        #region wall running and jumping
        if (flags.left || flags.right)
        {
            if (canWallRun)
            {
                if (Input.GetAxis("Vertical") > 0 && ableToWallRun == true && isGrounded == false)
                {
                    moveDirection.y = jumpSpeed / wallRunAmount;
                    StartCoroutine(WallRunWaiter());

                    if (flags.left)
                    {
                        transform.eulerAngles = new Vector3(0, 180, 0);
                    }
                    else if (flags.right)
                    {
                        transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                }
            }

            if (canWallJump)
            {
                if (Input.GetButtonDown("Jump") || wallJumped == false && isGrounded == false)
                {
                    if (moveDirection.x < 0)
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
                    if (canRunAfterWallJump)
                    {
                        doubleJumped = false;
                    }
                    else
                    {
                        ableToWallRun = false;
                    }
                }
            }
        }
        else
        {
            if (canRunAfterWallJump)
            {
                StopCoroutine(WallRunWaiter());
                ableToWallRun = true;
                isWallRunning = false;
            }
        }
        #endregion
        isGrounded = flags.below;
        UpdateAnimator();
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
        if (wallJumped == false)
        {
            ableToWallRun = false;
        }
    }

    //power jump timer
    IEnumerator PowerJumpWaiter()
    {
        isPowerJumping = true;
        yield return new WaitForSeconds(0.8f);
        isPowerJumping = false;
    }

    //animator set
    private void UpdateAnimator()
    {
        animator.SetFloat("movementX", Mathf.Abs(moveDirection.x / walkSpeed));
        animator.SetFloat("movementY", moveDirection.y);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("doubleJumped", doubleJumped);
        animator.SetBool("wallJumped", wallJumped);
        animator.SetBool("isWallRunning", isWallRunning);
        animator.SetBool("isGliding", isGliding);
        animator.SetBool("isDucking", isDucking);
        animator.SetBool("isCreeping", isCreeping);
        animator.SetBool("isPowerJumping", isPowerJumping);
        animator.SetBool("isStomping", isStomping);
        animator.SetBool("isSlopeSliding", isSlopeSliding);
    }
}
