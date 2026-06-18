using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float flySpeed = 5f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D belum ditemukan pada objek ini.");
        }
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        float verticalInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                verticalInput = 1f;
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                verticalInput = -1f;
        }

        rb.linearVelocity = new Vector2(moveSpeed, verticalInput * flySpeed);
    }
}
