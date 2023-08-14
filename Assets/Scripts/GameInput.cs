using UnityEngine;

public class GameInput : MonoBehaviour
{
    PlayerInputActions playerInputActions;
    [SerializeField] Player player;
    void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    void Update()
    {
        if (playerInputActions.Player.Jump.triggered)
        {
            Debug.Log("This update function runs, jump is being called");
            Jump();
        }
    }
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }

    void Jump()
    {
        player.Jump();
    }

    void OnEnable()
    {
        playerInputActions.Enable();
    }

    void OnDisable()
    {
        playerInputActions.Disable();
    }
}
