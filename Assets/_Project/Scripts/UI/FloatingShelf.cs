using UnityEngine;
using System.Collections.Generic;

public class FloatingShelf : MonoBehaviour
{
    [Header("Flotacion")]
    public float floatSpeed = 1f;
    public float floatAmount = 0.2f;

    [Header("Rotacion")]
    public float rotationSpeed = 5f;
    private Quaternion _targetRotation;
    private bool _isRotating = false;
    private float _currentAngle = 0f;

    [Header("Slots")]
    public PotionSlot[] potionSlots;

    void Start()
    {
        _targetRotation = transform.localRotation;

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryUpdated += RefreshShelf;
            RefreshShelf();
        }
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryUpdated -= RefreshShelf;
    }

    void Update()
    {
        // Flotacion
        float floatY = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + floatY * Time.deltaTime,
            transform.position.z
        );

        // Rotacion suave
        if (_isRotating)
        {
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                _targetRotation,
                rotationSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(transform.localRotation, _targetRotation) < 0.1f)
            {
                transform.localRotation = _targetRotation;
                _isRotating = false;
            }
        }
    }

    public void RotateLeft()
    {
        if (_isRotating) return;
        _currentAngle -= 90f;
        _targetRotation = Quaternion.Euler(0f, 0f, _currentAngle);
        _isRotating = true;
    }

    public void RotateRight()
    {
        if (_isRotating) return;
        _currentAngle += 90f;
        _targetRotation = Quaternion.Euler(0f, 0f, _currentAngle);
        _isRotating = true;
    }

    private void RefreshShelf()
    {
        foreach (var slot in potionSlots)
            slot.UpdateSlot();
    }
}