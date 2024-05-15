using System;
using UnityEngine;

public class HidePoint : MonoBehaviour
{
    public bool IsInsidePlayerFOV = false;
    public bool IsInsidePlayerLOS = false;
    public bool IsInsidePlayerDistance = false;

    [Range(0f, 100f)]
    public float HidePossibilityPercentage;
    private const float hidePossibilityPercentageMax = 100;

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
                if(!IsInsidePlayerDistance)
                {
                    HidePossibilityPercentage = hidePossibilityPercentageMax;
                }
                else
                {
                    HidePossibilityPercentage = 10;
                }
                
            }
            else
            {
                HidePossibilityPercentage = 30;
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
