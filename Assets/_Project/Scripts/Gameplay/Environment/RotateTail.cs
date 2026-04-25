using UnityEngine;

public class RotateTail : MonoBehaviour
{
    public float rotationSpeed = 30f; // Velocidad de rotación en grados por segundo
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
