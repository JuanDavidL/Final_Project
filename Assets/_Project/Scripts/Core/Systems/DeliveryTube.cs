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
        Debug.Log("Botón presionado");
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

    var inv = InventoryManager.Instance.inventory; 

    if (inv.Count == 0)
    {
        // Instanciamos la X
        currentVisualPotion = Instantiate(forbiddenSignPrefab, spawnPoint);
        Debug.Log("Inventario vacío: Aplicando holograma a la X");
    }
    else
    {
        // Instanciamos la poción
        currentIndex = Mathf.Clamp(currentIndex, 0, inv.Count - 1);
        ItemData selectedItem = inv[currentIndex].item;
        currentVisualPotion = Instantiate(selectedItem.potionPrefab, spawnPoint);
        Debug.Log("Mostrando holograma de: " + selectedItem.itemName);
    }

    // LLAMADA UNIFICADA: Ahora se aplica a lo que sea que esté en el spawnPoint
    ApplyHologramEffect(); 
    }
private void ApplyHologramEffect()
{
    if (currentVisualPotion == null) return;

    // Buscamos el renderizador en el modelo (puede estar en un hijo)
    MeshRenderer renderer = currentVisualPotion.GetComponentInChildren<MeshRenderer>();
    
    if (renderer != null)
    {
        // Guardamos los materiales originales para poder restaurarlos después
        originalMaterials = renderer.materials;

        // Creamos un array del mismo tamaño pero lleno con el material de holograma
        Material[] holoMats = new Material[originalMaterials.Length];
        for (int i = 0; i < holoMats.Length; i++)
        {
            holoMats[i] = hologramMaterial;
        }

        // Aplicamos los materiales de holograma
        renderer.materials = holoMats;
    }
    else
    {
        Debug.LogWarning("No se encontró MeshRenderer en el prefab de la poción.");
    }
}
    private void LockAndSendPotion()
    {
    currentState = TubeState.Locked;

    MeshRenderer renderer = currentVisualPotion.GetComponentInChildren<MeshRenderer>();
    if (renderer != null && originalMaterials != null)
    {
        // Devolvemos los materiales reales (vidrio, líquido, corcho)
        renderer.materials = originalMaterials; 
        Debug.Log("Poción solidificada: Lista para envío.");
    }

    // Esperamos un momento para que el jugador aprecie su creación sólida
    Invoke("ExecuteDelivery", 0.6f); 
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