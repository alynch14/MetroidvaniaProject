using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class Player_Control_ND : MonoBehaviour
{
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
    public bool isGrounded;
    public bool isJumping;
    public bool isFacingRight;
    public bool doubleJumped;
    public bool wallJumped;
    public bool isWallRunning;
    public bool isSlopeSliding;
    public bool isGliding;
    public bool canDoubleJump = true;
    public bool canWallJump = true;
    public bool canWallRun = true;
    public bool canRunAfterWallJump = true;
    public bool canGlide = true;
    private Vector3 _moveDirection = Vector3.zero;
    private CharacterController2D _characterController;
    private bool _lastJumpWasLeft;
    private float _slopeAngle;
    private Vector3 _slopeGradient = Vector3.zero;
    public LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(wallJumped ==false)
        {
            _moveDirection.x = Input.GetAxis("Horizontal");
            _moveDirection.x *= walkSpeed;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.up, 2f, layerMask);
        if (hit)
        {
            _slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            _slopeGradient = hit.normal;
            if (_slopeAngle > _characterController.slopeLimit)
            {
                isSlopeSliding = true;
            }
            else
            {
                isSlopeSliding = false;
            }
        }
        if (isGrounded)
        {
            _moveDirection.y = 0;
            isJumping = false;
            doubleJumped = false;

            if (_moveDirection.x < 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
                isFacingRight = false;
            }
            else if(_moveDirection.x >0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                isFacingRight = true;
            }

            if (isSlopeSliding)
            {
                _moveDirection = new Vector3(_slopeGradient.x * slopeSlideSpeed, _slopeGradient.y * slopeSlideSpeed, 0f);
            }
                if (Input.GetButtonDown("Jump"))
            {
                _moveDirection.y = jumpSpeed;
                isJumping = true;
                isWallRunning = true;
            }
        }
        else
        {
            if (Input.GetButtonUp("Jump"))
            {
                if (_moveDirection.y > 0)
                {
                    _moveDirection.y = _moveDirection.y * 0.5f;
                }
            }
            if (Input.GetButtonDown("Jump"))
            {
                if (canDoubleJump) {
                    if (!doubleJumped)
                    {
                        _moveDirection.y = doubleJumpSpeed;
                        doubleJumped = true;
                    }
                }
            }
        }
        if (canGlide ==true && Input.GetAxis("Vertical") > 0.5f && _characterController.velocity.y <0.2f)
        {
            isGliding = true;
            _moveDirection.y -= glideAmount * Time.deltaTime;
        }
        else
        {
            _moveDirection.y -= gravity * Time.deltaTime;
        }
        _moveDirection.y -= gravity * Time.deltaTime;
        _characterController.move(_moveDirection*Time.deltaTime);
        flags = _characterController.collisionState;
        isGrounded = flags.below;

        if (flags.above)
        {
            _moveDirection.y -= gravity * Time.deltaTime;
        }
        if (flags.left || flags.right)
        {
            if (canWallRun)
            {
                if (Input.GetAxis("Vertical") >0 && isWallRunning == true)
                {
                    _moveDirection.y = jumpSpeed / wallRunAmount;
                    StartCoroutine(WallRunWaiter());
                }
            }
            if (canWallJump)
            {
                if (Input.GetButtonDown("Jump") || wallJumped== false && isGrounded==false)
                {
                    if(_moveDirection.x < 0)
                    {
                        _moveDirection.x = jumpSpeed * wallJumpXAmount;
                        _moveDirection.y = jumpSpeed * wallJumpYAmount;
                        transform.eulerAngles = new Vector3(0, 180, 0);
                        _lastJumpWasLeft = false;
                    }
                    else if (_moveDirection.x > 0)
                    {
                        _moveDirection.x = jumpSpeed * wallJumpXAmount;
                        _moveDirection.y = jumpSpeed * wallJumpYAmount;
                        transform.eulerAngles = new Vector3(0, 0, 0);
                        _lastJumpWasLeft = true;
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
    IEnumerator WallJumpWaiter()
    {
        wallJumped = true;
        yield return new WaitForSeconds(0.5f);
        wallJumped = false;
    }
    IEnumerator WallRunWaiter()
    {
        isWallRunning = true;
        yield return new WaitForSeconds(0.5f);
        isWallRunning = false;
    }
}
