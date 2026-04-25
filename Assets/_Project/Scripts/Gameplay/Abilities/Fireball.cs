using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Fireball : BaseAbility
{
    [Header("FireBall")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float explosionRadius = 2f;

    [Header("Indicator")]
    public LineRenderer lineIndicator;

    private Camera mainCamera;
    private MagicBook magicBook;
    private Mouse mouse;

    void Start()
    {
        mainCamera = Camera.main;
        mouse = Mouse.current;
        magicBook = FindFirstObjectByType<MagicBook>();
    }

    void Update()
    {
        base.Update();
        if (isIndicatorActive)
            UpdateIndicator();
    }

    public override void ShowIndicator()
    {
        base.ShowIndicator();
        lineIndicator.enabled = true;
    }

    public override void HideIndicator()
    {
        base.HideIndicator();
        lineIndicator.enabled = false;
    }

    private void UpdateIndicator()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        Vector3 direction = (mouseWorldPosition - transform.position).normalized;
        direction.y = 0f;

        Vector3 startPos = new Vector3(transform.position.x, 0.05f, transform.position.z);
        Vector3 endPos = new Vector3(transform.position.x + direction.x * 20f, 0.05f, transform.position.z + direction.z * 20f);

        lineIndicator.SetPosition(0, startPos);
        lineIndicator.SetPosition(1, endPos);
    }

    public override void Use()
    {
        if (!TryConsumeCharge()) return;

        // Lanza 'quantity' bolas con 0.2s de pausa entre cada una
        StartCoroutine(LaunchSequence());
    }

    private IEnumerator LaunchSequence()
    {
        Debug.Log($"LaunchSequence iniciada. Quantity: {quantity}");

        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        Vector3 spawnPosition = magicBook != null ? magicBook.GetBookPosition() : transform.position;
        Vector3 direction = (mouseWorldPosition - spawnPosition).normalized;
        direction.y = 0f;

        for (int i = 0; i < quantity; i++)
        {
            Vector3 finalSpawn = spawnPosition + direction * 1f; // Spawn a 1 metro frente al libro
            finalSpawn = new Vector3(finalSpawn.x, transform.position.y, finalSpawn.z);

            GameObject projectile = Instantiate(projectilePrefab, finalSpawn, Quaternion.identity);
            FireballProjectile fp = projectile.GetComponent<FireballProjectile>();
            fp.Init(direction, projectileSpeed, explosionRadius, damage);

            // Si hay más de una bola, espera 0.2s antes de la siguiente
            if (i < quantity - 1)
                yield return new WaitForSeconds(0.2f);
        }

        HideIndicator();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
            return hit.point;
        return transform.position;
    }
}