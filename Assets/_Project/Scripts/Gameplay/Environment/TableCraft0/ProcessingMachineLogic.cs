using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessingMachineLogic : MonoBehaviour
{
    public enum MachineState { Cerrada, Recibiendo, Procesando, Lista }

    [Header("Estado Actual")]
    public MachineState currentState = MachineState.Cerrada;

    [Header("Configuración de Recetas")]
    [SerializeField] private RecipeData selectedRecipe;
    [SerializeField] private ItemData potionBasura;
    [SerializeField] private GameObject potionBad;
    private ItemData _lastResult; // NUEVO: Para guardar la poción obtenida

    [Header("Ingredientes en el Contenedor")]
    private List<InventoryManager.InventorySlot> currentIngredients = new List<InventoryManager.InventorySlot>();

    [Header("Referencias de Botones")]
    [SerializeField] private MachineButton btnRojo;
    [SerializeField] private MachineButton btnAmarillo;
    [SerializeField] private MachineButton btnVerde;

    [Header("Animaciones/Objetos")]
    [SerializeField] private GameObject contenedorFisico;
    [SerializeField] private Transform deliveryPoint; // NUEVO: Punto donde aparece la poción

    // Distancias del contenedor (Ajusta estos valores según tu modelo)
    private float zAbierto = 1.05f;
    private float zCerrado = 2.5f;

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
        // Solo abrimos si está cerrada o acabamos de sacar una poción
        if (currentState == MachineState.Cerrada || currentState == MachineState.Lista)
        {
            currentState = MachineState.Recibiendo;
            btnRojo.SetLight(true);
            btnAmarillo.SetLight(false);
            btnVerde.SetLight(false);

            StopAllCoroutines(); // Evita conflictos de movimiento
            StartCoroutine(MoverContenedor(zAbierto)); // CORREGIDO: 1.5 es hacia afuera
            Debug.Log("Máquina Abierta: Esperando materiales.");
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
                Debug.Log("<color=cyan>Materiales correctos. ¡Usa la palanca!</color>");
            }
            else
            {
                Debug.LogError("Receta incorrecta. Revisa los ingredientes.");
                // Opcional: Podrías devolver los materiales o abrir de nuevo
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
            GameObject prefabAFabricar = (_lastResult == potionBasura)
                            ? potionBad
                            : selectedRecipe.potionPrefab;

            // 2. Le pedimos al Pool que nos de una instancia de ESE prefab específico
            PoolableItem pocionVisual = PotionPool.Instance.Get(prefabAFabricar);

            pocionVisual.transform.position = deliveryPoint.position;

            Debug.Log($"Poción {_lastResult.itemName} entregada.");

            StartCoroutine(ProcesoEnvioInventario(pocionVisual, prefabAFabricar));

            // Limpieza final
            btnVerde.SetLight(false);
            currentState = MachineState.Cerrada;
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

        // 5. Sumar al inventario real
        InventoryManager.Instance.AddItem(_lastResult, 1);

        // 6. Devolver al Pool (Desactivar)
        PotionPool.Instance.Release(prefabUsado, itemVisual);

        Debug.Log($"Poción {_lastResult} añadida al inventario y devuelta al Pool.");
        _lastResult = null;
    }

    public void SeleccionarRecetaManual(RecipeData receta)
    {
        selectedRecipe = receta;
        // Resetear el proceso para la nueva receta
        currentIngredients.Clear();
        GetComponentInChildren<MachineLever>()?.ResetLever();
        Debug.Log($"Pantalla: Receta '{receta.recipeName}' seleccionada.");
    }

    public void AddIngredient(ItemData data)
    {
        if (currentState != MachineState.Recibiendo) return;

        var existing = currentIngredients.Find(s => s.item == data);
        if (existing != null) existing.quantity++;
        else currentIngredients.Add(new InventoryManager.InventorySlot { item = data, quantity = 1 });

        Debug.Log($"Agregado al contenedor: {data.itemName}. Total tipos: {currentIngredients.Count}");
    }

    private bool ValidarIngredientes()
    {
        if (selectedRecipe == null) return false;

        // 1. Verificar si la cantidad de tipos de ingredientes coincide
        if (currentIngredients.Count != selectedRecipe.requiredIngredients.Count) return false;

        // 2. Comparar cada ingrediente de la receta
        foreach (var req in selectedRecipe.requiredIngredients)
        {
            // Buscamos si el ingrediente requerido está en el contenedor
            var encontrado = currentIngredients.Find(i => i.item == req.item);

            if (encontrado == null || encontrado.quantity != req.quantity)
            {
                Debug.LogWarning($"Falta o sobra cantidad de: {req.item.itemName}");
                return false;
            }
        }

        return true;
    }

    public void ProcessFinalPotion()
    {
        if (selectedRecipe == null) return;

        float randomRoll = Random.Range(0f, 100f);

        // Guardamos el resultado en la variable temporal
        if (randomRoll <= selectedRecipe.successChance)
        {
            _lastResult = selectedRecipe.resultPotion;
            Debug.Log("<color=green>¡Éxito en la alquimia!</color>");
        }
        else
        {
            _lastResult = potionBasura;
            Debug.Log("<color=red>La mezcla ha fallado.</color>");
        }

        FinalizeProcess();
    }

    private void FinalizeProcess()
    {
        currentIngredients.Clear();
        currentState = MachineState.Lista;

        btnAmarillo.SetLight(false);
        btnVerde.SetLight(true); // Se habilita la entrega
    }

    public void UpdateLeverProgress(int count)
    {
        if (selectedRecipe == null || currentState != MachineState.Procesando) return;

        if (count >= selectedRecipe.requiredLeverPresses)
        {
            ProcessFinalPotion();
            // Buscamos la palanca para resetear su contador interno
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
}