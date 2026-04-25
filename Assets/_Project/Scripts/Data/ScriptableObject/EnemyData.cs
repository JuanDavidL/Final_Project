using UnityEngine;

[CreateAssetMenu(fileName = "E_Data", menuName = "SpawnEnemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Estadísticas Base")]
    public string enemyName;
    public float health;
    public float speed;
    public float damage;

}