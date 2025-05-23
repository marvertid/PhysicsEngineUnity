using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class solver : MonoBehaviour
{
    float radius = 10;
    public static List<VerletObject> verletObjects = new List<VerletObject>();
    int subSteps = 8;
    [Range(0f, 90f)]
    public float planeAngle = 30f; // Angolo del piano in gradi

    public float gravityMagnitude = 60f; // Modulo della gravità

    public InclinedPlane inclinedPlane;
    void Start()
    {
        inclinedPlane = GameObject.Find("InclinedPlane").GetComponent<InclinedPlane>();
    }


    void Update()
    {
        float subDt = Time.deltaTime / (float)subSteps;
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
        Vector2 gravity = new Vector2(0f, -gravityMagnitude); // Gravità verticale

        foreach (VerletObject obj in verletObjects)
        {
            obj.accelerate(gravity); // applicazione diretta (non forza)
        }
    }


    void applyConstraint()
    {
        // Vector2 position = new Vector2(0f, 0f); // Centro del vincolo

        // foreach (VerletObject obj in verletObjects) {
        //     Vector2 toObj = obj.position_current - position; // Vettore dal centro all'oggetto
        //     float distSqr = toObj.sqrMagnitude; // Distanza al quadrato per ottimizzare

        //     if (distSqr > radius * radius) { // Se la distanza al quadrato è maggiore del raggio al quadrato
        //         float dist = Mathf.Sqrt(distSqr); // Calcola la distanza effettiva solo qui
        //         Vector2 n = toObj / dist; // Normalizza il vettore

        //         // Imposta la posizione dell'oggetto al bordo del vincolo
        //         obj.position_current = position + n * radius;
        //     }
        // }
    }

    void solveCollision()
    {
        int objectCount = verletObjects.Count;
        float responseCoef = 0.75f;



        for (int i = 0; i < objectCount; i++)
        {
            VerletObject obj1 = verletObjects[i];
            float obj1Radius = obj1.getRadius();
            for (int j = i + 1; j < objectCount; j++)
            {
                VerletObject obj2 = verletObjects[j];
                float obj2Radius = obj2.getRadius();

                Vector2 collisionAxis = obj1.position_current - obj2.position_current;

                float distSqr = collisionAxis.sqrMagnitude;
                float minDist = obj1.getRadius() + obj2.getRadius();


                if (distSqr < minDist * minDist)
                {
                    float dist = Mathf.Sqrt(distSqr);
                    Vector2 n = collisionAxis / dist;

                    float massRatio1 = obj1.getMass() / (obj1.getMass() + obj2.getMass());
                    float massRatio2 = obj2.getMass() / (obj1.getMass() + obj2.getMass());
                    float delta = 0.5f * responseCoef * (dist - minDist);

                    obj1.position_current -= n * (massRatio2 * delta);
                    obj2.position_current += n * (massRatio1 * delta);
                }


            }

        }

        if (inclinedPlane != null)
        {
            Vector2 planeNormal = inclinedPlane.GetNormal();
            Vector2 pointOnPlane = inclinedPlane.GetPointOnPlane();

            foreach (VerletObject obj in verletObjects)
            {
                Vector2 toObj = obj.position_current - pointOnPlane;
                float distanceFromPlane = Vector2.Dot(toObj, planeNormal);
                float penetration = obj.getRadius() - distanceFromPlane;


                if (penetration < 0f)
                {
                    // Correggi la posizione spingendo fuori dalla superficie
                    obj.position_current -= planeNormal * penetration;

                    // Simula un rimbalzo ridotto
                    Vector2 velocity = obj.position_current - obj.position_old;
                    float vn = Vector2.Dot(velocity, planeNormal);
                    Vector2 vNormal = planeNormal * vn;
                    Vector2 vTangent = velocity - vNormal;

                    // Inverti la componente normale e applica attrito alla tangenziale
                    velocity = -vNormal * 0.5f + vTangent * 0.95f;

                    obj.position_old = obj.position_current - velocity;
                }
            }

        }
    }

}
