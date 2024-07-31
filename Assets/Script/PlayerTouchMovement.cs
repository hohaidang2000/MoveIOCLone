using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;
public class PlayerTouchMovement : MonoBehaviour
{
    [SerializeField]
    private Vector2 JoystickSize = new Vector2(300, 300);
    [SerializeField]
    private FloatingJoystick Joystick;
    [SerializeField]
    private CharacterController characterController;

    [SerializeField] NavMeshAgent agent;

    [SerializeField] SphereCollider sphereCollider;
    [SerializeField] BoxCollider boxCollider;

    bool isMovementPressed = false;
    [SerializeField] float speed = 5f;
    private float speedBase;
    float rotationFactorPerFrame = 5.0f;
    private Transform enemyTransform;
    bool isAttack = false;
    bool hasAttack = false;

    private Transform characterTransform;
    private Animator animator;
    private float scale = 1f;

    private Vector3 scaledMovement;
    private Vector3 MovementAmount;
    private Finger MovementFinger;

    [SerializeField] GameObject projectileSpawnLocation;
    [SerializeField] GameObject hammerThrow;

    [SerializeField] float firerate = 1f;
    private float firerateTimeStamp;


    [SerializeField] float attackAnimationTime;
    private float attackAnimationTimeStamp;

    private HashSet<Collider> _objectsInTrigger;
    private GameObject lockOnTarger;

    public bool isDeath = false;
    [SerializeField] GameplaySystem gameplaySystem;

    bool turnBig = false;
    private void Awake()
    {


        //characterController = GetComponentInChildren<CharacterController>();

    }
    private void Start()
    {
        characterTransform = GameObject.FindWithTag("PlayerModel").transform;
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        attackAnimationTime = firerate - 0.2f;

        _objectsInTrigger = new HashSet<Collider>();
        gameplaySystem = GameObject.FindWithTag("GameplaySystem").GetComponent<GameplaySystem>();
        speedBase = speed;
    }
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleLoseFinger;
        ETouch.Touch.onFingerMove += HandleFingerMove;
        sphereCollider.enabled = true;
        boxCollider.enabled = true;
    }
    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleLoseFinger;
        ETouch.Touch.onFingerMove -= HandleFingerMove;
        EnhancedTouchSupport.Disable();
        sphereCollider.enabled = false;
        boxCollider.enabled = false;
    }
    private void HandleFingerDown(Finger TouchedFinger)
    {
        if (MovementFinger == null /*&& TouchedFinger.screenPosition.x <= Screen.width / 2f*/)
        {
            MovementFinger = TouchedFinger;
            MovementAmount = Vector2.zero;
            Joystick.gameObject.SetActive(true);
            Joystick.RectTransform.sizeDelta = JoystickSize;
            Joystick.RectTransform.anchoredPosition = ClampStartPosition(TouchedFinger.screenPosition);

        }
    }

    private Vector2 ClampStartPosition(Vector2 StartPosition)
    {
        if (StartPosition.x < JoystickSize.x / 2)
        {
            StartPosition.x = JoystickSize.x / 2;
        }
        if (StartPosition.y < JoystickSize.y / 2)
        {
            StartPosition.y = JoystickSize.y / 2;
        } else if (StartPosition.y > Screen.height - JoystickSize.y / 2)
        {
            StartPosition.y = Screen.height - JoystickSize.y / 2;
        }
        return StartPosition;
    }
    private void HandleFingerMove(Finger MovedFinger)
    {
        if (MovedFinger == MovementFinger)
        {
            Vector2 knobPositoin;
            float maxMovement = JoystickSize.x / 2f;
            ETouch.Touch currentTouch = MovedFinger.currentTouch;

            if (Vector2.Distance(
                    currentTouch.screenPosition,
                    Joystick.RectTransform.anchoredPosition
                ) > maxMovement)
            {
                knobPositoin = (
                    currentTouch.screenPosition - Joystick.RectTransform.anchoredPosition
                    ).normalized
                    * maxMovement;
            }
            else
            {
                knobPositoin = currentTouch.screenPosition - Joystick.RectTransform.anchoredPosition;
            }

            Joystick.Knob.anchoredPosition = knobPositoin;
            MovementAmount = knobPositoin / maxMovement;
           
            isMovementPressed = true;
            hasAttack = false;
            firerateTimeStamp = Time.time;

        }
    }



    private void HandleLoseFinger(Finger LostFinger)
    {
        if (LostFinger == MovementFinger)
        {
            MovementFinger = null;
            Joystick.Knob.anchoredPosition = Vector2.zero;
            Joystick.gameObject.SetActive(false);
            MovementAmount = Vector2.zero;
            isMovementPressed = false;
        }
    }
    void handleRotation()
    {
        Vector3 positiontoLookAt;
        positiontoLookAt.x = MovementAmount.x;
        positiontoLookAt.y = 0.0f;
        positiontoLookAt.z = MovementAmount.z;

        //?
        //Quaternion currentRotation = transform.rotation;
        Quaternion currentRotation = characterTransform.rotation;
        if (isAttack)
        {
            if (GetClosestObjectInTrigger() != null)
            {
                
                enemyTransform = GetClosestObjectInTrigger().transform;


                Vector3 fixEnemyTransform = new Vector3(enemyTransform.position.x, characterTransform.position.y, enemyTransform.position.z);
                //fixEnemyTransform.y = 0.0f;

                //Quaternion targetRotation = Quaternion.LookRotation(enemyTransform.position - characterTransform.position);
                Quaternion targetRotation = Quaternion.LookRotation(fixEnemyTransform - characterTransform.position);
                characterTransform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame);
            }
        }
        if (isMovementPressed)
        {
            characterTransform.LookAt(characterTransform.transform.position + scaledMovement, Vector3.up); 
        }

    }
    void handleANimation()
    {
 
        if (isMovementPressed)
        {
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsAttack", false);
        }
        if (!isMovementPressed)
        {
            animator.SetBool("IsIdle", true);
        }
        if (!isMovementPressed && !isAttack)
        {
            animator.SetBool("IsIdle", true); 
            animator.SetBool("IsAttack", false) ;
        }

    }
    public void GetBig()
    {
        scale = scale + 0.1f;
        gameObject.transform.localScale = new Vector3(scale,scale,scale);
        speed = speedBase * scale;
        
    }
    void Attack()
    {
        if(hasAttack == true)
        {
            if (Time.time > attackAnimationTimeStamp + attackAnimationTime)
            {
                animator.SetBool("IsIdle", true);
                animator.SetBool("IsAttack", false);
            }
            if (isAttack && !isMovementPressed && Time.time > firerateTimeStamp + firerate)
            {
                //animator.SetBool("IsAttack", false);
                animator.SetBool("IsIdle", false);
                animator.SetBool("IsAttack", true);

                firerateTimeStamp = Time.time;
                attackAnimationTimeStamp = Time.time;

                GameObject projectile = Instantiate(hammerThrow, projectileSpawnLocation.transform.position, hammerThrow.transform.rotation) as GameObject;
                projectile.GetComponent<SelfYeet>().playerThrower = gameObject.GetComponent<PlayerTouchMovement>();
                projectile.GetComponent<SelfYeet>().enabled = true;
                projectile.transform.localScale = new Vector3(scale*20, scale * 20, scale*20);
                projectile.GetComponent<Rigidbody>().velocity = characterTransform.forward * speed;
            }
        }
        else
        {
            if (isAttack && !isMovementPressed)
            {


                //animator.SetBool("IsAttack", false);
                animator.SetBool("IsIdle", false);
                animator.SetBool("IsAttack", true);

                firerateTimeStamp = Time.time;
                attackAnimationTimeStamp = Time.time;

                hasAttack = true;
                
                GameObject projectile = Instantiate(hammerThrow, projectileSpawnLocation.transform.position, hammerThrow.transform.rotation) as GameObject;
                projectile.GetComponent<SelfYeet>().playerThrower = gameObject.GetComponent<PlayerTouchMovement>(); ;
                projectile.GetComponent<SelfYeet>().enabled = true;
                projectile.transform.localScale = new Vector3(scale*20, scale*20, scale*20);
                projectile.GetComponent<Rigidbody>().velocity = characterTransform.forward * speed;
            }
        }

        
    }
    GameObject GetClosestObjectInTrigger()
    {
        GameObject closestObject = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        if(lockOnTarger != null)
        {
            lockOnTarger.GetComponentInParent<EnemyAI>().LockedOFF();
        }
        
        foreach (Collider objectInTrigger in _objectsInTrigger)
        {

            float sqrDistanceToObject = (objectInTrigger.transform.position - currentPosition).sqrMagnitude;
            if (sqrDistanceToObject < closestDistanceSqr)
            {
                closestDistanceSqr = sqrDistanceToObject;
                closestObject = objectInTrigger.gameObject;
                lockOnTarger = objectInTrigger.gameObject;
                
            }
        }
        if (lockOnTarger != null)
        {
            lockOnTarger.GetComponentInParent<EnemyAI>().LockedON();
        }
        return closestObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyModel" &&  other.GetComponentInParent<EnemyAI>().isDeath == false)
        {
            enemyTransform = other.transform;
            //Debug.Log("Enter");
            isAttack = true;
            if (_objectsInTrigger.Count == 0)
            {
                firerateTimeStamp = Time.time - 2*firerate;
            }
            
            _objectsInTrigger.Add(other);
            //Attack();
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "EnemyModel")
        {
            if(other.GetComponentInParent<EnemyAI>().isDeath == false)
            {
                enemyTransform = other.transform;
                lockOnTarger = other.GameObject();
                //Debug.Log("Stay");
                isAttack = true;
                Attack();
            }
            else
            {
                _objectsInTrigger.Remove(other);
               
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "EnemyModel")
        {

            //Debug.Log("Leave");
            isAttack = false;
            if (_objectsInTrigger.Count == 0)
            {
                hasAttack = false;
            }
            other.GetComponentInParent<EnemyAI>().LockedOFF(); 
            _objectsInTrigger.Remove(other);
        }
    }
    public void Death()
    {
        isDeath = true;
        isAttack = false;
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsAttack", false);
        animator.SetBool("IsDead", true);
        characterController.enabled = false;
        FunctionTimer.Create(RemoveSelf, 2f, "Self Removed");
    }
    private void RemoveSelf()
    {

    }
    public void Win()
    {
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsAttack", false);
        animator.SetBool("IsWin", true);
    }
    private void Update()
    {
       
        if(isDeath == false)
        {
            scaledMovement = speed * Time.deltaTime * new Vector3(
            MovementAmount.x,
            0,
            MovementAmount.y
            );

            handleRotation();
            handleANimation();
            characterController.Move(scaledMovement);
            transform.position = new Vector3(transform.position.x, 0,transform.position.z);
        }
        

    }
    
}
