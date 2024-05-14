using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HidePointManager : MonoBehaviour
{
    public static HidePointManager Instance { get; private set; }

    [Range(-1f, 1f)]
    [SerializeField] private float fovDotThreshold = -0.5f;

    [SerializeField] private Transform playerTransform;

    private List<HidePoint> hidePoints = new List<HidePoint>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        hidePoints = FindObjectsOfType<HidePoint>().ToList();
    }

    private void Update()
    {
        if (hidePoints.Count == 0)
        {
            Debug.LogWarning("Hide Points Not Initialized");
            return;
        }

        for (int i = 0; i < hidePoints.Count; i++)
        {
            ValidateFieldOfView(hidePoints[i], playerTransform);
        }
    }

    private void ValidateFieldOfView(HidePoint hidePoint, Transform playerTransform)
    {
        Vector3 directionToCollider = hidePoint.transform.position - playerTransform.position;
        float dotProduct = Vector3.Dot(playerTransform.forward, directionToCollider.normalized);

        hidePoint.SetInsidePlayerFov(dotProduct > fovDotThreshold);
    }

    private bool IsInPlayerLOS(HidePoint hidePoint, Transform playerTransform)
    {
        Vector3 directionToPlayer = playerTransform.position - hidePoint.transform.position;
        float distanceToPlayer = directionToPlayer.magnitude; // Use magnitude instead of Vector3.Distance for performance

        RaycastHit hit;

        // Perform a raycast from the hide point towards the player
        if (Physics.Raycast(hidePoint.transform.position, directionToPlayer.normalized, out hit, distanceToPlayer))
        {
            // Check if the raycast hit the player or any obstacle between the hide point and the player
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                // The player is in line of sight
                return true;
            }
        }

        // Player is not in line of sight
        return false;
    }

    public HidePoint GetRandomNotAffectedHidePoint()
    {
        return hidePoints.Where(p => !p.IsInsidePlayerFov).FirstOrDefault();
    }

    public HidePoint GetNearestNotAffectedHidePoint(Vector3 position)
    {
        HidePoint nearHidePoint = hidePoints
        .Where(hidePoint => !hidePoint.IsInsidePlayerFov) // Filter out hide points that are inside player FOV
        .OrderBy(hidePoint => Vector3.Distance(hidePoint.transform.position, position)) // Order by distance to the given position
        .FirstOrDefault();

        if (nearHidePoint != null)
        {
            return nearHidePoint;
        }
        else
        {
            Debug.LogError("No Hide Point Found");
            return null;
        }
        
    }
}
