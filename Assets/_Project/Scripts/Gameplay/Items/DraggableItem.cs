using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class DraggableItem : MonoBehaviour
{
    [Header("Datos")]
    public ItemData itemContenido;

    [Header("Configuracion")]
    public Transform dropPoint;
    public float shakeDuration = 0.3f;
    public float shakeIntensity = 15f;
    public float returnDuration = 0.3f;

    [Header("VFX")]
    public GameObject pourVFX;

    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Quaternion _dropRotation;
    private bool _isAtDropPoint = false;
    private bool _isAnimating = false;
    private static bool _anyAnimating = false;
    private int _currentUses = 0;
    private int _requiredUses = 0;

    private MachineDeposit _deposit;
    private ProcessingMachineLogic _machine;
    private MeshRenderer _meshRenderer;

    void Awake()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        _dropRotation = Quaternion.Euler(180f, 0f, 0f);
        _deposit = FindFirstObjectByType<MachineDeposit>();
        _machine = FindFirstObjectByType<ProcessingMachineLogic>();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && !_anyAnimating)
            TryClick();
    }

    private void TryClick()
    {
        if (_machine.currentState != ProcessingMachineLogic.MachineState.Recibiendo)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (!_isAtDropPoint)
                    StartCoroutine(GoToDropPoint());
                else
                    StartCoroutine(ShakeAndPour());
            }
        }
    }

    private IEnumerator GoToDropPoint()
    {
        _isAnimating = true;
        _anyAnimating = true;

        // Obtiene los usos requeridos de la receta
        _requiredUses = GetRequiredUses();
        _currentUses = 0;

        // Desaparece del estante
        _meshRenderer.enabled = false;

        // Aparece en el dropPoint rotado 180 en X
        transform.position = dropPoint.position;
        transform.rotation = _dropRotation;
        _meshRenderer.enabled = true;
        _isAtDropPoint = true;

        _isAnimating = false;
        _anyAnimating = false;

        yield return null;
    }

    private IEnumerator ShakeAndPour()
    {
        _isAnimating = true;
        _anyAnimating = true;

        // Shake tipo salero
        yield return StartCoroutine(ShakeAnimation());

        // VFX
        if (pourVFX != null)
        {
            GameObject vfx = Instantiate(pourVFX, dropPoint.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }

        // Deposita un uso
        _deposit?.TryDeposit(itemContenido);
        _currentUses++;

        // Si completó todos los usos regresa al estante
        if (_currentUses >= _requiredUses)
        {
            _meshRenderer.enabled = false;
            transform.position = _startPosition;
            transform.rotation = _startRotation;

            yield return new WaitForSeconds(returnDuration);

            _meshRenderer.enabled = true;
            _isAtDropPoint = false;
            _currentUses = 0;
        }

        _isAnimating = false;
        _anyAnimating = false;
    }

    private IEnumerator ShakeAnimation()
    {
        float elapsed = 0f;
        Quaternion baseRotation = _dropRotation;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shakeDuration;
            float shake = Mathf.Sin(t * Mathf.PI * 6f) * shakeIntensity * (1f - t);
            transform.rotation = baseRotation * Quaternion.Euler(shake, shake * 0.5f, 0f);
            yield return null;
        }

        transform.rotation = _dropRotation;
    }

    private int GetRequiredUses()
    {
        if (_machine.selectedRecipe == null) return 1;

        foreach (var ingredient in _machine.selectedRecipe.requiredIngredients)
        {
            if (ingredient.item == itemContenido)
                return ingredient.quantity;
        }

        return 1;
    }
}