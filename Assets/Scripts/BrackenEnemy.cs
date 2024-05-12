using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class BrackenEnemy : MonoBehaviour
{
    public enum BrackenStates
    {
        Hunt,Retreat,Agro
    }
    [Header("Speed For Various States")]
    [SerializeField] private float brackenHuntMoveSpeed = 10;
    [SerializeField] private float brackenRetreatMoveSpeed = 10;
    [SerializeField] private float brackenAgroMoveSpeed = 20;
    [Space]
    [Header("DistanceToKeep For Various States")]
    [SerializeField] private float distanceToKeepForHunt = 10f;
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


        switch(brackenStates)
        {
            case BrackenStates.Hunt:

                HandleHuntLogics();

                break;
            case BrackenStates.Retreat:

                HandleRetreatLogics();

                break;
            case BrackenStates.Agro:

                HandleAgroLogics();

                break;
        }

       
    }

    private void HandleHuntLogics()
    {
        if (IsPlayerBehind(GetDotBetWeen(target)))
        {
            // Bracken IsBehind Player So Can Follow
            agent.SetDestination(target.position);
            agent.stoppingDistance = distanceToKeepForHunt;
            agent.speed = brackenHuntMoveSpeed;
        }
        else
        {
            // Bracken is not behind the player, so it needs to hide
        }
    }

    private void HandleRetreatLogics()
    {
        agent.SetDestination(IsPlayerInsideKeepDistance(distanceToKeepForRetreat) ? brackenSpawnPosition : transform.position);
        agent.stoppingDistance = distanceToKeepForRetreat;
        agent.speed = brackenRetreatMoveSpeed;

        transform.LookAt(target);
    }

    private void HandleAgroLogics()
    {
        agent.SetDestination(target.position);
        agent.stoppingDistance = distanceToKeepForAgro;
        agent.speed = brackenAgroMoveSpeed;
    }

    private bool IsPlayerInsideKeepDistance(float distanceToKeep)
    {
        float distanceBtBrackenToPlayer = Vector3.Distance(transform.position, target.position);
        return distanceBtBrackenToPlayer < distanceToKeep;
    }

    private float GetDotBetWeen(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        return Vector3.Dot(target.forward, dir.normalized);
    }
    private bool IsPlayerBehind(float dotValue)
    {
        return dotValue >= 0f;
    }

    private Collider[] GetBoxCollidersNearby(Transform center, float radius = 10f)
    {
        Collider[] colliders = Physics.OverlapSphere(center.position, radius);

        return colliders.Where(c => c.GetType() == typeof(BoxCollider)).ToArray();
    }

    private Collider[] GetBoxCollidersNotInPlayerFOV(Collider[] playerNearColliders, Transform playerTransform)
    {
        Collider[] collidersNotInFOV = playerNearColliders.Where(collider =>
        {
            Vector3 directionToCollider = collider.transform.position - playerTransform.position;

            float dotProduct = Vector3.Dot(playerTransform.forward, directionToCollider.normalized);

            return dotProduct < 0f;

        }).ToArray();

        return collidersNotInFOV;
    }

}
