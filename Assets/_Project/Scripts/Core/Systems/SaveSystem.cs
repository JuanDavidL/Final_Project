using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "inventory.json");

    public static void SaveInventory(InventorySO data)
    {
        // Convertimos el estado actual a una versión serializable (JSON)
        // Usamos 'true' para que el JSON sea legible por humanos (Pretty Print)
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        
        Debug.Log($"Juego guardado en: {SavePath}");
    }

    public static void LoadInventory(InventorySO data)
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            // Sobrescribe los valores del ScriptableObject con los del JSON
            JsonUtility.FromJsonOverwrite(json, data);
            Debug.Log("Inventario cargado.");
        }
    }
}