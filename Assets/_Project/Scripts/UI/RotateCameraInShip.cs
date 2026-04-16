using UnityEngine;

public class RotateCameraInShip : MonoBehaviour
{
    [SerializeField] GameObject btoLeft;
    [SerializeField] GameObject btoRigth;
    [SerializeField] GameObject GoToPlanetButton;
    [SerializeField] GameObject menuPanel;

    public float stepDegrees = -90f;
    public bool _menuWasActive = true;

    void Start()
    {
        transform.localRotation = Quaternion.Euler(0, stepDegrees, 0f);
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
    
    }

    private void UpdateGoToPlanetButton()
    {
        // Normaliza el ángulo entre 0 y 360 para comparar correctamente
        float normalizedAngle = stepDegrees % 360f;
        if (normalizedAngle < 0) normalizedAngle += 360f;

        if (Mathf.Approximately(normalizedAngle, 270f))
        GoToPlanetButton.SetActive(true);
        else
        GoToPlanetButton.SetActive(false);
    }

    public void RotateLeft()
    {
        stepDegrees -= 90;
        transform.localRotation = Quaternion.Euler(0, stepDegrees, 0f);
        UpdateGoToPlanetButton();
    }

    public void RotateRigth()
    {
        stepDegrees += 90;
        transform.localRotation = Quaternion.Euler(0, stepDegrees, 0f);
        UpdateGoToPlanetButton();
    }
}