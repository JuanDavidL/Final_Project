using UnityEngine;

public class IslandFloating : MonoBehaviour
{
    public float speed = 1f; // Velocidad de oscilación
    public Vector3 amplitude = new Vector3(0f, 0.5f, 0f); // Amplitud de oscilación en cada eje
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            transform.position.x + Mathf.Sin(Time.time * speed) * amplitude.x,
            transform.position.y + Mathf.Sin(Time.time * speed) * amplitude.y,
            transform.position.z + Mathf.Sin(Time.time * speed) * amplitude.z
        );
    }
}
