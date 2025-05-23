using UnityEngine;

public class InclinedPlane : MonoBehaviour
{
    public Vector2 GetTangent()
    {
        float angleRad = transform.eulerAngles.z * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;
    }

    public Vector2 GetNormal()
    {
        float angleRad = transform.eulerAngles.z * Mathf.Deg2Rad;
        return new Vector2(-Mathf.Sin(angleRad), Mathf.Cos(angleRad)).normalized;
    }

    public Vector2 GetPointOnPlane()
    {
        return transform.position;
    }

}
