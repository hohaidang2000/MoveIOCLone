using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInput playerInput;
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool isMovementPressed = false;
    [SerializeField] float speed = 4f;
    float rotationFactorPerFrame = 5.0f;

    private Transform enemyTransform;

    bool isAttack=false;
    bool isRepeatAttack=false;
    public Transform characterTransform;
    public Animator animator;
    public CharacterController characterController;

    
    // Start is called before the first frame update
    private void Awake()
    {

        characterTransform = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        //characterController = GetComponentInChildren<CharacterController>();
        playerInput = new PlayerInput();
        playerInput.GamePlay.Walk.started += ctx => {
            onMovementInput(ctx);

            Debug.Log(ctx.ReadValue<Vector2>()); 
        };

        playerInput.GamePlay.Walk.canceled += ctx =>
        {
            onMovementInput(ctx);

        };
        playerInput.GamePlay.Walk.performed += ctx =>
        {
            onMovementInput(ctx);

        };
    }
    void handleRotation()
    {
        Vector3 positiontoLookAt;
        positiontoLookAt.x = currentMovement.x;
        positiontoLookAt.y = 0.0f;
        positiontoLookAt.z = currentMovement.z;

        //?
        //Quaternion currentRotation = transform.rotation;
        Quaternion currentRotation = characterTransform.rotation;
        if (isAttack)
        {
            Vector3 fixEnemyTransform = enemyTransform.position;
            //fixEnemyTransform.y = 0.0f;

            Quaternion targetRotation = Quaternion.LookRotation(enemyTransform.position- characterTransform.position);
            characterTransform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame );
        }
        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positiontoLookAt);
            //transform.rotation =Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame);  
            characterTransform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }

    }
    void onMovementInput(InputAction.CallbackContext ctx)
    {
        currentMovementInput = ctx.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }
    //Update được gọi vào mỗi Frame
    void handleANimation()
    {
       
        if (!isMovementPressed && isAttack)
        {
           
            animator.SetBool("IsIdle", false);
            
            animator.SetBool("IsAttack", true);
            
            
            //animator.Play("Attack");


        }
        if (isMovementPressed)
        {
            
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsAttack", false);
            
        }
        if (!isMovementPressed && !isAttack)
        {
            animator.SetBool("IsIdle", true);
            animator.SetBool("IsAttack", false);
        }
    }
    private void Update()
    {
        handleRotation();
        handleANimation();
        characterController.Move(currentMovement * Time.deltaTime * speed);
        
    }
    //New Input yêu cầu phải có OnEnable và OnDisable để bật tắt
    void OnEnable()
    {
        playerInput.GamePlay.Enable();
    }
    void OnDisable()
    {
        playerInput.GamePlay.Disable();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            enemyTransform = other.transform;
            Debug.Log("saw");
            isAttack = true;
        }   
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            enemyTransform = other.transform;
            Debug.Log("saw2");
            isAttack = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
           
            Debug.Log("saw");
            isAttack = false;
        }
    }

}
