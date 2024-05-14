using System;
using UnityEngine;

public class HidePoint : MonoBehaviour
{
    public bool IsInsidePlayerFOV = false;
    public bool IsInsidePlayerLOS = false;

    [Range(0f, 100f)]
    public float HidePossibilityPercentage;
    private const float hidePossibilityPercentageMax = 100;

    public void SetHidePossibilityPercentage(float percentage)
    {
        HidePossibilityPercentage = percentage;
    }

    private void Update()
    {
        if (IsInsidePlayerFOV)
        {
            HidePossibilityPercentage = 0;
        }
        else
        {
            if(!IsInsidePlayerLOS)
            {
                HidePossibilityPercentage = hidePossibilityPercentageMax;
            }
            else
            {
                HidePossibilityPercentage = 20;
            }
           
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.Lerp(Color.red,Color.green, HidePossibilityPercentage / hidePossibilityPercentageMax);
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    public void SetInsidePlayerFOV(bool insidePlayerFOV)
    {
        IsInsidePlayerFOV = insidePlayerFOV;
    }

    public void SetInsidePlayerLOS(bool insidePlayerLOS)
    {
        IsInsidePlayerLOS = insidePlayerLOS;
    }

}
