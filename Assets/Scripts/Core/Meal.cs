using UnityEngine;

public class Meal : MonoBehaviour
{
    public bool healthy;

    void Update()
    {
        transform.Rotate(0f, 90f * Time.deltaTime, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        GameManager.Instance.Eat(healthy);
        Destroy(gameObject);
    }
}