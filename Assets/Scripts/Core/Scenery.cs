using UnityEngine;

public class Scenery : MonoBehaviour
{
    public float length = 520f;

    void Start()
    {
        Color grass = new Color(0.35f, 0.65f, 0.3f);

        // herbe de chaque côté de la piste (le milieu reste libre pour les trous)
        for (int s = -1; s <= 1; s += 2)
        {
            var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.name = "Herbe";
            g.transform.SetParent(transform);
            g.transform.position = new Vector3(s * 51.5f, -0.5f, length / 2f - 10f);
            g.transform.localScale = new Vector3(96f, 1f, length);
            g.GetComponent<Renderer>().material.color = grass;
            Destroy(g.GetComponent<BoxCollider>());
        }

        // arbres le long de la piste
        for (float z = -5f; z < length; z += 8f)
        {
            for (int s = -1; s <= 1; s += 2)
            {
                float x = s * Random.Range(6f, 22f);
                Tree(new Vector3(x, 0f, z + Random.Range(-3f, 3f)));
            }
        }

        // collines au loin
        for (int i = 0; i < 14; i++)
        {
            float side = (i % 2 == 0) ? -1f : 1f;
            var h = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            h.name = "Colline";
            h.transform.SetParent(transform);
            h.transform.position = new Vector3(side * Random.Range(70f, 130f), -6f, i * 45f);
            h.transform.localScale = new Vector3(Random.Range(50f, 80f), Random.Range(25f, 40f), Random.Range(50f, 80f));
            h.GetComponent<Renderer>().material.color = new Color(0.25f, 0.5f, 0.3f);
            Destroy(h.GetComponent<SphereCollider>());
        }
    }

    void Tree(Vector3 p)
    {
        float h = Random.Range(0.8f, 1.6f);

        var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Tronc";
        trunk.transform.SetParent(transform);
        trunk.transform.position = p + Vector3.up * h;
        trunk.transform.localScale = new Vector3(0.5f, h, 0.5f);
        trunk.GetComponent<Renderer>().material.color = new Color(0.45f, 0.3f, 0.18f);
        Destroy(trunk.GetComponent<Collider>());

        var leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaves.name = "Feuillage";
        leaves.transform.SetParent(transform);
        leaves.transform.position = p + Vector3.up * (2f * h + 1.2f);
        leaves.transform.localScale = Vector3.one * Random.Range(2.5f, 3.5f);
        leaves.GetComponent<Renderer>().material.color = new Color(0.15f, 0.5f, 0.2f);
        Destroy(leaves.GetComponent<Collider>());
    }
}