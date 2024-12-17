using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class solver : MonoBehaviour
{
    Vector2 gravity = new Vector2(0.0f, -60.0f);
    float radius = 10;
    public static List<VerletObject> verletObjects = new List<VerletObject>();
    int subSteps = 8;
    void Update()
    {
        float subDt = Time.deltaTime / (float) subSteps;
        for (int i = 0; i < subSteps ; i++) {
            applyGravity();
            applyConstraint();
            solveCollision();
            updatePosition(subDt);
        }
    }

    void updatePosition(float dt) {
        foreach (VerletObject obj in verletObjects) {
            obj.updatePosition(dt);
        }
    }

    void applyGravity() {
        foreach (VerletObject obj in verletObjects)
        {
            obj.accelerate(gravity);
        }

    }

    void applyConstraint() {
        Vector2 position = new Vector2(0f, 0f); // Centro del vincolo

        foreach (VerletObject obj in verletObjects) {
            Vector2 toObj = obj.position_current - position; // Vettore dal centro all'oggetto
            float distSqr = toObj.sqrMagnitude; // Distanza al quadrato per ottimizzare

            if (distSqr > radius * radius) { // Se la distanza al quadrato è maggiore del raggio al quadrato
                float dist = Mathf.Sqrt(distSqr); // Calcola la distanza effettiva solo qui
                Vector2 n = toObj / dist; // Normalizza il vettore

                // Imposta la posizione dell'oggetto al bordo del vincolo
                obj.position_current = position + n * radius;
            }
        }
    }

    void solveCollision() {
        int objectCount = verletObjects.Count;
        float responseCoef = 0.75f;

        for (int i = 0; i < objectCount; i++) {
            VerletObject obj1 = verletObjects[i];
            for (int j = i + 1; j < objectCount; j++) {
                VerletObject obj2 = verletObjects[j];

                Vector2 collisionAxis = obj1.position_current - obj2.position_current;
                
                float dist = collisionAxis.magnitude;
                

                float minDist = obj1.getRadius() + obj2.getRadius();
                
                
                if (dist < minDist * minDist) {
                    Vector2 n = collisionAxis / dist;

                    float massRatio1 = obj1.getRadius() / (obj1.getRadius() + obj2.getRadius());
                    float massRatio2 = obj2.getRadius() / (obj1.getRadius() + obj2.getRadius());
                    float delta = 0.5f * responseCoef * (dist - minDist);

                    obj1.position_current -= n * (massRatio2 * delta);
                    obj2.position_current += n * (massRatio1 * delta); 
                }

            }
        }

    }
}
