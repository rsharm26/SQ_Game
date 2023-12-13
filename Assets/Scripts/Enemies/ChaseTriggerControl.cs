using UnityEngine;

/*
    Class:  ChaseTriggerControl 
    Purpose:This script is to be attached to flying enemies in the scene that has a 'stalk' option 
            When player enters a zone, the trigger control sets all included flying enemy's 'chase'
            property to true. Consequently set them to 'false' after the player has exited the zone.  
*/
public class ChaseTriggerControl : MonoBehaviour
{
    [SerializeField] FlyingEnemy[] enemyArray; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            foreach (FlyingEnemy enemy in enemyArray)
            {
                enemy.chase = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            foreach (FlyingEnemy enemy in enemyArray)
            {
                enemy.chase = false; 
            }
        }
    }
}
