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
            ValidateForPlayerFOV(hidePoints[i], playerTransform);

            ValidateForPlayerLOS(hidePoints[i], playerTransform);
        }
    }

    private void ValidateForPlayerFOV(HidePoint hidePoint, Transform playerTransform)
    {
        Vector3 directionToCollider = hidePoint.transform.position - playerTransform.position;
        float dotProduct = Vector3.Dot(playerTransform.forward, directionToCollider.normalized);

        hidePoint.SetInsidePlayerFOV(dotProduct > fovDotThreshold);
    }

    private void ValidateForPlayerLOS(HidePoint hidePoint, Transform playerTransform)
    {
        //No need extra Calc Skip Thsi test ,For Performance 
        if (hidePoint.IsInsidePlayerFOV) return;

        Vector3 directionToPlayer = playerTransform.position - hidePoint.transform.position;
        float distanceToPlayer = directionToPlayer.magnitude; // Use magnitude instead of Vector3.Distance for performance

        // Perform a raycast from the hide point towards the player
        if (Physics.Raycast(hidePoint.transform.position, directionToPlayer.normalized, out RaycastHit hit, distanceToPlayer))
        {
            // Check if the raycast hit the player or any obstacle between the hide point and the player
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                // The player is in line of sight
                hidePoint.IsInsidePlayerLOS = true;
                Debug.DrawLine(hidePoint.transform.position, playerTransform.position, Color.red);
            }
            else
            {
                // Something else was hit (obstacle)
                hidePoint.IsInsidePlayerLOS = false;
                Debug.DrawLine(hidePoint.transform.position, hit.point, Color.blue);
            }
        }
        else
        {
            // No hit, meaning no obstacles between hide point and player
            hidePoint.IsInsidePlayerLOS = false;
            Debug.DrawLine(hidePoint.transform.position, playerTransform.position, Color.green);
        }
    }

    public HidePoint GetNearestNotAffectedHidePoint(Vector3 position)
    {
        HidePoint nearestHidePoint = null;
        float nearestDistance = float.MaxValue;

        foreach (HidePoint hidePoint in hidePoints)
        {
            if (hidePoint.HidePossibilityPercentage >= 90)
            {
                float distance = Vector3.Distance(hidePoint.transform.position, position);
                if (distance < nearestDistance)
                {
                    nearestHidePoint = hidePoint;
                    nearestDistance = distance;
                }
            }
        }

        if (nearestHidePoint != null)
        {
            return nearestHidePoint;
        }
        else
        {
            Debug.LogWarning("No Hide Point Found");
            return null;
        }
    }

}
