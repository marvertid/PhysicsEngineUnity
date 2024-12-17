using UnityEngine;
using UnityEngine.UIElements;

public class VerletObject : MonoBehaviour
{
   [SerializeField]
   public Vector2 position_current;
   
   [SerializeField]
   public Vector2 position_old;
   
   [SerializeField]
    public Vector2 acceleration;

    private float radius;

     void OnEnable() {
        if (!solver.verletObjects.Contains(this)) {
            solver.verletObjects.Add(this);
        }
        position_current = transform.position;
        position_old = position_current;
        radius = transform.localScale.x / 2;
    }

    void OnDisable() {
        solver.verletObjects.Remove(this);
    }

    public void updatePosition(float dt) {
        transform.position = position_current;

        Vector2 velocity = position_current - position_old;

        position_old = position_current;

        position_current = position_current + velocity + acceleration * dt * dt;

        acceleration = Vector2.zero;


    }

    public void accelerate(Vector2 acc) {
        acceleration += acc;
    }

    public float getRadius() {
        return radius;
    }
}
