using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class EnemyAutoMovement : MonoBehaviour
{
    public GameObject player;
    public NavMeshAgent agent;
    private Animator animator;
    public List<Transform> wayPoints;
    public int currentWayPointIndex = 0;
    public int walkSpeed = 3;

    private Vector3 oldPos;
    private Quaternion oldRot;
    
    public VisionCone visionCone;
    public Enemy enemyScript;

    Vector3 randomDeltaLookPosition;
    Vector3 lookZoneCenter;
    Vector3 randomLookPosition;
    bool newDestinationReached = false;
    bool newnewDestinationReached = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isWalking", true);
        enemyScript = GetComponent<Enemy>();
    }
    private void Update()
    {
        if (enemyScript.cantMove == false)
        {
            Animation();
        }
    }

    void RandomWalk()
    {
        if (visionCone.playerWayPoint.position != visionCone.originalPlayerWayPointPosition && Vector3.Distance(visionCone.playerWayPoint.position, transform.position)<=20)
        {
            ShiftDirection();
        }
        else 
        {
            if (wayPoints.Count == 0)
            {
                return;
            }

            float distanceToWayPoint = Vector3.Distance(wayPoints[currentWayPointIndex].position, transform.position);

            if (distanceToWayPoint <= 3)
            {
                currentWayPointIndex = Random.Range(0, 4); /*(currentWayPointIndex + 1) % wayPoints.Count;*/
            }

            agent.SetDestination(wayPoints[currentWayPointIndex].position);
        }
    }
    void WalktoPlayer()
    {        
         agent.destination = player.transform.position;        
    }

    void Animation()
    {
        if (visionCone.canSeePlayer == false /*&& visionCone.attackPlayer == false*/)
        {
            agent.speed = walkSpeed;
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
            RandomWalk();
        }
        else if (visionCone.canSeePlayer == true /*&& visionCone.attackPlayer == false*/)
        {
            agent.speed = walkSpeed+3;
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", true);
            animator.SetBool("isAttacking", false);
            WalktoPlayer(); 
        }
        
        if (visionCone.canAttackPlayer == true)
        {
            agent.speed = 0;
            animator.SetBool("isAttacking", true);
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false);
        }

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            agent.speed = 0;
        }      
    }

    void ShiftDirection()
    {
        //Va a l'endroit ou le joueur a ete vu la derniere fois : Vector3
        agent.destination = visionCone.playerWayPoint.position;
        //Endroit ou il a été vu: Vector3
        lookZoneCenter = visionCone.playerWayPoint.position;
        //Calcul de la distance entre lookZoneCenter et sa pos pour savoir quand il l'atteint
        float distanceFromLastPlayerDetection = Vector3.Distance(visionCone.playerWayPoint.position, transform.position);
        // Quand il l'atteint et qu'il est pas en mode attaque:
        if (distanceFromLastPlayerDetection < 1 && visionCone.canAttackPlayer == false)
        {
            StartCoroutine(LookAroundPlayer());
            newDestinationReached = true;
            //StartCoroutine(HasReachedDestination());
           
        }

        if (newDestinationReached == true)
        {
                      
            //On défini  un nouveau vecteur (aléatoire) pour créer un nouvel itinéraire
            randomDeltaLookPosition = new Vector3(-10, 0, 6);

            //Recalcul de son itinéraire en prenant le centre du dernier point connu + un vecteur aléatoire avec une range definie
            randomLookPosition = lookZoneCenter + randomDeltaLookPosition;
            //Set up la nouvelle destination
            agent.destination = randomLookPosition;

            if(Vector3.Distance(randomLookPosition, transform.position) < 1)
            {
                StartCoroutine(LookAroundPlayer());

                newnewDestinationReached = true;

                
                
            }

            if (newnewDestinationReached == true)
            {
                randomDeltaLookPosition = new Vector3(10, 0, 6);

                //Recalcul de son itinéraire en prenant le centre du dernier point connu + un vecteur aléatoire avec une range definie
                randomLookPosition = lookZoneCenter + randomDeltaLookPosition;
                //Set up la nouvelle destination
                agent.destination = randomLookPosition;

                if (Vector3.Distance(randomLookPosition, transform.position) < 1)
                {
                    StartCoroutine(LookAroundPlayer());
                }

                //Coroutine qui le fait revenir a la position de depart apres 5s
                StartCoroutine(WaitAndReplaceWayPoint());
            }
        }

        
    }

    IEnumerator LookAroundPlayer()
    {
       
            animator.SetBool("isWalking", false);
            animator.SetBool("Look Around", true);
            agent.speed = 0;
            yield return new WaitForSeconds(4.14f);
            animator.SetBool("isWalking", true);
            animator.SetBool("Look Around", false);
            agent.speed = walkSpeed;
        

    }
    IEnumerator WaitAndReplaceWayPoint()
    {
        yield return new WaitForSeconds(10);
        visionCone.playerWayPoint.position = visionCone.originalPlayerWayPointPosition;
        newDestinationReached = false;
        newnewDestinationReached = false;
    }
    IEnumerator HasReachedDestination()
    {
        yield return new WaitForSeconds(20);
        newDestinationReached = false;
        newnewDestinationReached = false;
    }
    IEnumerator aiSync()
    {
       
        while(true)
        { 
            if(transform.position != oldPos || transform.rotation != oldRot)
            {
                DataSyncAI dataSync = new DataSyncAI();
                dataSync.aiId = 1;
                dataSync.aiPos = transform.position;
                dataSync.aiRot = transform.rotation;
                string content = JsonUtility.ToJson(dataSync);
            }
        
        }
        yield return new WaitForSeconds(0.033f);
    }

}


[System.Serializable]
public class DataSyncAI
{
    public int aiId;
    public Vector3 aiPos;
    public Quaternion aiRot;
}


