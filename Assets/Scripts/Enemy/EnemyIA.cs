using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType{
    gunner,
    runner,
    sniper,
}

public class EnemyIA : MonoBehaviour
{

    [Header("Reference")]
    public EnemyType currentEnemyType;
    private NavMeshAgent agent;
    private Transform player;
    public LayerMask whatIsPlayer, whatIsGround;
    private Animator anim;
    private Enemy enemy;
    
    [Header("Attacking")]
    public float attackRange;
    public float timeBetweenAttacks;
    public bool playerInAttackRange;
    public bool alreadyAttacked;

    [Header("Patroling")]
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;
    public bool waitingAtPoint;
    public float waitingTime;
    public float currentWaitingTime;
    
    [Header("Sighting")]
    public float sightRange;
    public bool playerInSightRange;
    public float sightTime = 3f;
    public float currentSightTime;
    
    private void Awake(){
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
    }

    private void Patroling(){
        if (currentSightTime != 0) currentSightTime = 0;

        if (!walkPointSet && !waitingAtPoint) SearchWalkPoint();
        else{

            if (transform.position == walkPoint){
                waitingAtPoint = true;
                anim.SetFloat("Movement", 0, 0.1f, Time.deltaTime);
                anim.SetBool("Look Around", true);
            }
            else{
                agent.SetDestination(walkPoint);
                anim.SetFloat("Movement", 1, 0.1f, Time.deltaTime);
                anim.SetBool("Look Around", false);
                agent.speed = enemy.walkSpeed;  
            }
        }
    }

    private void SearchWalkPoint(){
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        walkPointSet = true;
    }

    private void ChasePlayer(){
        transform.LookAt(player);
        agent.SetDestination(transform.position);
        anim.SetBool("Look Around", false);
        anim.SetFloat("Movement", 0, 0.1f, Time.deltaTime);

        if (currentSightTime <= sightTime) currentSightTime += Time.deltaTime;
    }

    private void AttackPlayer(){
        if (currentEnemyType == EnemyType.runner){
            transform.LookAt(player);
            agent.SetDestination(player.position);
            agent.speed = enemy.sprintSpeed;
            waitingAtPoint = false;

            walkPoint = Vector3.zero;
            walkPointSet = false;

            if (currentSightTime != 0) ResetCurrentSightTime();
            anim.SetBool("Look Around", false);
            
            anim.SetFloat("Movement", 2, 0.1f, Time.deltaTime);
        }
        
    }

    IEnumerator ResetCurrentSightTime(){
        yield return new WaitForSeconds(1);
        currentSightTime = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        SearchWalkPoint();
        agent.SetDestination(walkPoint);
    }

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (waitingAtPoint){
            if (currentWaitingTime < waitingTime){
                currentWaitingTime += Time.deltaTime;
            }
            else{
                currentWaitingTime = 0;
                walkPointSet = false;
                waitingAtPoint = false;
            }
        }


        if (!playerInSightRange && !playerInAttackRange){
            Patroling();
        }
        if (playerInSightRange && !playerInAttackRange) {
            ChasePlayer();
        }
        if (playerInSightRange && playerInAttackRange || currentSightTime >= sightTime){
            AttackPlayer();
        }
    }
}
