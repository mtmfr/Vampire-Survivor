using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    public static InputManager Instance
    {
        get
        {
            if (instance == null)
                Debug.LogError("No InputManager");
            return instance;
        }
    }

    private InputSystem_Actions inputs;


    //position the player will go towards to
    private Vector2 movementDir;
    public Vector2 MovementDir { get { return movementDir; } }

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else instance = this;

        inputs = new();
    }

    private void OnEnable()
    {
        inputs.Enable();
        inputs.Player.Move.performed += Move;
        inputs.Player.Move.canceled += StopMoving;
    }

    private void OnDisable()
    {
        inputs.Disable();
        inputs.Player.Move.performed -= Move;
        inputs.Player.Move.canceled -= StopMoving;
    }

    /// <summary>
    /// set the value of movementDir to the position of the cursor on the screen
    /// </summary>
    private void Move(InputAction.CallbackContext context)
    {
        movementDir = context.ReadValue<Vector2>();
    }

    private void StopMoving(InputAction.CallbackContext context)
    {
        movementDir = Vector2.zero;
    }
}