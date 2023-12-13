
using UnityEngine;

/*
    Class: FlyingEnemy 
    Purpose: This script helds the movements instruction for any flying enemy that could chase player when required. 
            The figure lays dormat (naps) when untriggered, until 'chase' is set to true, then the enemy moves to 
            player as long as player remain in the chase trigger area. 
*/

public class FlyingEnemy : MonoBehaviour
{

    [SerializeField] private float speed;
    private GameObject player; 
    public bool chase = false; 
    private Vector3 origin;     // tracks the hibernating origin
    private bool atOrigin = true; 
    private Vector3 faceLeft;   // rotation fields to alter the enemy's animation depending on their position relative to the player
    private Vector3 faceRight; 
    private Animator anim;      // animator instance 


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>(); 
        player = GameObject.FindGameObjectWithTag("Player"); 
        origin = transform.position; 

        if (origin.x < player.transform.position.x)     // determine the animator's look based on whether enemy's origin position is left or right of player. 
        {
            Flip(); 
            faceLeft = new Vector3 (0, 180, 0); 
            faceRight = new Vector3 (0, 0, 0);
        }
        else
        {
            faceLeft = new Vector3 (0, 0, 0); 
            faceRight = new Vector3 (0, 180, 0);
        }
    }


    /*
        Constantly updated logic are here. 
        If chase trigger is set to true, enemy animation transform to flying. 
        If chase trigger is false, and enemy has yet to return to its hibernation origin, call Return() to move back to origin. 
    */
    void Update()
    {
        if (chase)
        {
            Chase(); 
            anim.SetBool("isFlying", true);
        }
        else if (!chase & !atOrigin)
        {
            Return(); 
        }
    }


    /*  
        Function: Chase() 
        Description: This function moves the enemy towards the player, indicate enemy is not at origin, and flips the animation as needed.
    */
    private void Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        atOrigin = false; 
        Turn(); 
    }


    /*
        Function: Turn() 
        Description: This function updates the sprite's direction based on player's relative X position to the enemy. 
    */
    private void Turn()
    {
        transform.rotation =  Quaternion.Euler((transform.position.x > player.transform.position.x) ? faceLeft : faceRight); 
    }


    /*  
        Function: Return() 
        Description: This function returns the enemy to the origin, once enemy has arrived, it will set atOrigin to true, and animation
                     back to the dormant, hibernating one. 
    */
    private void Return()
    {
        transform.position = Vector2.MoveTowards(transform.position, origin, speed * Time.deltaTime);

        transform.rotation =  Quaternion.Euler((transform.position.x > origin.x) ? faceLeft : faceRight); 
        if (transform.position == origin) 
        {
            atOrigin = true;
            anim.SetBool("isFlying", false); 
        }
        
    }


    /*  
        Function: Flip() 
        Description: This function turns the animation 180 degrees on the X-axis. 
    */
    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f; 
        transform.localScale = localScale;
    }

}
