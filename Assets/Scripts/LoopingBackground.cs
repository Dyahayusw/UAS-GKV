using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    // Kecepatan perpindahan tekstur background (efek parallax looping)
    public float backgroundSpeed;
    // Renderer untuk mengakses material background
    public Renderer backgroundRenderer;

    void Update()
    {
        // Geser offset tekstur ke kanan untuk membuat efek background bergerak
        backgroundRenderer.material.mainTextureOffset += new Vector2(backgroundSpeed * Time.deltaTime, 0f);
    }
}
