using UnityEngine;

public class RotateCameraInShip : MonoBehaviour
{
    [SerializeField] GameObject btoLeft;
    [SerializeField] GameObject btoRigth;

    public float stepDegrees = -90f;

    void Start()
    {
        transform.localRotation = Quaternion.Euler(0, stepDegrees, 0f);
        btoLeft.SetActive(false);
        btoRigth.SetActive(true);

    }

    public void RotateLeft()
    {
        stepDegrees -= 90;
        btoLeft.SetActive(true);
        btoRigth.SetActive(true);

        transform.localRotation = Quaternion.Euler(0, stepDegrees, 0f);

        if (stepDegrees == -90)
        {
            btoLeft.SetActive(false);
            btoRigth.SetActive(true);
            Debug.Log($"<color=green>Limite:</color> Limite alcanzado - izquierda.");
        }

    }

    public void RotateRigth()
    {
        stepDegrees += 90;
        btoLeft.SetActive(true);
        btoRigth.SetActive(true);

        transform.localRotation = Quaternion.Euler(0, stepDegrees, 0f);

        if (stepDegrees == 90)
        {
            btoLeft.SetActive(true);
            btoRigth.SetActive(false);
            Debug.Log($"<color=red>Limite:</color> Limite alcanzado - derecha.");
        }

    }


}