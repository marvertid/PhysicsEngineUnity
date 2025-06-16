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
        return new Vector2(Mathf.Sin(angleRad), -Mathf.Cos(angleRad)).normalized;
    }


    public Vector2 GetPointOnPlane()
    {
        return transform.position;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 start = transform.position;
        Vector3 normal = new Vector3(GetNormal().x, GetNormal().y, 0f);
        Gizmos.DrawLine(start, start + normal * 3f);
    }

    public float planeLength = 2000f;

    public bool IsPointOverPlane(Vector2 point)
    {
        Vector2 start = transform.position;
        Vector2 tangent = GetTangent();
        Vector2 toPoint = point - start;
        float proj = Vector2.Dot(toPoint, tangent);
        return proj >= 0 && proj <= planeLength;
    }   


}
