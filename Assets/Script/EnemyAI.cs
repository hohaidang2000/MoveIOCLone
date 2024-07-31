using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] GameplaySystem gameplaySystem;
    public bool isWin = false;
    [SerializeField] Transform player;
    [SerializeField] Transform characterTransform;
    private float scale = 1f;
    [SerializeField] float speed = 5;
    private float speedBase;

    float rotationFactorPerFrame = 5.0f;
    [SerializeField] float EnemyRange = 20f;
    [SerializeField] float range = 30f;
    private float distanceBetweenTarget;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    [SerializeField] GameObject projectileSpawnLocation;
    [SerializeField] GameObject hammerThrow;

    [SerializeField] float firerate;
    private float firerateTimeStamp;
    [SerializeField] float projectileSpeed = 4f;

    [SerializeField] float attackAnimationTime;
    private float attackAnimationTimeStamp;

    [SerializeField] private GameObject gameObject;
    [SerializeField] private GameObject markUI;
    // Start is called before the first frame update

    public LayerMask whatIsGround, whatIsPlayer;

    public Vector3 walkPoint;
    bool walkPointSet;
    bool chasingPlayer;
    public float walkPointRange;

    Vector3 destPoint;

    private Rigidbody rb;

    [SerializeField] float descicionTimer = 10f;
    private float reDescicionTimer;
    [SerializeField] float patrolingCooldown = 4f;
    private float rePatrolingCooldown;


    public bool isDeath = false;

    private Transform enemyTransform;
    private bool isAttack = false;

    private HashSet<Collider> _objectsInTrigger;
    private void Awake()
    {

        player = GameObject.Find("Player").transform;

        characterTransform = GetComponent<Transform>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        reDescicionTimer = descicionTimer;
        rePatrolingCooldown = patrolingCooldown;

        _objectsInTrigger = new HashSet<Collider>();
        rb = GetComponent<Rigidbody>();

        gameplaySystem = GameObject.Find("GameplaySystem").GetComponent<GameplaySystem>();
       
        //gameObject = GetComponent<GameObject>();
    }

    private void Patroling()
    {

        if (!walkPointSet && !chasingPlayer)
        {
            float z = Random.Range(-range, range);
            float x = Random.Range(-range, range);
            destPoint = new Vector3(x, 0, z);
            walkPointSet = true;


        }
        if (walkPointSet && !chasingPlayer)
        {
            animator.SetBool("IsIdle", false);
            if (Vector3.Distance(transform.position, destPoint) < 5)
            {

                walkPointSet = false;

                animator.SetBool("IsIdle", true);
            }
            else
            {
                navMeshAgent.SetDestination(destPoint);
            }

        }

    }
    private void ChasePlayer()
    {


        chasingPlayer = true;

        float z = player.position.z;
        float x = player.position.x;
        destPoint = new Vector3(x, 0, z);

        if (Vector3.Distance(transform.position, destPoint) < 7)
        {

            chasingPlayer = false;

            //animator.SetBool("IsIdle", true);
        }
        else
        {
            animator.SetBool("IsIdle", false);
            navMeshAgent.SetDestination(player.position);
        }



    }
    public void LockedON()
    {
        markUI.SetActive(true);
    }
    public void LockedOFF()
    {
        markUI.SetActive(false);
    }
    public void Death()
    {
        isDeath = true;
        isAttack = false;
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsAttack", false);
        animator.SetBool("IsDead", true);
        //BoxCollider box = GetComponent<BoxCollider>();
        //box.enabled = false;
        navMeshAgent.isStopped = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        FunctionTimer.Create(RemoveSelf, 2f, "Self Removed");
    }
    private void RemoveSelf()
    {
        Debug.Log("RemoveSelf");
        //gameObject.SetActive(false);
        Destroy(gameObject);

    }
    private void AttackAim()
    {


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
    }
    public void GetBig()
    {
        scale = scale + 0.1f;
        gameObject.transform.localScale = new Vector3(scale, scale, scale);
        navMeshAgent.speed = speedBase * scale;
    }
    void Attack()
    {

        if (Time.time > attackAnimationTimeStamp + attackAnimationTime)
        {
            animator.SetBool("IsIdle", true);
            animator.SetBool("IsAttack", false);
        }
        if (isAttack && Time.time > firerateTimeStamp + firerate)
        {
            //animator.SetBool("IsAttack", false);
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsAttack", true);

            firerateTimeStamp = Time.time;
            attackAnimationTimeStamp = Time.time;

            GameObject projectile = Instantiate(hammerThrow, projectileSpawnLocation.transform.position, hammerThrow.transform.rotation) as GameObject;
            projectile.GetComponent<SelfYeet>().AIThrower = gameObject.GetComponent<EnemyAI>();
            projectile.GetComponent<SelfYeet>().enabled = true;
            projectile.transform.localScale = new Vector3(scale * 20, scale * 20, scale * 20);
            projectile.GetComponent<Rigidbody>().velocity = characterTransform.forward * speed*scale;
        }
    }

    void Start()
    {

        animator.SetBool("IsIdle", true);
        firerate = Random.Range(1.0f, 2f);

        speedBase = speed;
        if (gameplaySystem.wave.start)
        {
            Patroling();
        }

    }
    GameObject GetClosestObjectInTrigger()
    {
        GameObject closestObject = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        if (_objectsInTrigger.Count > 0)
        {
            foreach (Collider objectInTrigger in _objectsInTrigger)
            {

                float sqrDistanceToObject = (objectInTrigger.transform.position - currentPosition).sqrMagnitude;
                if (sqrDistanceToObject < closestDistanceSqr)
                {
                    closestDistanceSqr = sqrDistanceToObject;
                    closestObject = objectInTrigger.gameObject;

                }
            }


        }
        return closestObject;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyModel" && other.GetComponentInParent<EnemyAI>().isDeath == false)
        {
            enemyTransform = other.transform;
            Debug.Log("BotEnter");
            isAttack = true;

            firerateTimeStamp = Time.time;

            _objectsInTrigger.Add(other);
            //Attack();
        }
        if (other.tag == "PlayerModel" && other.GetComponentInParent<PlayerTouchMovement>().isDeath == false)
        {
            //enemyTransform = other.transform;
            Debug.Log("PlayerEnter");
            isAttack = true;
            firerateTimeStamp = Time.time;
            _objectsInTrigger.Add(other);
            //Attack();
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "EnemyModel")
        {

            if (other.GetComponentInParent<EnemyAI>().isDeath == false)
            {
                //enemyTransform = other.transform;

                //Debug.Log("BotStay");
                isAttack = true;
                //Attack();
            }
            else
            {
                isAttack = false;
                _objectsInTrigger.Remove(other);
            }

        }
        if (other.tag == "PlayerModel")
        {
            if (other.GetComponentInParent<PlayerTouchMovement>().isDeath == false)
            {
                //enemyTransform = other.transform;

                //Debug.Log("PLayerStay");
                isAttack = true;
                //Attack();
            }
            else
            {
                isAttack = false;
                _objectsInTrigger.Remove(other);
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "EnemyModel")
        {

            Debug.Log("BotLeave");
            isAttack = false;
            _objectsInTrigger.Remove(other);
        }
        if (other.tag == "PlayerModel")
        {
            Debug.Log("PlayerLeave");
            isAttack = false;
            _objectsInTrigger.Remove(other);
        }
    }
    void Update()
    {
        if (gameplaySystem.isPausing == true)
        {
            
            if (gameplaySystem.isDeath && !isDeath && isWin == false)
            {
                Debug.Log("Dance");
                rb.velocity = Vector3.zero;
                navMeshAgent.velocity = Vector3.zero;
                navMeshAgent.Stop();
                animator.SetBool("IsIdle", false);
                animator.SetBool("IsAttack", false);
                animator.SetBool("IsWin", true);
                isWin = true;
            }
        }
        else
        {


            //navMeshAgent.SetDestination(player.transform.position);
            if (!isDeath)
            {
                if (isAttack == false)
                {
                    navMeshAgent.Resume();
                    descicionTimer -= Time.deltaTime;
                    patrolingCooldown -= Time.deltaTime;
                    if (navMeshAgent.velocity.magnitude < 1f)
                    {
                        animator.SetBool("IsIdle", true);
                    }
                    else
                    {
                        animator.SetBool("IsIdle", false);
                    }

                    if (patrolingCooldown <= 0)
                    {
                        patrolingCooldown = rePatrolingCooldown;
                        Patroling();
                    }
                    if (descicionTimer <= 0)
                    {
                        descicionTimer = reDescicionTimer;

                        ChasePlayer();
                    }
                }
                else
                {
                    rb.velocity = Vector3.zero;
                    navMeshAgent.velocity = Vector3.zero;
                    navMeshAgent.Stop();
                    AttackAim();
                    Attack();
                }

            }
        }

    }


}
