using UnityEngine;

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
