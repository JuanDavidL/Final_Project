using UnityEngine;

public class FrostNova : BaseAbility
{
    [Header("FrostNova")]
    public float novaRadius = 5f;
    public GameObject frostNovaVFX;

    [Header("Indicator")]
    public GameObject circleIndicator;

    void Start()
    {
        circleIndicator.SetActive(false);
    }

    void Update()
    {
        base.Update();
        circleIndicator.transform.position = new Vector3(
            transform.position.x,
            circleIndicator.transform.position.y,
            transform.position.z
        );
    }

    public override void ShowIndicator()
    {
        base.ShowIndicator();
        if (circleIndicator != null)
        {
            circleIndicator.SetActive(true);
            circleIndicator.transform.localScale = new Vector3(novaRadius * 2f, circleIndicator.transform.localScale.y, novaRadius * 2f);
        }
    }

    public override void HideIndicator()
    {
        base.HideIndicator();
        if (circleIndicator != null)
            circleIndicator.SetActive(false);
    }

    public override void Use()
    {
        // TryConsumeCharge maneja las 2 cargas independientes
        if (!TryConsumeCharge()) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, novaRadius);
        foreach (Collider hit in hits)
        {
            Destructible dest = hit.GetComponent<Destructible>();
            if (dest != null) dest.TakeDamage(damage);
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null) enemy.TakeDamage(damage);
        }

        if (frostNovaVFX != null)
        {
            GameObject vfx = Instantiate(frostNovaVFX, transform.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }

        HideIndicator();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, novaRadius);
    }
}