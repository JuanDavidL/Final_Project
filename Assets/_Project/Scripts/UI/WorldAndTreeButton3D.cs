using UnityEngine;
using UnityEngine.InputSystem;

public class WorldAndTreeButton3D : MonoBehaviour
{
    public enum ButtonType { TravelToOtherWorld, SkillTree }

    [Header("Button Type")]
    public ButtonType buttonType;

    [Header ("Visual Feedback")]
    public MeshRenderer meshRenderer;
    public Material materialNormal;
    public Material materialHover;

    private InteractiveMenuManager _menuManager;
    private Camera _cam;

    void Awake()
    {
        _menuManager = FindFirstObjectByType<InteractiveMenuManager>();
        _cam = Camera.main;
        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    void Update ()
    {
        HandleHover();

        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryClick();
    }

    private void HandleHover()
    {
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
        {
          if (materialHover != null)
              meshRenderer.material = materialHover;
        }
        else
        {
            if (materialNormal != null)
                meshRenderer.material = materialNormal;
        }
    }

    private void TryClick()
    {
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
        {
            _menuManager.OnMainButtonPressed(buttonType);
        }
    }
}
