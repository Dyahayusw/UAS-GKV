using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Border")
        {
            // Menghancurkan objek ini saat bertabrakan dengan batas layar
            Destroy(this.gameObject);
        }
        else if (collision.tag == "Player")
        {
            // Ambil komponen GameOver dari player dan proses tabrakan
            GameOver gameOver = collision.GetComponentInParent<GameOver>();

            if (gameOver != null)
            {
                gameOver.HandleObstacleHit();
            }
        }
    }
}
