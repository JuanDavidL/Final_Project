using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessingMachineLogic : MonoBehaviour
{
    public enum MachineState { Cerrada, Recibiendo, Procesando, Lista }

    [Header("Estado Actual")]
    public MachineState currentState = MachineState.Cerrada;

    [Header("Configuración de Recetas")]
    public RecipeData selectedRecipe;
    [SerializeField] private ItemData potionBasura;
    [SerializeField] private GameObject potionBad;
    private ItemData _lastResult;

    [Header("Ingredientes en el Contenedor")]
    private List<InventoryManager.InventorySlot> currentIngredients = new List<InventoryManager.InventorySlot>();

    [Header("Referencias de Botones")]
    [SerializeField] private MachineButton btnRojo;
    [SerializeField] private MachineButton btnAmarillo;
    [SerializeField] private MachineButton btnVerde;

    [Header("Animaciones/Objetos")]
    [SerializeField] private GameObject contenedorFisico;
    [SerializeField] private Transform deliveryPoint;

    private float zAbierto = -0.9f;
    private float zCerrado = 0.11f;
    public int _selectedQuantity = 1;
    private MachineUI _machineUI;
    private bool _waitingForYellow = false;
    private int _successCount = 0;
    private int _failCount = 0;
    

    private void Awake()
    {
        _machineUI = FindFirstObjectByType<MachineUI>();
    }

    public void HandleButtonPress(MachineButton.ButtonType tipo)
    {
        switch (tipo)
        {
            case MachineButton.ButtonType.Rojo:
                TryOpenMachine();
                break;
            case MachineButton.ButtonType.Amarillo:
                TryCloseAndLock();
                break;
            case MachineButton.ButtonType.Verde:
                TryDeliverPotion();
                break;
        }
    }

    private void TryOpenMachine()
    {
        if (selectedRecipe == null)
        {
        Debug.LogWarning("Selecciona una receta primero!");
        return;
        }

        if (_waitingForYellow) return;

        // ← bloquea si está procesando o lista
        if (currentState == MachineState.Procesando || currentState == MachineState.Lista) return;

        if (currentState == MachineState.Cerrada)
        {
        currentState = MachineState.Recibiendo;
        btnRojo.SetLight(true);
        btnAmarillo.SetLight(false);
        btnVerde.SetLight(false);
        StopAllCoroutines();
        StartCoroutine(MoverContenedor(zAbierto));
        _machineUI?.OnRedButtonPressed();
        Debug.Log("Máquina Abierta: Esperando materiales.");
        }
        else if (currentState == MachineState.Recibiendo)
        {
        currentState = MachineState.Cerrada;
        btnRojo.SetLight(false);
        currentIngredients.Clear();
        StopAllCoroutines();
        StartCoroutine(MoverContenedor(zCerrado));
        Debug.Log("Máquina Cancelada.");
        }

        
}

    private void TryCloseAndLock()
    {
        if (currentState == MachineState.Recibiendo && currentIngredients.Count > 0)
        {
            if (ValidarIngredientes())
            {
                currentState = MachineState.Procesando;
                btnRojo.SetLight(false);
                btnAmarillo.SetLight(true);
                StopAllCoroutines();
                StartCoroutine(MoverContenedor(zCerrado));
                _machineUI?.OnYellowButtonPressed();
                Debug.Log("<color=cyan>Materiales correctos. ¡Usa la palanca!</color>");
            }
            else
            {
                Debug.LogError("Receta incorrecta. Revisa los ingredientes.");
            }
        }
        else if (currentIngredients.Count == 0)
        {
            Debug.LogWarning("¡No puedes procesar un contenedor vacío!");
        }
    }

    private void TryDeliverPotion()
    {
        if (currentState == MachineState.Lista && _lastResult != null)
        {
            bool success = _lastResult != potionBasura;

            GameObject prefabAFabricar = (_lastResult == potionBasura)
                ? potionBad
                : selectedRecipe.potionPrefab;

            PoolableItem pocionVisual = PotionPool.Instance.Get(prefabAFabricar);
            pocionVisual.Initialize(_lastResult, Color.white);
            pocionVisual.transform.position = deliveryPoint.position;

            StartCoroutine(ProcesoEnvioInventario(pocionVisual, prefabAFabricar));

            _machineUI?.OnProcessComplete(success, _successCount, _failCount);
            btnVerde.SetLight(false);
            currentState = MachineState.Cerrada;
            _waitingForYellow = false;
            selectedRecipe = null;

        }
    }

    private IEnumerator ProcesoEnvioInventario(PoolableItem itemVisual, GameObject prefabUsado)
    {
        float timer = 0;
        Vector3 startPos = itemVisual.transform.position;

        while (timer < 2.0f)
        {
            itemVisual.transform.position = startPos + new Vector3(0, Mathf.Sin(Time.time * 5f) * 0.1f, 0);
            timer += Time.deltaTime;
            yield return null;
        }

        InventoryManager.Instance.AddItem(_lastResult, _selectedQuantity);
        if (_failCount > 0)
           InventoryManager.Instance.AddItem(potionBasura, _failCount);

        PotionPool.Instance.Release(prefabUsado, itemVisual);
    }

    public void SeleccionarRecetaManual(RecipeData receta, int quantity = 1)
    {
        selectedRecipe = receta;
        _selectedQuantity = quantity;
        currentIngredients.Clear();
        GetComponentInChildren<MachineLever>()?.ResetLever();
        Debug.Log($"Receta '{receta.recipeName}' seleccionada. Cantidad: {quantity  }");
    }

    public void AddIngredient(ItemData data)
    {
        if (currentState != MachineState.Recibiendo) return;

        // Verifica si este ingrediente pertenece a la receta
        bool isCorrect = selectedRecipe.requiredIngredients
        .Exists(i => i.item == data);

        if (!isCorrect)
        {
        // Se descarta → ya fue descontado del inventario en TryDeposit
        _machineUI?.OnWrongIngredient();
        Debug.LogWarning($"{data.itemName} no pertenece a esta receta. Descartado.");
        return;
        }

        var existing = currentIngredients.Find(s => s.item == data);
        if (existing != null) existing.quantity++;
        else currentIngredients.Add(new InventoryManager.InventorySlot { item = data, quantity = 1 });

        _machineUI?.OnIngredientDeposited(data);
        _waitingForYellow = true;
        Debug.Log($"Agregado: {data.itemName}");
    }

    private bool ValidarIngredientes()
    {
        if (selectedRecipe == null) return false;
        if (currentIngredients.Count != selectedRecipe.requiredIngredients.Count) return false;

        foreach (var req in selectedRecipe.requiredIngredients)
        {
            var encontrado = currentIngredients.Find(i => i.item == req.item);
            if (encontrado == null || encontrado.quantity != req.quantity * _selectedQuantity)
            {
                Debug.LogWarning($"Falta: {req.item.itemName}");
                return false;
            }
        }
        return true;
    }

    public void ProcessFinalPotion()
    {
           if (selectedRecipe == null) return;

        int successCount = 0;

        for (int i = 0; i < _selectedQuantity; i++)
        {
            float roll = Random.Range(0f, 100f);
             if (roll <= selectedRecipe.successChance)
            successCount++;
        }

        _lastResult = selectedRecipe.resultPotion;
        _successCount = successCount;
        _failCount = _selectedQuantity - successCount;

        Debug.Log($"Éxito: {successCount}/{_selectedQuantity}");
        FinalizeProcess();
    }

    private void FinalizeProcess()
    {
        currentIngredients.Clear();
        currentState = MachineState.Lista;
        btnAmarillo.SetLight(false);
        btnVerde.SetLight(true);
    }

    public void UpdateLeverProgress(int count)
    {
        if (selectedRecipe == null || currentState != MachineState.Procesando) return;

        _machineUI?.UpdateLeverCounter(count);

        if (count >= selectedRecipe.requiredLeverPresses)
        {
            ProcessFinalPotion();
            _machineUI?.OnLeverComplete();
            GetComponentInChildren<MachineLever>()?.ResetLever();
        }
    }

    private IEnumerator MoverContenedor(float targetZ)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startPos = contenedorFisico.transform.localPosition;
        Vector3 endPos = new Vector3(startPos.x, startPos.y, targetZ);

        while (elapsed < duration)
        {
            contenedorFisico.transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        contenedorFisico.transform.localPosition = endPos;
    }

    public void CancelProcess()
    {
        currentIngredients.Clear();
        selectedRecipe = null;
        _selectedQuantity = 1;
        currentState = MachineState.Cerrada;
        btnRojo.SetLight(false);
        btnAmarillo.SetLight(false);
        btnVerde.SetLight(false);
        StopAllCoroutines();
        StartCoroutine(MoverContenedor(zCerrado));
    }

    
}