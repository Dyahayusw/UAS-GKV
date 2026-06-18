using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Obstacle : MonoBehaviour
{

    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Border")
        {
            // Menghancurkan objek ini saat bertabrakan dengan pemain
            Destroy(this.gameObject);
        }else if (collision.tag == "Player")
        {
            // Menghancurkan objek ini saat bertabrakan dengan pemain
            Destroy(player.gameObject);
        }
    }
}
