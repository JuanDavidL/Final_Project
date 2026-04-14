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
        buttonRenderer.material.color = isOn ? activeColor : inactiveColor;
        // Aquí podrías añadir un sonido de "Click" o "Beep"
    }
}