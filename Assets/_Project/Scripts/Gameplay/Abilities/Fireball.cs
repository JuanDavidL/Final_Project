using UnityEngine;
using UnityEngine.InputSystem;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        mouse = Mouse.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (isIndicatorActive)
        {
            UpdateIndicator();
        }
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
        direction.Normalize();

        Vector3 startPos = new Vector3(transform.position.x, 0.05f, transform.position.z);
        Vector3 endPos = new Vector3(transform.position.x + direction.x * 20f, 0.05f, transform.position.z + direction.z * 20f);

        lineIndicator.SetPosition(0, startPos);
        lineIndicator.SetPosition(1, endPos);
    }

    public override void Use()
    {
        if (IsOnCooldown())
            return;

        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        Vector3 spwanPosition = magicBook != null ? magicBook.GetBookPosition() : transform.position;
        Vector3 direction = (mouseWorldPosition - spwanPosition).normalized;
        direction.Normalize();

        Vector3 finalPosition = new Vector3(spwanPosition.x, transform.position.y, spwanPosition.z);
        GameObject projectile = Instantiate(projectilePrefab, finalPosition, Quaternion.identity);
        
        FireballProjectile fp = projectile.GetComponent<FireballProjectile>();
        fp.Init(direction, projectileSpeed, explosionRadius, damage);
    
        HideIndicator();
        lastUsedTime = Time.time;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreen = mouse.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mouseScreen);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }

        return transform.position;
    }
}
