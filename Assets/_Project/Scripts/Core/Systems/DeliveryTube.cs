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
    public MonitorSwitch powerSwitch;
    public Animator tubeAnimator;
    public Transform spawnPoint;

    [Header("Materiales y Visuales")]
    public Material hologramMaterial;
    public GameObject forbiddenSignPrefab;

    [Header("UI de Selección")]
    [SerializeField]
    private TextMeshProUGUI nameDisplayText;

    [SerializeField]
    private Color normalTextColor = new Color(0.576f, 0.972f, 0.443f);

    private int currentIndex = 0;
    private GameObject currentVisualPotion;
    private Material[] originalMaterials;

    public void OnMainTubeButtonClicked()
    {
        // 1. El único "seguro" que dejamos es el del switch de energía
        if (powerSwitch == null || !powerSwitch.IsOn)
            return;

        // 2. Si el tubo está arriba (Idle), lo bajamos y TERMINAMOS el proceso de este clic
        if (currentState == TubeState.Idle)
        {
            OpenTube();
            return; // <-- Este 'return' es vital para que no se envíe solo
        }

        // 3. Si el tubo YA está abajo (Selecting), entonces procesamos el envío
        if (currentState == TubeState.Selecting)
        {
            List<ItemData> validPotions = GetValidPotions();

            if (validPotions.Count > 0)
            {
                LockAndSendPotion();
            }
            else
            {
                UpdateVisuals(); // Refresca por si ya hay pociones
            }
        }
    }

    private void OpenTube()
    {
        currentState = TubeState.Selecting;
        tubeAnimator.SetTrigger("LowerTube");

        List<ItemData> validPotions = GetValidPotions();
        currentIndex = 0;

        UpdateVisuals();

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
        if (powerSwitch == null || !powerSwitch.IsOn)
            return;
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
        if (powerSwitch == null || !powerSwitch.IsOn)
            return;
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

    private void UpdateVisuals()
    {
        if (currentVisualPotion != null)
            Destroy(currentVisualPotion);

        // Si el tubo no está en modo selección, pantalla limpia.
        if (currentState != TubeState.Selecting)
        {
            nameDisplayText.text = "";
            return;
        }
        // --- PRIORIDAD: ¿SIN POCIONES? ---
        List<ItemData> validPotions = GetValidPotions();

        if (validPotions.Count == 0)
        {
            nameDisplayText.text = "SIN POCIONES";
            nameDisplayText.color = Color.red;
            currentVisualPotion = null;
            return;
        }

        // --- ESTADO NORMAL: MOSTRAR POCIÓN DEL INVENTARIO ---
        nameDisplayText.color = normalTextColor;

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

    void Update()
    {
        if (powerSwitch != null && !powerSwitch.IsOn && currentState != TubeState.Idle)
        {
            CloseTubeForcefully();
        }
    }

    private void CloseTubeForcefully()
    {
        if (currentVisualPotion != null)
            Destroy(currentVisualPotion);
        nameDisplayText.text = ""; // Pantalla a negro
        tubeAnimator.SetTrigger("SendUp"); // O el trigger que lo suba
        currentState = TubeState.Idle;
    }

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

        nameDisplayText.text = "";
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
