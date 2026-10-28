using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform player;
    public float laneDistance = 2f;
    public float spawnAhead = 60f;
    public float spacing = 12f;
    public float trackStart = -10f;
    public float trackEnd = 500f;
    public float finishDistance = 480f;
    public float holeLength = 4f;

    float nextZ = 25f;
    readonly List<float> holes = new List<float>();
    // noms de fichiers des prefabs dans Assets/Resources/Meals/ (sans accent)
    string[] healthyNames = { "Salade", "Riz", "Escalope" };
    string[] unhealthyNames = { "Pates", "Couscous" };

    void Start()
    {
        for (float z = 90f; z < finishDistance - 20f; z += 80f)
            holes.Add(z + Random.Range(-10f, 10f));

        BuildTrack();
        BuildFinishLine();

        foreach (var h in holes)
            SpawnMeal(h + holeLength / 2f, 1, true, 2f);
    }

    void Update()
    {
        while (nextZ < player.position.z + spawnAhead && nextZ <= finishDistance - 10f)
        {
            if (!NearHole(nextZ)) SpawnSlot(nextZ);
            nextZ += spacing;
        }
    }

    bool NearHole(float z)
    {
        foreach (var h in holes)
            if (z > h - 10f && z < h + holeLength + 4f) return true;
        return false;
    }

    void SpawnSlot(float z)
    {
        float r = Random.value;
        if (r < 0.25f)
        {
            int lane = Random.Range(0, 3);
            SpawnBarrier(z, lane);
            if (Random.value < 0.4f) SpawnBarrier(z, (lane + 1) % 3);
        }
        else if (r < 0.85f)
        {
            SpawnMeal(z, Random.Range(0, 3), Random.value < 0.6f, 1f);
        }
    }

    void BuildTrack()
    {
        float start = trackStart;
        foreach (var h in holes)
        {
            Segment(start, h);
            start = h + holeLength;
        }
        Segment(start, trackEnd);
    }

    void Segment(float from, float to)
    {
        var t = GameObject.CreatePrimitive(PrimitiveType.Cube);
        t.name = "Track";
        t.transform.position = new Vector3(0f, -0.5f, (from + to) / 2f);
        t.transform.localScale = new Vector3(7f, 1f, to - from);
        t.GetComponent<Renderer>().material.color = new Color(0.28f, 0.28f, 0.32f);
    }

    void BuildFinishLine()
    {
        var f = GameObject.CreatePrimitive(PrimitiveType.Cube);
        f.name = "Arrivee";
        f.transform.position = new Vector3(0f, 0.02f, finishDistance);
        f.transform.localScale = new Vector3(7f, 0.05f, 1f);
        f.GetComponent<Renderer>().material.color = Color.yellow;
        Destroy(f.GetComponent<BoxCollider>());
    }

    void SpawnBarrier(float z, int lane)
    {
        var b = GameObject.CreatePrimitive(PrimitiveType.Cube);
        b.name = "Barriere";
        b.transform.position = new Vector3((lane - 1) * laneDistance, 0.5f, z);
        b.transform.localScale = new Vector3(1.8f, 1f, 0.5f);
        b.GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0f);
        b.GetComponent<BoxCollider>().isTrigger = true;
        b.AddComponent<Obstacle>();
    }

    void SpawnMeal(float z, int lane, bool healthy, float y)
    {
        string key = healthy
            ? healthyNames[Random.Range(0, healthyNames.Length)]
            : unhealthyNames[Random.Range(0, unhealthyNames.Length)];
        string label = key == "Pates" ? "Pâtes" : key;

        var pos = new Vector3((lane - 1) * laneDistance, y, z);
        var prefab = Resources.Load<GameObject>("Meals/" + key);
        GameObject go;

        if (prefab != null)
        {
            // vrai modèle 3D
            go = Instantiate(prefab, pos, Quaternion.identity);
            foreach (var c in go.GetComponentsInChildren<Collider>()) Destroy(c);
            var col = go.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 0.7f;
        }
        else
        {
            // à défaut : sphère verte (sain) ou rouge (malsain)
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = pos;
            go.transform.localScale = Vector3.one * 0.8f;
            go.GetComponent<Renderer>().material.color = healthy ? Color.green : Color.red;
            go.GetComponent<SphereCollider>().isTrigger = true;
        }

        go.name = label;
        go.AddComponent<Meal>().healthy = healthy;

        var textGO = new GameObject("Label");
        textGO.transform.SetParent(go.transform);
        textGO.transform.localPosition = new Vector3(0f, 1.2f, 0f);
        var tm = textGO.AddComponent<TextMesh>();
        tm.text = label;
        tm.fontSize = 60;
        tm.characterSize = 0.12f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.color = healthy ? Color.green : Color.red;
    }
}