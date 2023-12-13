using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Class: CheckPoint 
    Purpose:  This class holds the code behind for checkpoints within the levels, refer to function comments for detail. 
*/
public class CheckPoint : MonoBehaviour
{
    /*
    *	METHOD          : OnTriggerEnter2D() 
    *	DESCRIPTION		:
    *		This method detects when player has encountered a checkpiont, and updates the player's respawn point to the latest 
    *       check point that they have encountered. 
    *	PARAMETERS      :     
    *       Collision2D other: any object that has collided with the checkpoint trigger. 
    *	RETURNS			:           N/A 
    */
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Assuming the player script is attached to the same GameObject as the player
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)     // if it is a player and there is a player. Update player's respawn point, and deactivate this checkpoint. 
            {
                player.RespawnPoint = transform.position; 

                Destroy(gameObject); 
            }
        }
    }
}
