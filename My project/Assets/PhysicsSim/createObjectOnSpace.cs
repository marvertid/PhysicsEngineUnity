using Unity.Mathematics;
using UnityEngine;
using TMPro;

public class createObjectOnSpace : MonoBehaviour
{
    public GameObject particle;
    public Vector3 spawnPos = new Vector3(5, 0, 0); // Posizione dove creare l'oggetto

    public TextMeshProUGUI CounterText;

    void Start() {
          CounterText = GameObject.Find("CounterText").GetComponent<TextMeshProUGUI>();
          CounterText.text = "";
    }
   void Update() {
    if (Input.GetKeyDown(KeyCode.Space)) {
        GameObject newParticle = Instantiate(particle, spawnPos, Quaternion.identity);

        // Massa random
        float mass = UnityEngine.Random.Range(0.5f, 3f);

        // Ridimensionamento in base alla massa (opzionale: sqrt per estetica più stabile)
        float scale = Mathf.Sqrt(mass);
        newParticle.transform.localScale = new Vector3(scale, scale, 1f);

        // Accedi al componente VerletObject
        VerletObject vObj = newParticle.GetComponent<VerletObject>();
        if (vObj != null) {
            vObj.setMass(mass);
        }

        // Colorazione in base alla massa (rossa = più massiccia, blu = più leggera)
        SpriteRenderer rend = newParticle.GetComponent<SpriteRenderer>();
        if (rend != null) {
            float t = Mathf.InverseLerp(0.5f, 3f, mass); // Normalizza tra 0 e 1
            Color color = Color.Lerp(Color.cyan, Color.red, t);
            rend.color = color;
        }

        CounterText.text = "Numero particelle: " + solver.verletObjects.Count;
    }
}

}
