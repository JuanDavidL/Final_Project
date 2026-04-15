using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MachineLever : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ProcessingMachineLogic machine;
    
    [Header("Configuración de Animación")]
    [SerializeField] private Vector3 rotationOffset = new Vector3(0, 90, -90); // Cuánto baja
    [SerializeField] private float animationDuration = 0.2f;

    private int _currentPresses = 0;
    private bool _isAnimating = false;
    private Quaternion _initialRotation;

    void Awake()
    {
        _initialRotation = transform.localRotation;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Solo funciona si la máquina está en estado "Procesando" y no está animando
        if (machine.currentState != ProcessingMachineLogic.MachineState.Procesando || _isAnimating)
            return;

        _currentPresses++;
        Debug.Log($"Palancazo número: {_currentPresses}");

        // Ejecutamos la animación (puedes comentar una u otra para probar)
        // PlayAnimationLeanTween(); 
        StartCoroutine(PlayAnimationCoroutine());

        // Avisamos a la máquina del progreso
        machine.UpdateLeverProgress(_currentPresses);
    }

    // --- MÉTODO 1: LEANTWEEN ---
    // private void PlayAnimationLeanTween()
    // {
    //     _isAnimating = true;
    //     // Baja
    //     LeanTween.rotateAroundLocal(gameObject, Vector3.right, rotationOffset.x, animationDuration)
    //         .setEaseInBack()
    //         .setOnComplete(() => {
    //             // Sube automáticamente al terminar de bajar
    //             LeanTween.rotateAroundLocal(gameObject, Vector3.right, -rotationOffset.x, animationDuration)
    //                 .setEaseOutBack()
    //                 .setOnComplete(() => _isAnimating = false);
    //         });
    // }

    // --- MÉTODO 2: CORRUTINA ---
    private IEnumerator PlayAnimationCoroutine()
    {
        _isAnimating = true;
        
        float elapsed = 0;
        Quaternion targetRotation = _initialRotation * Quaternion.Euler(rotationOffset);

        // Bajar
        while (elapsed < animationDuration)
        {
            transform.localRotation = Quaternion.Slerp(_initialRotation, targetRotation, elapsed / animationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Subir
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