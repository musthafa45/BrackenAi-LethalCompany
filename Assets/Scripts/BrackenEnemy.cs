using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BrackenBrain))]
public class BrackenEnemy : MonoBehaviour
{
    public enum BrackenStates
    {
        Follow,Hide,Retreat,Agro
    }

    [Header("Speed For Various States")]
    [SerializeField] private float brackenFollowMoveSpeed = 10;
    [SerializeField] private float brackenRetreatMoveSpeed = 10;
    [SerializeField] private float brackenAgroMoveSpeed = 20;
    [Space]
    [Header("DistanceToKeep For Various States")]
    [SerializeField] private float distanceToKeepForFollow = 10f;
    [SerializeField] private float distanceToKeepForRetreat = 10f;
    [SerializeField] private float distanceToKeepForAgro = 2f;

    [Header(" Manual State Change For Debug Purpose")]
    [SerializeField] private BrackenStates brackenStates;

    private NavMeshAgent agent;
    private Transform target; // Represents Player
    private Vector3 brackenSpawnPosition; // Spawning Position At Start

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GetNearestTarget();
    }

    private void Start()
    {
        brackenSpawnPosition = transform.position;
    }

    private Transform GetNearestTarget()
    {
        FirstPersonController[] playerTransforms = FindObjectsOfType<FirstPersonController>();
        var nearestPlayer = playerTransforms.OrderBy(tr => Vector3.Distance(transform.position, tr.transform.position)).FirstOrDefault();
        return nearestPlayer.transform;
    }

    private void Update()
    {
        if (agent == null && target == null)
        {
            Debug.LogWarning("Agent or Target References Missing");
            return;
        }

        switch (brackenStates)
        {
            case BrackenStates.Follow:

                HandleFollowLogics();

                break;
            case BrackenStates.Hide:

                break;
            case BrackenStates.Retreat:

                HandleRetreatLogics();

                break;
            case BrackenStates.Agro:

                HandleAgroLogics();

                break;
        }

    }

    private void HandleFollowLogics()
    {
        if(GetDistance(transform.position, target.position) > distanceToKeepForFollow)
        {
            agent.SetDestination(target.position);
        }
        else
        {
            agent.SetDestination(transform.position);
        }
        agent.speed = brackenFollowMoveSpeed;
    }

    private void HandleRetreatLogics()
    {
        agent.SetDestination(GetDistance(transform.position,target.position) < distanceToKeepForRetreat ? brackenSpawnPosition : transform.position);
        agent.speed = brackenRetreatMoveSpeed;

        transform.LookAt(target);
    }

    private void HandleAgroLogics()
    {
        agent.SetDestination(target.position);
        agent.speed = brackenAgroMoveSpeed;
    }

    private float GetDistance(Vector3 pointA,Vector3 pointB)
    {
        return Vector3.Distance(pointA, pointB);
    }

    private float GetDotBetWeen(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        return Vector3.Dot(target.forward, dir.normalized);
    }
    private bool IsPlayerBehind(float dotValue)
    {
        return dotValue > -0.5f;
    }
}
