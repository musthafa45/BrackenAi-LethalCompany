using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BrackenBrain))]
public class BrackenEnemyStates : MonoBehaviour
{
    public enum BrackenStates
    {
        Follow,Hide,Retreat,Agro
    }

    [Header("Speed For Various States")]
    [SerializeField] private float brackenFollowMoveSpeed = 10;
    [SerializeField] private float brackenHideMoveSpeed = 25;
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

    private HidePoint hidePoint;
    private BrackenBrain brackenBrain;

    public void SetState(BrackenStates newState)
    {
        this.brackenStates = newState;
    }

    public BrackenStates GetState()
    {
        return brackenStates;
    }
    public Transform GetTarget()
    {
        return target;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        brackenBrain = GetComponent<BrackenBrain>();

        target = brackenBrain.GetNearestTarget();
    }

    private void Start()
    {
        brackenSpawnPosition = transform.position;
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

                HandleHideLogics();
                break;
            case BrackenStates.Retreat:

                HandleRetreatLogics();

                break;
            case BrackenStates.Agro:

                HandleAgroLogics();

                break;
        }

    }

    private void HandleHideLogics()
    {
        hidePoint = HidePointManager.Instance.GetNearestNotAffectedHidePoint(transform.position);

        if(hidePoint != null)
        {
            agent.SetDestination(hidePoint.transform.position);
            agent.speed = brackenHideMoveSpeed;
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

}
