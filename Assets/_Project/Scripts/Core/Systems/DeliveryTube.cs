using UnityEngine;
using System.Collections.Generic;

public class DeliveryTube3D : MonoBehaviour
{
    public enum TubeState { Idle, Selecting, Locked }
    public TubeState currentState = TubeState.Idle;

    [Header("Referencias del Sistema")]
    public ClientSystem tradeManager;
    public Animator tubeAnimator;       
    public Transform spawnPoint;        

    [Header("Materiales y Visuales")]
    public Material hologramMaterial;   
    public GameObject forbiddenSignPrefab; 

    private int currentIndex = 0;
    private GameObject currentVisualPotion;
    private Material[] originalMaterials; 

    public void OnMainTubeButtonClicked()
    {
        if (currentState == TubeState.Idle)
        {
            OpenTube();
        }
        else if (currentState == TubeState.Selecting && InventoryManager.Instance.inventory.Count > 0)
        {
            LockAndSendPotion();
        }
    }

    private void OpenTube()
    {
        currentState = TubeState.Selecting;
        tubeAnimator.SetTrigger("LowerTube"); 
        UpdateVisuals();
    }

    public void OnLeftArrowClicked()
    {
        var inv = InventoryManager.Instance.inventory;
        if (currentState != TubeState.Selecting || inv.Count == 0) return;
        
        currentIndex--;
        if (currentIndex < 0) currentIndex = inv.Count - 1;
        UpdateVisuals();
    }

    public void OnRightArrowClicked()
    {
        var inv = InventoryManager.Instance.inventory;
        if (currentState != TubeState.Selecting || inv.Count == 0) return;
        
        currentIndex++;
        if (currentIndex >= inv.Count) currentIndex = 0;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (currentVisualPotion != null) Destroy(currentVisualPotion);

        // Acceso a tu InventoryManager real
        var inv = InventoryManager.Instance.inventory; 

        if (inv.Count == 0)
        {
            currentVisualPotion = Instantiate(forbiddenSignPrefab, spawnPoint);
            return;
        }

        currentIndex = Mathf.Clamp(currentIndex, 0, inv.Count - 1);
        ItemData selectedItem = inv[currentIndex].item; // Extraemos el ItemData del Slot
        
        currentVisualPotion = Instantiate(selectedItem.potionPrefab, spawnPoint);

        // Aplicar efecto Holograma
        MeshRenderer renderer = currentVisualPotion.GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
        {
            originalMaterials = renderer.materials;
            Material[] holoMats = new Material[originalMaterials.Length];
            for (int i = 0; i < holoMats.Length; i++) holoMats[i] = hologramMaterial;
            renderer.materials = holoMats;
        }
    }

    private void LockAndSendPotion()
    {
        currentState = TubeState.Locked;

        // Quitar efecto holograma
        MeshRenderer renderer = currentVisualPotion.GetComponentInChildren<MeshRenderer>();
        if (renderer != null && originalMaterials != null)
        {
            renderer.materials = originalMaterials; 
        }

        Invoke("ExecuteDelivery", 0.5f); 
    }

    private void ExecuteDelivery()
    {
        var inv = InventoryManager.Instance.inventory;
        ItemData itemToSend = inv[currentIndex].item;

        // 1. Enviamos al sistema de comercio
        tradeManager.ProcessDelivery(itemToSend);

        // 2. Restamos del inventario usando tu método RemoveItem
        InventoryManager.Instance.RemoveItem(itemToSend, 1);

        tubeAnimator.SetTrigger("SendUp");
        Destroy(currentVisualPotion);
        currentState = TubeState.Idle;
    }
}