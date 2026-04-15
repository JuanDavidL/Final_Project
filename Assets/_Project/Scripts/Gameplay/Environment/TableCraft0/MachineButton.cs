using UnityEngine;
using UnityEngine.EventSystems;

public class MachineButton : MonoBehaviour, IPointerClickHandler
{
    public enum ButtonType { Rojo, Amarillo, Verde }
    public ButtonType tipo;

    [SerializeField] private ProcessingMachineLogic machine;
    [SerializeField] private MeshRenderer buttonRenderer;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;

    void Start()
    {
        SetLight(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Solo enviamos la orden a la máquina
        machine.HandleButtonPress(tipo);
    }

    public void SetLight(bool isOn)
    {
        // Cambia el color o la intensidad del material
        if (isOn)
        {
            buttonRenderer.material.EnableKeyword("_EMISSION"); // Si usas materiales con emisión
            buttonRenderer.material.color = activeColor;
        }
        else
        {
            buttonRenderer.material.DisableKeyword("_EMISSION");
            buttonRenderer.material.color = inactiveColor;
        }
    }
}