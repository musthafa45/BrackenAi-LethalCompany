using UnityEngine;

public class HidePoint : MonoBehaviour
{
    public bool IsInsidePlayerFov = false;

    public void SetInsidePlayerFov(bool isInsidePlayerFov)
    {
        this.IsInsidePlayerFov = isInsidePlayerFov;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsInsidePlayerFov ? Color.red : Color.green;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
