// LootTable.cs
using UnityEngine;

[System.Serializable]
public class DropEntry
{
    public ItemData item;
    public GameObject collectablePrefab; //Aca va el cubito azul, no el drop que contiene la imagen EVITAR CONFUSIONES ya que si se le pone el de la imagen no funciona.
    public int amount = 1;
    [Range(0f, 1f)] public float dropChance = 1f;
}

public class LootTable : MonoBehaviour
{
    public DropEntry[] drops;

    public void SpawnDrops()
    {
        foreach (DropEntry drop in drops)
        {
            if (drop.collectablePrefab == null) continue;

            float roll = Random.Range(0f, 1f);
            if (roll <= drop.dropChance)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-2f, 2f), 1.5f, Random.Range(-2f, 2f));

                GameObject dropObj = Instantiate(
                    drop.collectablePrefab,
                    transform.position + randomOffset,
                    Quaternion.identity);
                CollectibleItem collectible = dropObj.GetComponent<CollectibleItem>();
                collectible.item = drop.item;
                collectible.amount = drop.amount;
            }
        }
    }
}