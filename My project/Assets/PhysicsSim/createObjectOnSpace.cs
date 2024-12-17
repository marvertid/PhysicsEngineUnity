using Unity.Mathematics;
using UnityEngine;

public class createObjectOnSpace : MonoBehaviour
{
    public GameObject particle;
    public Vector3 spawnPos = new Vector3(5, 0, 0); // Posizione dove creare l'oggetto

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            particle.transform.position = spawnPos;
            Instantiate(particle, spawnPos, Quaternion.identity);
            
        }
    }
}
