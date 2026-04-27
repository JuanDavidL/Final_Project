using UnityEngine;

public class BiomeSpawnManager : MonoBehaviour
{
    [Header("SpawnPoints en orden")]
    public Transform[] spawnPoints; // SpawnPoint Bioma1, SpawnPoint Bioma2

    [Header("Skyboxes en orden")]
    public Material[] skyboxMaterials; // Skybox Bioma1, Skybox Bioma2

    void Start()
    {
        int index = GameManager.Instance != null 
            ? GameManager.Instance.selectedPlanetIndex 
            : 0;

        //Teletransporta al jugador
        if (index < spawnPoints.Length && spawnPoints[index] != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Rigidbody rb = player.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.MovePosition(spawnPoints[index].position);
                }
                else
                    player.transform.position = spawnPoints[index].position;
            }
        }

        //Cambia el Skybox
        if (index < skyboxMaterials.Length && skyboxMaterials[index] != null)
            RenderSettings.skybox = skyboxMaterials[index];
    }
}