using UnityEngine;

public class RotateCameraInShip : MonoBehaviour
{
    [SerializeField] GameObject btoLeft;
    [SerializeField] GameObject btoRigth;
    [SerializeField] GameObject GoToPlanetButton;
    [SerializeField] GameObject menuPanel;

    public float stepDegrees = -90f;
    public float rotationSpeed = 5f;

    private Quaternion _targetRotation;
    private bool _isRotating = false;

    void Start()
    {
        transform.localRotation = Quaternion.Euler(0, stepDegrees, 0f);
        _targetRotation = transform.localRotation;
        btoLeft.SetActive(false);
        btoRigth.SetActive(false);
        GoToPlanetButton.SetActive(false);
    }

    void Update()
    {
        if (menuPanel != null && !menuPanel.activeSelf)
        {
            btoLeft.SetActive(true);
            btoRigth.SetActive(true);
            UpdateGoToPlanetButton();
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

    private void UpdateGoToPlanetButton()
    {
        float normalizedAngle = stepDegrees % 360f;
        if (normalizedAngle < 0) normalizedAngle += 360f;

        if (Mathf.Approximately(normalizedAngle, 270f))
            GoToPlanetButton.SetActive(true);
        else
            GoToPlanetButton.SetActive(false);
    }

    public void RotateLeft()
    {
        if (_isRotating) return; // evita rotar mientras esta rotando
        stepDegrees -= 90;
        _targetRotation = Quaternion.Euler(0, stepDegrees, 0f);
        _isRotating = true;
        UpdateGoToPlanetButton();
    }

    public void RotateRigth()
    {
        if (_isRotating) return; // evita rotar mientras esta rotando
        stepDegrees += 90;
        _targetRotation = Quaternion.Euler(0, stepDegrees, 0f);
        _isRotating = true;
        UpdateGoToPlanetButton();
    }
}