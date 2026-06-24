using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    // Kecepatan gerak horizontal player
    [SerializeField] private float moveSpeed = 5f;
    // Kecepatan terbang (gerak vertikal) player
    [SerializeField] private float flySpeed = 5f;
    // Batas posisi sumbu Y terendah (bawah)
    [SerializeField] private float minY = -3.4f;
    // Batas posisi sumbu Y tertinggi (atas)
    [SerializeField] private float maxY = 4.5f;

    // Referensi Rigidbody2D untuk kontrol fisik player
    private Rigidbody2D rb;

    private void Awake()
    {
        // Ambil komponen Rigidbody2D dari objek ini
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

        // Deteksi input keyboard (atas/bawah atau panah atas/bawah)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                verticalInput = 1f;
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                verticalInput = -1f;
        }

        // Batasi posisi player agar tidak keluar dari zona vertikal
        Vector2 position = rb.position;
        if ((position.y <= minY && verticalInput < 0f) || (position.y >= maxY && verticalInput > 0f))
        {
            verticalInput = 0f;
        }

        // Update posisi dan kecepatan player
        rb.position = new Vector2(position.x, Mathf.Clamp(position.y, minY, maxY));
        rb.linearVelocity = new Vector2(moveSpeed, verticalInput * flySpeed);
    }
}
