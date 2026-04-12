using UnityEngine;
using UnityEngine.InputSystem;

public class MagicBook : MonoBehaviour
{
    [Header("Positions")]
    public Transform player;
    public float sideOffset = 2f;
    public float heightOffset = 1f;
    public float aboveOffset = 2.5f;

    [Header("Floating")]
    public float floatspeed = 2f;
    public float floatAmount = 0.2f;

    [Header("Smoothness")]
    public float followSpeed = 5f;

    private Vector3 targetPosition;
    private bool isCollecting = false;
    private float currentSide = 1f;
    private PlayerInput playerInput;
    private InputAction moveAction;


    void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }

    public void StartCollecting(bool state)
    {
        isCollecting = state;
    }

    // Update is called once per frame
    void Update()
    {
        float floatY = Mathf.Sin(Time.time * floatspeed) * floatAmount;

        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        if (moveInput.x > 0.1f)
        {
            currentSide = -1f;
        }
        else if (moveInput.x < -0.1f)
        {
            currentSide = 1f;
        }

        if (isCollecting)
        {
            targetPosition = new Vector3(player.position.x, player.position.y + aboveOffset + floatY, player.position.z);
        }
        else
        {
            targetPosition = new Vector3(player.position.x + currentSide * sideOffset, player.position.y + heightOffset + floatY, player.position.z + aboveOffset);
            
        }

         transform.position = Vector3.Lerp(transform.position,targetPosition,followSpeed * Time.deltaTime);
    }
}
