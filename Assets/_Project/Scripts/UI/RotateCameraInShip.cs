using UnityEngine;

public class RotateCameraInShip : MonoBehaviour
{
    [SerializeField]
    GameObject btoLeft;

    [SerializeField]
    GameObject btoRigth;

    [SerializeField]
    GameObject menuPanel;

    public float stepDegrees = -90f;
    public float rotationSpeed = 5f;

    private Quaternion _targetRotation;
    private bool _isRotating = false;

    void Start()
    {
        //transform.localRotation = Quaternion.Euler(0, stepDegrees, 0f);
        _targetRotation = transform.localRotation;
        btoLeft.SetActive(false);
        btoRigth.SetActive(false);
    }

    void Update()
    {
        if (menuPanel != null && !menuPanel.activeSelf)
        {
            btoLeft.SetActive(true);
            btoRigth.SetActive(true);
        }

        // Mueve suavemente hacia la rotacion objetivo
        if (_isRotating)
        {
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                _targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // Cuando esta muy cerca del objetivo para de rotar
            if (Quaternion.Angle(transform.localRotation, _targetRotation) < 0.1f)
            {
                transform.localRotation = _targetRotation;
                _isRotating = false;
            }
        }
    }

    public void RotateLeft()
    {
        if (_isRotating)
            return; // evita rotar mientras esta rotando
        stepDegrees -= 90;
        _targetRotation = Quaternion.Euler(0, stepDegrees, 0f);
        _isRotating = true;
    }

    public void RotateRigth()
    {
        if (_isRotating)
            return; // evita rotar mientras esta rotando
        stepDegrees += 90;
        _targetRotation = Quaternion.Euler(0, stepDegrees, 0f);
        _isRotating = true;
    }
}
