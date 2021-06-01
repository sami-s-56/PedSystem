using System.Collections;
using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(NavMeshAgent))]
public class CharacterController : MonoBehaviour
{
    Animator animator;

    //NavMeshAgent navAgent;

    [SerializeField]
    AnimatorOverrideController overrideController;

    public Waypoint wayPoint;
    public BlockScript block;

    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    
    //[SerializeField] float minDelay = 1f;    //Minimum interval between changing State
    //[SerializeField] float maxDelay = 5f;    //Maximum interval between changing State    

    [SerializeField] float stoppingDistance;
    [SerializeField] float rotationSpeed;

    //[SerializeField] bool hasAltIdle = false;
    [SerializeField] bool canRun = true;    //Set depending on whether character has running animation or not

    float speed;

    //bool isMoving;
    //bool canMove = true;   //to determine whether character can move from idle state (in case of some idle animations having exit time)

    [SerializeField]
    int direction;

    [SerializeField]
    bool isCrossing = false;    //To check if player is on crossing and manipulate according to signals
    [SerializeField] bool canBranch = true;      //To ensure that character do not recrosses the same road

    int moveState;  //To determine whether character is walking or running ( 0 : Walk, 1 : Run )

    NavMeshAgent meshAgent;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = overrideController;

        meshAgent = GetComponent<NavMeshAgent>();

        if(meshAgent == null)
        {
            print("Nav Mesh Agent not found");
        }
    }

    private void OnEnable()
    {
        direction = Random.Range(0, 2); //Set the direction of movement

        //Make character to walk or run
        if (canRun)
        {
            moveState = Random.Range(0, 2);

            if (moveState == 0)
            {
                //Walk
                animator.SetTrigger("Walk");
                speed = walkSpeed;
            }
            else
            {
                //Run
                animator.SetTrigger("Run");
                speed = runSpeed;
            }
        }
        else
        {
            moveState = 0;
            animator.SetTrigger("Walk");
            speed = walkSpeed;
        }

        Invoke("EnableNavMesh", 1f);


    }

    void OnDisable()
    {
        meshAgent.enabled = false;
    }

    private void Update()
    {
        /**OldCode
        //isMoving = false may be used in future
        //animator.ResetTrigger("Walk");
        //animator.ResetTrigger("Run");
        //animator.SetTrigger("Idle");
        //StartCoroutine("ProcessState");
        */

        //Destination Direction
        Vector3 dir = wayPoint.transform.position - transform.position;
        dir.y = 0f;

        //Destination Distance
        float destinationDistance = dir.magnitude;

        //If character reached waypoint
        if (destinationDistance <= stoppingDistance)
        {
            //Change waypoints according to all possible conditions
            ProcessWaypoints();
        }
        else //When character is on its way to next waypoint
        {
            //Move
            //Move(dir);  //Without NavMesh
        }

    }

    private void ProcessWaypoints()
    {
        //Check if there is branch and determine whether to take or not
        bool shouldBranch = false;

        //If waypoint has branches then decide weather to take branch or not and accordingly 
        if (wayPoint.branches.Count > 0 && canBranch)
        {
            shouldBranch = Random.Range(0f, 1f) <= wayPoint.branchFactor ? true : false;

            if (shouldBranch)  //If branch is taken
            {
                isCrossing = true;
                canBranch = false;
                direction = 0;  //Since crossing waypoints are made such that character on only go on next waypoint
                wayPoint = wayPoint.branches[Random.Range(0, wayPoint.branches.Count)];
            }
            else
            {
                ChangeWaypoint();
            }
        }
        else  //If branch is not present
        {
            if (!isCrossing)   //Character is on Normal Block
            {
                ChangeWaypoint();
            }
            else
            {
                if (!wayPoint.canCross && wayPoint.isCrossing) //When character chose to cross and signal is red (determined by cancross field)
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                    {
                        animator.SetTrigger("Idle");
                    }
                }
                else //When Signal is green
                {
                    wayPoint = wayPoint.nextWaypoint;
                    animator.ResetTrigger("Idle");
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                    {
                        if (moveState == 0)
                        {
                            //Walk
                            animator.SetTrigger("Walk");
                        }
                        else
                        {
                            //Run
                            animator.SetTrigger("Run");
                        }
                    }
                }
                if (!wayPoint.isCrossing)    //When character reaches another block
                {
                    isCrossing = false;
                    direction = Random.Range(0, 2);
                    block = wayPoint.block;
                }
            }
        }

        meshAgent.SetDestination(wayPoint.transform.position);
    }

    private void Move(Vector3 dir)
    {
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void ChangeWaypoint()
    {
        if (!canBranch)
        {
            canBranch = true;
        }
        if (direction == 0)
        {
            //If a path has no further waypoint (or dead end) then reverse the characters direction
            if (wayPoint.nextWaypoint == null)
            {
                direction = 1;
                wayPoint = wayPoint.previousWaypoint;

            }
            else
            {
                wayPoint = wayPoint.nextWaypoint;
            }
        }
        else
        {
            //If a path has no further waypoint (or dead end) then reverse the characters direction
            if (wayPoint.previousWaypoint == null)
            {
                direction = 0;
                wayPoint = wayPoint.nextWaypoint;

            }
            else
            {
                wayPoint = wayPoint.previousWaypoint;
            }
        }
    }

    private void EnableNavMesh()
    {
        meshAgent.enabled = true;
        meshAgent.speed = speed;
        meshAgent.angularSpeed = rotationSpeed;

        if (wayPoint != null)
        {
            meshAgent.SetDestination(wayPoint.transform.position);
        }
    }

    /** To be used if desired behavior is like the ped should stop and perform different activities at some random intervals while moving
    //int state;
    //IEnumerator ProcessState()
    //{
    //    if (!isMoving)
    //    {
    //        state = Random.Range(0, 2); // 0 for idle 1 for moving
    //        if (state == 0)
    //        {
    //            //Select Random Idle State
    //            animator.ResetTrigger("Walk");
    //            animator.ResetTrigger("Run");

    //            if (hasAltIdle)
    //            {
    //                int idleState = Random.Range(0, 2);
    //                if (idleState == 0)
    //                {
    //                    canMove = true;
    //                    animator.SetTrigger("Idle");
    //                }
    //                else
    //                {
    //                    canMove = false;
    //                    animator.SetTrigger("AltIdle");
    //                }
    //            }
    //            else
    //            {
    //                canMove = true;
    //                animator.SetTrigger("Idle");
    //            }

    //            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
    //            StartCoroutine("ProcessState");
    //        }
    //        else
    //        {
    //            if (canMove)
    //            {
    //                isMoving = true;

    //                animator.ResetTrigger("Idle");

    //                if (canRun)
    //                {
    //                    int moveState = Random.Range(0, 2);

    //                    if (moveState == 0)
    //                    {
    //                        //Walk
    //                        animator.SetTrigger("Walk");
    //                        navAgent.speed = walkSpeed;
    //                    }
    //                    else
    //                    {
    //                        //Run
    //                        animator.SetTrigger("Run");
    //                        navAgent.speed = runSpeed;
    //                    }
    //                }
    //                else
    //                {
    //                    animator.SetTrigger("Walk");
    //                    navAgent.speed = walkSpeed;
    //                }

    //                //if (wayPoint != null)
    //                //{
    //                //    Vector3 destination = wayPoint.GetPosition();
    //                //    navAgent.SetDestination(destination);
    //                //}
    //            }

    //        }
    //    }
    //    yield return new WaitForSeconds(0f);
    //}

    //Vector3 GetRandomDestination()
    //{
    //    SplinePoint[] splinePoints = destinationPointComputer.GetPoints();
    //    int randomPoint = Random.Range(0, splinePoints.Length);
    //    Vector3 randomPosition = splinePoints[randomPoint].position;
    //    return randomPosition;
    //}
    */
}

