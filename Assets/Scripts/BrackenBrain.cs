using System;
using System.Linq;
using UnityEngine;

public class BrackenBrain : MonoBehaviour
{
    [Range(-1,1)]
    [SerializeField] private float fovDotThreshold = 0.5f;

    private BrackenEnemyStates brackenEnemyStates;

    //private float brackenAngerLevel = 0;
    //private const float brackenAngerLevelMax = 5;

    //private float playerFocusTimer;
    //private const float playerFocusTimeOut = 5f;

    private void Awake()
    {
        brackenEnemyStates = GetComponent<BrackenEnemyStates>();
    }

    private void Update()
    {
        HandleMoveResponses();
    }

    private void HandleMoveResponses()
    {
        if (IsPlayerBehind(GetDotBetWeen(brackenEnemyStates.GetTarget())))
        {
            Debug.Log("Braken Behind Player");

            brackenEnemyStates.SetState(BrackenEnemyStates.BrackenStates.Follow);
        }
        else
        {
            Debug.Log("Braken Front of player");

            brackenEnemyStates.SetState(BrackenEnemyStates.BrackenStates.Hide);
            
        }
    }


    //private bool IsInsidePlayerLOS()
    //{

    //    Vector3 directionToPlayer = brackenEnemyStates.GetTarget().position - transform.position;
    //    float distanceToPlayer = directionToPlayer.magnitude; 

    //    if (Physics.Raycast(transform.position, directionToPlayer.normalized, out RaycastHit hit, distanceToPlayer))
    //    {
    //        // Check if the raycast hit the player or any obstacle between the hide point and the player
    //        if (hit.collider != null && hit.collider.CompareTag("Player"))
    //        {
    //            // The player is in line of sight
    //            Debug.DrawLine(transform.position, brackenEnemyStates.GetTarget().position, Color.green);
    //            return true;
    //        }
    //        else
    //        {
    //            // Something else was hit (obstacle)
    //            Debug.DrawLine(transform.position, hit.point, Color.red);
    //            return false;
    //        }
    //    }
    //    else
    //    {
    //        // No hit, meaning no obstacles between enemy and player
    //        Debug.DrawLine(transform.position, brackenEnemyStates.GetTarget().position, Color.green);
    //        return true;
    //    }
    //}

    public Transform GetNearestTarget()
    {
        FirstPersonController[] playerTransforms = FindObjectsOfType<FirstPersonController>();
        var nearestPlayer = playerTransforms.OrderBy(tr => Vector3.Distance(transform.position, tr.transform.position)).FirstOrDefault();
        return nearestPlayer.transform;
    }

    private float GetDotBetWeen(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        return Vector3.Dot(target.forward, dir.normalized);
    }
    private bool IsPlayerBehind(float dotValue)
    {
        return dotValue > fovDotThreshold;
    }
}
