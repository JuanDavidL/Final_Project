using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs;           // Arrastra tus 4 prefabs aquí

    [Header("Spawn Points")]
    public Transform[] spawnPoints;             // Los puntos O del mapa

    [Header("Spawn Limits")]
    public int totalEnemiesLimit = 15;          // Máximo total que spawnearán en toda la partida
    public int maxAliveAtOnce = 5;              // Máximo vivos al mismo tiempo

    [Header("Spawn Timing")]
    public float spawnInterval = 3f;            // Segundos entre cada chequeo de spawn
    public float initialDelay = 1f;             // Espera antes del primer spawn

    // ---- Estado interno ----
    private int totalSpawned = 0;
    private List<GameObject> aliveEnemies = new List<GameObject>();

    void Start()
    {
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: No hay prefabs asignados.");
            return;
        }
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: No hay spawn points asignados.");
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(initialDelay);

        while (totalSpawned < totalEnemiesLimit)
        {
            // Limpiar referencias nulas (enemigos destruidos)
            aliveEnemies.RemoveAll(e => e == null);

            int currentAlive = aliveEnemies.Count;
            int canSpawn = Mathf.Min(
                maxAliveAtOnce - currentAlive,          // Cuántos caben hasta el límite vivo
                totalEnemiesLimit - totalSpawned        // Cuántos quedan por spawnear en total
            );

            for (int i = 0; i < canSpawn; i++)
            {
                SpawnOneEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("EnemySpawner: Límite total alcanzado. No spawneará más enemigos.");
    }

    void SpawnOneEnemy()
    {
        // Elegir punto de spawn aleatorio
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Elegir prefab aleatorio entre los 4
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        GameObject enemy = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        aliveEnemies.Add(enemy);
        totalSpawned++;

        Debug.Log($"Spawneado: {enemy.name} | Vivos: {aliveEnemies.Count} | Total spawneados: {totalSpawned}/{totalEnemiesLimit}");
    }

    // ---- Gizmos para ver los spawn points en el editor ----
    void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        Gizmos.color = Color.green;
        foreach (Transform sp in spawnPoints)
        {
            if (sp == null) continue;
            Gizmos.DrawSphere(sp.position, 0.4f);
            Gizmos.DrawLine(sp.position, sp.position + Vector3.up * 1.5f);
        }
    }
}