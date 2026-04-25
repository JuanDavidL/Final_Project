using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MachineLever : MonoBehaviour
{
    [SerializeField] private ProcessingMachineLogic machine;

    [Header("Configuración de Animación")]
    [SerializeField] private Vector3 rotationOffset = new Vector3(0, 90, -90);
    [SerializeField] private float animationDuration = 0.2f;

    private int _currentPresses = 0;
    private bool _isAnimating = false;
    private Quaternion _initialRotation;

    void Awake()
    {
        _initialRotation = transform.localRotation;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && !_isAnimating)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    if (machine.currentState != ProcessingMachineLogic.MachineState.Procesando) return;

                    _currentPresses++;
                    StartCoroutine(PlayAnimationCoroutine());
                    machine.UpdateLeverProgress(_currentPresses);
                }
            }
        }
    }

    private IEnumerator PlayAnimationCoroutine()
    {
        _isAnimating = true;

        float elapsed = 0;
        Quaternion targetRotation = _initialRotation * Quaternion.Euler(rotationOffset);

        while (elapsed < animationDuration)
        {
            transform.localRotation = Quaternion.Slerp(_initialRotation, targetRotation, elapsed / animationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0;
        while (elapsed < animationDuration)
        {
            transform.localRotation = Quaternion.Slerp(targetRotation, _initialRotation, elapsed / animationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = _initialRotation;
        _isAnimating = false;
    }

    public void ResetLever() => _currentPresses = 0;
}