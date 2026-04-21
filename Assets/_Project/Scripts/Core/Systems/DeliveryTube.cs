using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeliveryTube3D : MonoBehaviour
{
    public enum TubeState
    {
        Idle,
        Selecting,
        Locked,
    }

    public TubeState currentState = TubeState.Idle;

    [Header("Referencias del Sistema")]
    public ClientSystem tradeManager;
    public Animator tubeAnimator;
    public Transform spawnPoint;

    [Header("Materiales y Visuales")]
    public Material hologramMaterial;
    public GameObject forbiddenSignPrefab;

    [Header("UI de Selección")]
    [SerializeField]
    private TextMeshProUGUI nameDisplayText;

    private int currentIndex = 0;
    private GameObject currentVisualPotion;
    private Material[] originalMaterials;

    public void OnMainTubeButtonClicked()
    {
        if (currentState == TubeState.Idle)
        {
            OpenTube();
        }
        else if (currentState == TubeState.Selecting)
        {
            List<ItemData> validPotions = GetValidPotions();

            if (validPotions.Count > 0)
            {
                LockAndSendPotion();
            }
            else
            {
                // Si intenta enviar y no hay nada, refrescamos el aviso de error
                UpdateVisuals();
            }
        }
    }

    private void OpenTube()
    {
        currentState = TubeState.Selecting;
        tubeAnimator.SetTrigger("LowerTube");

        List<ItemData> validPotions = GetValidPotions();
        currentIndex = 0;

        if (validPotions.Count > 0)
        {
            ItemData neededPotion = tradeManager.GetCurrentOrder();

            if (neededPotion != null)
            {
                for (int i = 0; i < validPotions.Count; i++)
                {
                    if (validPotions[i].id == neededPotion.id)
                    {
                        currentIndex = i;
                        break;
                    }
                }
            }
        }

        UpdateVisuals();
    }

    public void OnLeftArrowClicked()
    {
        if (currentState != TubeState.Selecting)
            return;

        List<ItemData> validPotions = GetValidPotions();

        // Si hay al menos una poción, permitimos que el sistema se refresque
        if (validPotions.Count > 0)
        {
            // Solo cambiamos el índice si hay más de una opción
            if (validPotions.Count > 1)
            {
                currentIndex--;
                if (currentIndex < 0)
                    currentIndex = validPotions.Count - 1;
            }
            else
            {
                // Si solo hay una, nos aseguramos de estar en el índice 0
                currentIndex = 0;
            }

            // ¡CLAVE!: Siempre llamamos a UpdateVisuals si hay pociones,
            // esto "limpiará" el mensaje de SIN POCIONES.
            UpdateVisuals();
        }
    }

    // ARREGLO: La flecha derecha ahora usa la lista filtrada igual que la izquierda
    public void OnRightArrowClicked()
    {
        if (currentState != TubeState.Selecting)
            return;

        List<ItemData> validPotions = GetValidPotions();

        if (validPotions.Count > 0)
        {
            if (validPotions.Count > 1)
            {
                currentIndex++;
                if (currentIndex >= validPotions.Count)
                    currentIndex = 0;
            }
            else
            {
                currentIndex = 0;
            }

            UpdateVisuals();
        }
    }

    // ARREGLO: UpdateVisuals limpio. Sin redundancias ni variables duplicadas.
    private void UpdateVisuals()
    {
        if (currentVisualPotion != null)
            Destroy(currentVisualPotion);

        List<ItemData> validPotions = GetValidPotions();

        if (validPotions.Count == 0)
        {
            nameDisplayText.text = "SIN POCIONES";
            nameDisplayText.color = Color.red;
            currentVisualPotion = null;
            return;
        }

        //nameDisplayText.color = Color.white;

        if (currentIndex >= validPotions.Count)
            currentIndex = 0;
        if (currentIndex < 0)
            currentIndex = validPotions.Count - 1;

        ItemData selectedItem = validPotions[currentIndex];
        nameDisplayText.text = selectedItem.itemName.ToUpper();

        currentVisualPotion = Instantiate(selectedItem.potionPrefab, spawnPoint);

        ApplyHologramEffect();
    }

    private void ApplyHologramEffect()
    {
        if (currentVisualPotion == null)
            return;

        MeshRenderer renderer = currentVisualPotion.GetComponentInChildren<MeshRenderer>();

        if (renderer != null)
        {
            originalMaterials = renderer.materials;
            Material[] holoMats = new Material[originalMaterials.Length];
            for (int i = 0; i < holoMats.Length; i++)
            {
                holoMats[i] = hologramMaterial;
            }
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
            renderer.materials = originalMaterials;
        }

        Invoke("ExecuteDelivery", 0.6f);
    }

    // ARREGLO CRÍTICO: La entrega ahora saca el ítem correcto de la lista filtrada,
    // evitando que le entregues a B.E.L. una raíz creyendo que es una poción.
    private void ExecuteDelivery()
    {
        List<ItemData> validPotions = GetValidPotions();

        // Seguro de vida por si ocurre algún error extraño
        if (validPotions.Count == 0 || currentIndex >= validPotions.Count)
            return;

        ItemData itemToSend = validPotions[currentIndex];

        tradeManager.ProcessDelivery(itemToSend);
        InventoryManager.Instance.RemoveItem(itemToSend, 1);

        tubeAnimator.SetTrigger("SendUp");
        Destroy(currentVisualPotion);
        currentState = TubeState.Idle;
    }

    private List<ItemData> GetValidPotions()
    {
        List<ItemData> validPotions = new List<ItemData>();
        var inv = InventoryManager.Instance.inventory;

        foreach (var slot in inv)
        {
            if (slot.item != null && slot.item.potionPrefab != null)
            {
                validPotions.Add(slot.item);
            }
        }
        return validPotions;
    }
}
