using UnityEngine;
using UnityEngine.InputSystem;

public class ArrowButton3D : MonoBehaviour
{
    public enum ArrowDirection
    {
        Left,
        Right,
    }

    [Header("Direction")]
    public ArrowDirection Direction;

    private InteractiveMenuManager _menuManager;
    private Camera _cam;

    void Awake()
    {
        _menuManager = FindFirstObjectByType<InteractiveMenuManager>();
        _cam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryClick();
    }

    private void TryClick()
    {
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
        {
            if (Direction == ArrowDirection.Left)
                _menuManager.OnArrowLeft();
            else
                _menuManager.OnArrowRight();
        }
    }
}
