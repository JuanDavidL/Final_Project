using UnityEngine;

[System.Serializable]
public class DropEntry
{
    public ItemData item;
    public int amount = 1;
    [Range(0f, 1f)] public float dropChance = 1f;
}

public class LootTable : MonoBehaviour
{
    public GameObject CollectablePrefab;
    public DropEntry[] drops;

    public void SpawnDrops()
    {
        foreach (DropEntry drop in drops)
        {
          float roll = Random.Range(0f, 1f);
          if (roll <= drop.dropChance)
          {
            Vector3 radomOffset = new Vector3(Random.Range(-2f, 2f), 1.5f, Random.Range(-2f, 2f));
              GameObject dropObj = Instantiate(CollectablePrefab, transform.position + radomOffset, Quaternion.identity);
              CollectibleItem collectible = dropObj.GetComponent<CollectibleItem>();
              collectible.item = drop.item;
              collectible.amount = drop.amount;
          }
        }
    }
}
