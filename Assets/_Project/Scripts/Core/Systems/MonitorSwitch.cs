using UnityEngine;
using UnityEngine.EventSystems;

public class MonitorSwitch : MonoBehaviour, IPointerClickHandler
{
    [Header("Configuración Física")]
    [SerializeField] private Transform switchPivot;
    [SerializeField] private float onRotationY = -36.362f; // Ahora en Y
    [SerializeField] private float offRotationY = -3.362f;

    [Header("Visuales y Hardware")]
    [SerializeField] private GameObject monitorCanvas;
    [SerializeField] private MeshRenderer monitorRenderer;
    [SerializeField] private Material screenMaterial;

    public bool IsOn { get; private set; } = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        IsOn = !IsOn;
        ToggleSwitch();
    }

    private void ToggleSwitch()
    {
        // 1. ROTACIÓN EN Y LOCAL
        float targetY = IsOn ? onRotationY : offRotationY;
        // Cambiamos el parámetro a (0, targetY, 0)
        switchPivot.localRotation = Quaternion.Euler(0, targetY, 0);

        if (monitorRenderer != null)
        {
            // ESTA ES LA CLAVE: .material (en minúscula) accede a la copia viva
            Material instanceMaterial = monitorRenderer.material;

            if (IsOn)
            {
                instanceMaterial.EnableKeyword("_EMISSION");
                // Usamos un multiplicador alto para asegurar que se vea
                instanceMaterial.SetColor("_EmissionColor", Color.white * 1.5f);
                Debug.Log("Material de instancia: ON");
            }
            else
            {
                instanceMaterial.SetColor("_EmissionColor", Color.black);
                instanceMaterial.DisableKeyword("_EMISSION");
                Debug.Log("Material de instancia: OFF");
            }
        }

        if (monitorCanvas != null) monitorCanvas.SetActive(IsOn);
    }
    }

