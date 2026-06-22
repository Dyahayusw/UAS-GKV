using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float flySpeed = 5f;
    [SerializeField] private float minY = -3.4f;
    [SerializeField] private float maxY = 4.5f;

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

        Vector2 position = rb.position;
        if ((position.y <= minY && verticalInput < 0f) || (position.y >= maxY && verticalInput > 0f))
        {
            verticalInput = 0f;
        }

        rb.position = new Vector2(position.x, Mathf.Clamp(position.y, minY, maxY));
        rb.linearVelocity = new Vector2(moveSpeed, verticalInput * flySpeed);
    }
}
