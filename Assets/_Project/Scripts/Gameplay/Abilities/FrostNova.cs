using UnityEngine;

public class FrostNova : BaseAbility
{
    [Header("FrostNova")]
    public float novaRadius = 5f;
    public GameObject frostNovaVFX;

    [Header("Indicator")]
    public GameObject circleIndicator;

    public override void ShowIndicator()
    {
        base.ShowIndicator();
        
        if (circleIndicator != null)
        {
            circleIndicator.SetActive(true);
            circleIndicator.transform.localScale = new Vector3 (novaRadius * 2f, circleIndicator.transform.localScale.y, novaRadius * 2f);

        }
    }

    public override void HideIndicator()
    {
        base.HideIndicator();
        if (circleIndicator != null)
        {
            circleIndicator.SetActive(false);
        }
    }

    public override void Use()
    {
        if (IsOnCooldown())
            return;
        
        Collider[] hits = Physics.OverlapSphere(transform.position, novaRadius);

        foreach (Collider hit in hits)
        {
           Destructible destructible = hit.GetComponent<Destructible>();
              if (destructible != null)
              {
                destructible.TakeDamage(damage);
              }
        }

        if (frostNovaVFX != null)
        {
            GameObject vfx = Instantiate(frostNovaVFX, transform.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }

        HideIndicator();
        lastUsedTime = Time.time;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, novaRadius);
    }

    void Start()
    {
        circleIndicator.SetActive(false);
    }

    void Update()
    {
        circleIndicator.transform.position = new Vector3(transform.position.x, circleIndicator.transform.position.y, transform.position.z);
    }
}
