using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnObstacles : MonoBehaviour
{
    // Prefab obstacle yang akan di-spawn
    public GameObject obstacle;
    // Batas posisi sumbu X maksimum untuk spawn
    public float maxX;
    // Batas posisi sumbu X minimum untuk spawn
    public float minX;
    // Batas posisi sumbu Y maksimum untuk spawn
    public float maxY;
    // Batas posisi sumbu Y minimum untuk spawn
    public float minY;
    // Waktu antar spawn obstacle
    public float timeBetweenSpawns;
    // Waktu spawn obstacle berikutnya
    private float spawnTime;

    void Update()
    {
        // Jika waktu sud exceeds waktu spawn, spawn obstacle baru
        if (Time.time > spawnTime)
        {
            Spawn();
            spawnTime = Time.time + timeBetweenSpawns;
        }
    }

    void Spawn()
    {
        // Randomkan posisi spawn dalam batas yang ditentukan
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
    
        // Buat instance obstacle pada posisi random di sekitar objek ini
        Instantiate(obstacle, transform.position + new Vector3(randomX, randomY, 0), transform.rotation);
    }
}
