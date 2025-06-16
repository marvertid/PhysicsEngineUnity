using System;
using System.Collections.Generic;
using UnityEngine;

public class solver : MonoBehaviour
{
    float radius = 10;
    public static List<VerletObject> verletObjects = new List<VerletObject>();
    int subSteps = 8;

    [Range(0f, 90f)]
    public float planeAngle = 30f;
    public float gravityMagnitude = 60f;

    public InclinedPlane inclinedPlane;

    void Update()
    {
        float subDt = Time.deltaTime / Mathf.Max(1f, subSteps);
        for (int i = 0; i < subSteps; i++)
        {
            applyGravity();
            applyConstraint();
            solveCollision();
            updatePosition(subDt);
        }
    }

    void updatePosition(float dt)
    {
        foreach (VerletObject obj in verletObjects)
        {
            obj.updatePosition(dt);
        }
    }

    void applyGravity()
    {
        Vector2 gravity = new Vector2(0f, -gravityMagnitude);
        foreach (VerletObject obj in verletObjects)
        {
            obj.accelerate(gravity);
        }
    }

    void applyConstraint()
    {
        // Constraint disabilitato (raggio contenimento)
    }

    void solveCollision()
    {
        int objectCount = verletObjects.Count;
        float responseCoef = 0.75f;

        // Collisions tra particelle
        for (int i = 0; i < objectCount; i++)
        {
            VerletObject obj1 = verletObjects[i];
            for (int j = i + 1; j < objectCount; j++)
            {
                VerletObject obj2 = verletObjects[j];

                Vector2 collisionAxis = obj1.position_current - obj2.position_current;
                float distSqr = collisionAxis.sqrMagnitude;
                float minDist = obj1.getRadius() + obj2.getRadius();

                if (distSqr < minDist * minDist)
                {
                    float dist = Mathf.Sqrt(distSqr);
                    Vector2 n = collisionAxis / dist;

                    float m1 = obj1.getMass();
                    float m2 = obj2.getMass();
                    float totalMass = m1 + m2;

                    float delta = 0.5f * responseCoef * (dist - minDist);

                    obj1.position_current -= n * (m2 / totalMass * delta);
                    obj2.position_current += n * (m1 / totalMass * delta);
                }
            }
        }

        // Collisione con piano inclinato
        if (inclinedPlane != null)
        {
            Vector2 planeNormal = inclinedPlane.GetNormal();
            Vector2 pointOnPlane = inclinedPlane.GetPointOnPlane();

            foreach (VerletObject obj in verletObjects)
            {

               // if (!inclinedPlane.IsPointOverPlane(obj.position_current)) continue;

                Vector2 toObj = obj.position_current - pointOnPlane;
                float distanceFromPlane = Vector2.Dot(toObj, planeNormal);
                float penetration = obj.getRadius() - distanceFromPlane;

                Debug.DrawLine(obj.position_current, obj.position_current - planeNormal * 1f, Color.green);  // Normale dal centro
                Debug.DrawLine(obj.position_current, pointOnPlane, Color.yellow);  // Linea verso il piano

                if (float.IsNaN(distanceFromPlane) || float.IsNaN(penetration))
                {
                    Debug.LogError("Valori non validi! distanceFromPlane: " + distanceFromPlane + ", penetration: " + penetration);
                    continue;
                }

                if (penetration > 0f)
                {
                    Vector2 surfacePoint = obj.position_current - planeNormal * obj.getRadius();
                    if (!inclinedPlane.IsPointOverPlane(surfacePoint)) continue;


                    // 👉 Correzione posizione: spingi la particella fuori dal piano
                    obj.position_current -= planeNormal * penetration;

                    Vector2 velocity = obj.position_current - obj.position_old;

                    if (float.IsNaN(velocity.x) || float.IsNaN(velocity.y))
                    {
                        Debug.LogError("NaN nella velocità! Oggetto: " + obj.name);
                        continue;
                    }

                    float vn = Vector2.Dot(velocity, planeNormal);
                    Vector2 vNormal = planeNormal * vn;
                    Vector2 vTangent = velocity - vNormal;

                    Vector2 newVelocity = -vNormal * 0.5f + vTangent * 0.95f;

                    if (float.IsNaN(newVelocity.x) || float.IsNaN(newVelocity.y))
                    {
                        Debug.LogError("NaN nella nuova velocità! Oggetto: " + obj.name);
                        continue;
                    }

                    obj.position_old = obj.position_current - newVelocity;
                }


            }
        }
    }
}
