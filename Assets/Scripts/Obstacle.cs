using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Obstacle : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Border")
        {
            // Menghancurkan objek ini saat bertabrakan dengan pemain
            Destroy(this.gameObject);
        }else if (collision.tag == "Player")
        {
            GameOver gameOver = collision.GetComponentInParent<GameOver>();

            if (gameOver != null)
            {
                gameOver.HandleObstacleHit();
            }
        }
    }
}
