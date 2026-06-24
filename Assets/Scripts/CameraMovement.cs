using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Kecepatan gerak kamera ke arah sumbu X (horizontal)
    public float moveSpeed;

    void Update()
    {
        // Gerakkan kamera ke kanan berdasarkan kecepatan dan deltaTime
        transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
    }
}
