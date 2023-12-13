using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Assuming the player script is attached to the same GameObject as the player
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                player.RespawnPoint = transform.position; 

                Destroy(gameObject); 
            }
        }
    }
}
