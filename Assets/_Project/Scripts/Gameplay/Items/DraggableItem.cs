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
    private static DraggableItem _itemAtDropPoint = null;
    private int _currentUses = 0;
    private int _requiredUses = 0;

    private MachineDeposit _deposit;
    private ProcessingMachineLogic _machine;
    private MeshRenderer _meshRenderer;
    private Rigidbody _rb;

    void Awake()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        _dropRotation = Quaternion.Euler(180f, 0f, 0f);
        _deposit = FindFirstObjectByType<MachineDeposit>();
        _machine = FindFirstObjectByType<ProcessingMachineLogic>();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && !_anyAnimating)
            TryClick();
    }

    private void TryClick()
    {
        Debug.Log($"TryClick llamado en {gameObject.name}");
    Debug.Log($"currentState: {_machine.currentState}");
    Debug.Log($"_anyAnimating: {_anyAnimating}");
    Debug.Log($"_itemAtDropPoint: {(_itemAtDropPoint == null ? "NULL" : _itemAtDropPoint.gameObject.name)}");
        
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
        else
        {
            Debug.Log("Raycast no golpeo nada");
        }
    }

    private IEnumerator GoToDropPoint()
    {
        _isAnimating = true;
        _anyAnimating = true;

        // Obtiene los usos requeridos de la receta
        if (_itemAtDropPoint != null && _itemAtDropPoint != this)
        {
            _isAnimating = false;
            _anyAnimating = false;
            yield break;
        }

        _requiredUses = GetRequiredUses();
        _currentUses = 0;

        // Desaparece del estante
        _meshRenderer.enabled = false;
        _rb.isKinematic = true;

        transform.position = dropPoint.position;
        transform.rotation = _dropRotation;

        yield return new WaitForFixedUpdate();
        
        _meshRenderer.enabled = true;
        _isAtDropPoint = true;
        _itemAtDropPoint = this;

        _isAnimating = false;
        _anyAnimating = false;
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

         // Deposita TODOS los usos de una vez
        for (int i = 0; i < _requiredUses; i++)
        _deposit?.TryDeposit(itemContenido);

        // Regresa al estante directamente
        _meshRenderer.enabled = false;
        transform.position = _startPosition;
        transform.rotation = _startRotation;
        
        yield return new WaitForFixedUpdate();

        yield return new WaitForSeconds(returnDuration);

        _rb.isKinematic = false;

        _meshRenderer.enabled = true;
        _isAtDropPoint = false;
        _itemAtDropPoint = null;
        _currentUses = 0;
        

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
                return ingredient.quantity * _machine._selectedQuantity;
        }

        return 1;
    }
}