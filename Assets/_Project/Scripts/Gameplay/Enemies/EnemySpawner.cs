using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable] // Esto permite que aparezca en el Inspector
    public class EnemyConfig
    {
        public string name;
        public GameObject prefab;
        public int maxAmount = 10;
        public float spawnInterval = 5f;
        [HideInInspector] public float lastSpawnTime;
        [HideInInspector] public List<GameObject> activeEnemies = new List<GameObject>();
    }

    [Header("Configuración de Enemigos")]
    public List<EnemyConfig> enemyConfigs; // Aquí añades tus 5 o 6 tipos

    [Header("Área de Spawn")]
    public Vector3 spawnAreaSize = new Vector3(100, 0, 100);

    void Update()
    {
        foreach (var config in enemyConfigs)
        {
            // 1. Limpiar muertos de la lista específica
            config.activeEnemies.RemoveAll(e => e == null);

            // 2. Verificar condiciones: ¿Hay espacio? ¿Pasó el tiempo (cooldown)?
            if (config.activeEnemies.Count < config.maxAmount)
            {
                if (Time.time >= config.lastSpawnTime + config.spawnInterval)
                {
                    SpawnEnemy(config);
                    config.lastSpawnTime = Time.time;
                }
            }
        }
    }

    void SpawnEnemy(EnemyConfig config)
    {
        Vector3 randomPoint = GetRandomNavMeshPoint();
        
        if (randomPoint != Vector3.zero)
        {
            GameObject newEnemy = Instantiate(config.prefab, randomPoint, Quaternion.identity);
            config.activeEnemies.Add(newEnemy);
            Debug.Log($"Spawned {config.name}. Total: {config.activeEnemies.Count}/{config.maxAmount}");
        }
    }

    Vector3 GetRandomNavMeshPoint()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            10,
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        ) + transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 15f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return Vector3.zero;
    }

    // Para ver el área de spawn en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}