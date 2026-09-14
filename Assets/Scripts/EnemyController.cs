using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    float speed = 2f;
    //Rigidbody rb;
    NavMeshAgent agent;
    Animator anim;

    [SerializeField]
    List<Transform> waypoints = new List<Transform>();
    [SerializeField]
    float waitTimeAtPoint = 3f;

    int currentWaypointIndex = 0;
    bool isWaiting = false;

    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        if (waypoints == null || waypoints.Count == 0) return;

        GoToCurrentWaypoint();
    }

    void Update()
    {
        if (waypoints.Count == 0 || isWaiting) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            anim.SetTrigger("Stop");
            agent.speed = 0;
            StartCoroutine(WaitAtWaypoint());
        }
    }

    void GoToCurrentWaypoint()
    {
        if (waypoints.Count == 0) return;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    void SelectNextWayPoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTimeAtPoint);
        SelectNextWayPoint();
        anim.SetTrigger("Walk");
        agent.speed = speed;
        GoToCurrentWaypoint();
        isWaiting = false;
    }

    void FixedUpdate()
    {
        //Vector3 forwardMove = transform.forward * speed;
        //rb.linearVelocity = new Vector3(forwardMove.x, rb.linearVelocity.y, forwardMove.z);
    }

}
