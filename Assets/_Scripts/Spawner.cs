﻿using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject player;
    public GameObject zombie;
    public Collider m_Collider;
    private Vector3 m_Size;
    
    // spawn time
    public int spawntime = 5;
    private int countdown;

    // this will make zombie will not be spawn at player
    public int spawnFarRadius = 5;

    void Start()
    {
        countdown = spawntime * 60;

        //Fetch the size of the Collider volume
        m_Size = m_Collider.bounds.size * 9 / 10;
    }
    
    private void FixedUpdate()
    {
        // let spawn it randomly on the map
        if (countdown == 0)
        {
            Vector3 playerPosition = player.GetComponent<Player>().transform.position;
            // first random x axis
            float x;
            float z;
            do
            {
                float x_size = m_Size.x / 2;
                x = Random.Range(-x_size, x_size);
                float z_size = m_Size.z / 2;
                z = Random.Range(-z_size, x_size);
            } while ((Mathf.Abs(playerPosition.x - x) <= 10) && (Mathf.Abs(playerPosition.z - z) <= 10));

            Instantiate(zombie, new Vector3(x, 0, z), Quaternion.identity);

            countdown = spawntime * 60;
        }
        else
        {
            countdown--;
        }
    }
}
