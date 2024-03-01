using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    [Header("VisionCone - OnlyVisual")]
    public Material VisionConeMaterial;
    public float visionRange;
    public float attackRange;
    public float VisionAngle;
    public LayerMask VisionObstructingLayer;
    public int VisionConeResolution = 120;
    Mesh VisionConeMesh;
    MeshFilter MeshFilter_;
    public bool playerDetected = false;
    public bool attackPlayer = false;


    [Header("FieldOfView - Real Detection")]
    public float radius;
    public float attackRadius;
    [Range(0,360)]
    public float angle;
    public GameObject playerRef;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public bool canSeePlayer;
    public bool canAttackPlayer;

    public EnemyAutoMovement enemyAutoMovement;

    public Vector3 originalPlayerWayPointPosition;
    public Transform playerWayPoint;
    public Transform playerGhost;

    void Start()
    {
        //Start OnlyVisual
        transform.AddComponent<MeshRenderer>().material = VisionConeMaterial;
        MeshFilter_ = transform.AddComponent<MeshFilter>();
        VisionConeMesh = new Mesh();
        VisionAngle *= Mathf.Deg2Rad;

        //Start Real Detection
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
        angle = VisionAngle * Mathf.Rad2Deg;
        radius = visionRange;

        enemyAutoMovement= GetComponentInParent<EnemyAutoMovement>();

        originalPlayerWayPointPosition = new Vector3(50, 50, 50);
        playerWayPoint.position = originalPlayerWayPointPosition;
        
    }




    void Update()
    {
        DrawVisionCone();
        //DrawAttackCone();
    }

    void DrawVisionCone()
    {
        int[] triangles = new int[(VisionConeResolution - 1) * 3];

        Vector3[] Vertices = new Vector3[VisionConeResolution + 1];

        Vertices[0] = Vector3.zero;

        float Currentangle = -VisionAngle / 2;

        float angleIcrement = VisionAngle / (VisionConeResolution - 1);

        float Sine;

        float Cosine;


        for (int i = 0; i < VisionConeResolution; i++)
        {
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);


            Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);

            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);

            if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit ray, visionRange, VisionObstructingLayer))
            {
                Vertices[i + 1] = VertForward * ray.distance;
            

            }
            else
            {
                Vertices[i + 1] = VertForward * visionRange;
                
            }           

            Currentangle += angleIcrement;
        }

        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }

        VisionConeMesh.Clear();
        VisionConeMesh.vertices = Vertices;
        VisionConeMesh.triangles = triangles;
        MeshFilter_.mesh = VisionConeMesh;
    }

   /* void DrawAttackCone()
    {
        int[] triangles = new int[(VisionConeResolution - 1) * 3];

        Vector3[] Vertices = new Vector3[VisionConeResolution + 1];

        Vertices[0] = Vector3.zero;

        float Currentangle = -VisionAngle / 2;

        float angleIcrement = VisionAngle / (VisionConeResolution - 1);

        float Sine;

        float Cosine;


        for (int i = 0; i < VisionConeResolution; i++)
        {
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);


            Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);

            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);

            if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, attackRange, VisionObstructingLayer))
            {
                Vertices[i + 1] = VertForward * hit.distance;
                if (hit.transform.gameObject.CompareTag("Player"))
                {
                    attackPlayer = true;
                    Debug.Log("attack");
                }
                else { attackPlayer = false; }

            }
            else
            {
                Vertices[i + 1] = VertForward * attackRange;
            }

            Currentangle += angleIcrement;
        }
        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }

        VisionConeMesh.Clear();
        VisionConeMesh.vertices = Vertices;
        VisionConeMesh.triangles = triangles;
        MeshFilter_.mesh = VisionConeMesh;


    }*/


    //Coroutine de detection du joueur
    private IEnumerator FOVRoutine()
    {        
        WaitForSeconds wait = new WaitForSeconds(.2f);
        while(true)
        {
            yield return wait;
            
            FieldOfViewCheck();

            FieldOfViewAttackCheck();
        }
    }

    private void FieldOfViewCheck()
    {
               Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if(rangeChecks.Length != 0 ) 
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directiontoTarget = (target.position - transform.position).normalized;

            if(Vector3.Angle(transform.forward, directiontoTarget) < angle/2)
            {
                float distancetotarget = Vector3.Distance(transform.position, target.position);
                if(!Physics.Raycast(transform.position, directiontoTarget, distancetotarget, obstructionMask))
                {
                    canSeePlayer = true;
                    
                    //Change la pos du WayPoint en cours pour aller a la derniere position connue du joueur
                    playerWayPoint.position = GameObject.Find("Player").transform.position;
                    playerGhost.position = GameObject.Find("Player").transform.position;


                }
                else { canSeePlayer = false; }

            }
            else
            {
                canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
        }
    }

    private void FieldOfViewAttackCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, attackRadius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directiontoTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directiontoTarget) < angle / 2)
            {
                float distancetotarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directiontoTarget, distancetotarget, obstructionMask))
                {
                    canAttackPlayer = true;
                    canSeePlayer = false;
                    enemyAutoMovement.agent.speed = 0f;
                    
                }
                else { canAttackPlayer = false; }

            }
            else
            {
                canAttackPlayer = false;
            }
        }
        else if (canAttackPlayer)
        {
            canAttackPlayer = false;
        }
    }
}
