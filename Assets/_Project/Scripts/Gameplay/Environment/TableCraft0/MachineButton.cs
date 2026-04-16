using UnityEngine;
using UnityEngine.InputSystem;

public class MachineButton : MonoBehaviour
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

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                    machine.HandleButtonPress(tipo);
            }
        }
    }

    public void SetLight(bool isOn)
    {
        if (isOn)
        {
            buttonRenderer.material.EnableKeyword("_EMISSION");
            buttonRenderer.material.color = activeColor;
        }
        else
        {
            buttonRenderer.material.DisableKeyword("_EMISSION");
            buttonRenderer.material.color = inactiveColor;
        }
    }
}