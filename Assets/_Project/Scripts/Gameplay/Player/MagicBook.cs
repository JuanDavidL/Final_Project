using UnityEngine;
using UnityEngine.InputSystem;

public class MagicBook : MonoBehaviour
{
    [Header("Orbital Settings")]
    public Transform player;
    public float orbitalRadius = 2f;
    public float orbitalSpeed = 90f;

    [Header("Heights")]
    public float orbitHeight = 1f;
    public float collectHeight = 2.5f;
    public float abilityHeight = 2f;

    [Header("Smoothness")]
    public float followSpeed = 5f;

    private float currentAngle = 0f;
    private Vector3 targetPosition;

    public enum BookState { Orbiting, Collecting, Ability1, Ability2}
    public BookState currentState = BookState.Orbiting;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private Camera mainCamera;


    void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case BookState.Orbiting:
                UpdateOrbiting();
                break;
            case BookState.Collecting:
                UpdateAbovePlayer(collectHeight);
                break;
            case BookState.Ability1:
                UpdateOrbitTowardMouse();
                break;
            case BookState.Ability2:
                UpdateAbovePlayer(abilityHeight);
                break;
        }

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

    }

    private void UpdateOrbiting()
    {
        currentAngle += orbitalSpeed * Time.deltaTime;

        float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * orbitalRadius;
        float z = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * orbitalRadius;

        targetPosition = new Vector3(player.position.x + x, player.position.y + orbitHeight, player.position.z + z);
    }

    private void UpdateAbovePlayer(float height)
    {
        targetPosition = new Vector3(player.position.x, player.position.y + height, player.position.z);
    }

    private void UpdateOrbitTowardMouse()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mouseScreen);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 directionToMouse = (hit.point - player.position);
            directionToMouse.y = 0f;
            directionToMouse.Normalize();

            float targetAngle = Mathf.Atan2(directionToMouse.z, directionToMouse.x) * Mathf.Rad2Deg;
            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * followSpeed);
        }

        float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * orbitalRadius;
        float z = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * orbitalRadius;

        targetPosition = new Vector3(player.position.x + x, player.position.y, player.position.z + z);
    }

    public void SetState(BookState state)
    {
        currentState = state;
    }

    public Vector3 GetBookPosition()
    {
        return transform.position;
    }

}

