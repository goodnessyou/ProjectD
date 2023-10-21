using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    PlayerInput playerInput;
    private CharacterController characterController;
    public Animator animator;
    
    int isWalkingHash;
    int isRunningHash;
    int isJumpingHash;
    int isActionHash;
    int isAttackHash;
    int isRolloverHash;
    //bool isJumpAnimating;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool isMovementPressed;
    bool isWalkPressed;
    bool isAttackPressed;
    bool isJumpPressed = false;
    bool isActionPressed = false;
    bool isRollPressed = false;
    public Transform cam;
    public float turnSmoothTime = 0.1f;
    

    private float playerSpeed = 3.0f;
    private float playerRunSpeed = 9.0f;
    float groundGravity = -7.05f;
    float gravity = -9.81f;
    private bool _isGround = true;

    float turnSmoothVelocity;
    
    
    float jumpHeight = 14.0f;

    bool isJumping = false;
    bool isRoll = false;
    //bool isAttack = false;

    private Vector3 playerVelocity;
    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = gameObject.GetComponent<CharacterController>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        isActionHash = Animator.StringToHash("isAction");
        isAttackHash = Animator.StringToHash("isAttack");
        isRolloverHash = Animator.StringToHash("isRollover");

        playerInput.CharacterControls.Move.started += OnMovementInput;
        playerInput.CharacterControls.Move.canceled += OnMovementInput;
        playerInput.CharacterControls.Move.performed += OnMovementInput;
        playerInput.CharacterControls.Walk.canceled += onWalk;
        playerInput.CharacterControls.Jump.started += onJump;
        playerInput.CharacterControls.Action.started += onAction;
        playerInput.CharacterControls.Action.canceled += onAction;

        playerInput.CharacterControls.Roll.started += onRoll;

        playerInput.CharacterControls.Attack.started += onAttack;
        playerInput.CharacterControls.Attack.canceled += onAttack;
    }

    void onRoll(InputAction.CallbackContext context)
    {
        if(!isRollPressed)
        {
            isRollPressed = true;
            StartCoroutine(RollCoroutine());
        }
        
    }
    void handleRoll()
    {
        if(!isJumping && isMovementPressed && isRollPressed)
        {
            isRoll = true;
            animator.SetBool(isRolloverHash, true);
        }
        // else if(!isJumpPressed && isJumping && _isGround)
        // {
        //     isJumping = false;
        // }
    }
    IEnumerator RollCoroutine()
    {
        //отжатие прыжка
        yield return new WaitForSeconds(0.1f);
        isRollPressed = false;
    }

    void onAttack(InputAction.CallbackContext context)
    {
        isAttackPressed = context.ReadValueAsButton();
        Debug.Log("attackstart");
    }
    
    void handleAttack()
    {
        if(!isJumping && _isGround && isAttackPressed)
        {
            isJumping = true;
            animator.SetBool(isAttackHash, true);
        }
        else
        {
            animator.SetBool(isAttackHash, false);
        }
    }
    void handleJump()
    {
        if(!isJumping && _isGround && isJumpPressed)
        {
            isJumping = true;
            animator.SetBool(isJumpingHash, true);
            _isGround = false;
            turnSmoothTime = 2f;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            
        }
        else if(!isJumpPressed && isJumping && _isGround)
        {
            isJumping = false;
        }
    }
    void handleAction()
    {
        if(!isJumping && _isGround && isActionPressed)
        {
            animator.SetBool(isActionHash, true);
        }
        
    }
    
    void onAction(InputAction.CallbackContext context)
    {
        isActionPressed = context.ReadValueAsButton();
    }
    void onJump(InputAction.CallbackContext context)
    {
        if(!isJumpPressed)
        {
            isJumpPressed = true;
            StartCoroutine(JumpCoroutine());
        }
        
    }
    IEnumerator JumpCoroutine()
    {
        //отжатие прыжка
        yield return new WaitForSeconds(0.1f);
        isJumpPressed = false;
    }
    
    void onWalk(InputAction.CallbackContext context)
    {
        if(isWalkPressed == true)
        {
            isWalkPressed = false;
        }
        else
        {
            isWalkPressed = true;
        }
    }
    void OnMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        currentRunMovement.x = currentMovementInput.x * playerRunSpeed;
        currentRunMovement.z = currentMovementInput.y * playerRunSpeed;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        
    }
    
    void handleAnimation()
    {
        bool isWalking = animator.GetBool("isWalking");
        bool isRunning = animator.GetBool("isRunning");

        if((isMovementPressed && isWalkPressed) && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }
        else if((!isMovementPressed || !isWalkPressed) && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }
        
        if(isMovementPressed && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }
        else if(!isMovementPressed && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }

        
    }

    void handleGravity()
    {
        if(_isGround)
        {
            animator.SetBool(isJumpingHash, false);
            isJumping = false;
            playerVelocity.y = groundGravity;
            turnSmoothTime = 0.1f;
            
        }
        else
        {
            animator.SetBool(isJumpingHash, true);
            playerVelocity.y += gravity * Time.deltaTime;
        }
    }

    void Update()
    {
        handleAnimation();
        handleAction();
        handleAttack();
        handleRoll();

        if(currentMovement.magnitude >= 0.1)
        {
            TakingEnd(); //сброс анимаций, которые не мувмент



            float targetAngle = Mathf.Atan2(currentMovement.x, currentMovement.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            if(!isWalkPressed)
            {
                characterController.Move(moveDir.normalized * playerRunSpeed * Time.deltaTime);
            }
            else
            {
                characterController.Move(moveDir.normalized * playerSpeed * Time.deltaTime);
            }
        }

        handleGravity();
        handleJump();
        
        characterController.Move(playerVelocity * Time.deltaTime);
    }



    private void OnEnable() 
    {
        playerInput.CharacterControls.Enable();
    }
    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }

    public void GroundSet(bool state)
    {
        _isGround = state;
    }

    public void TakingEnd()
    {
         animator.SetBool(isActionHash, false);
    }
    public void RollEnd()
    {
        Debug.Log("RollEnd");
         animator.SetBool(isRolloverHash, false);
         isRoll = false;
    }
}
