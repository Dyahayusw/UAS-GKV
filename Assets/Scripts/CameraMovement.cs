using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed;

    void Update()
    {
      transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
    }
}
