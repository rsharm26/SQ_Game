using System.Collections; 
using UnityEngine;

/*
    Class: PatrolEnemy 
    Purpose: This script holds the movement for patroling enemies. A patrolling enemy travels between way points, remain idle at an end point 
            before turning back and running towards the other destination.  
            A patrol enemy can be set to patrol to either direction. 
*/
public class PatrolEnemy : MonoBehaviour
{
    // speed, reference points, and destination index 
    [SerializeField] private float speed = 5f;
    private float runSpeed; 
    [SerializeField] private float idleDuration = 2f;
    [SerializeField] private GameObject[] patrolPoints;     // holds the way points that enemy travels in between 
    private int destinationIndex; 
    private Vector2 currentDestination; 
    [SerializeField] private bool PatrolRight; 
    private float bodyOffset;   // remain as a field if one wants to leave spacing between walls, using the enemy's body size as a reference. 

    // fields for updating animation states correctly 
    private Animator anim; 
    private bool isFacingRight = false; 

    private bool isRunning = true; 
    private bool isIdleComplete = false; 
    
    /*
        Start is called once upon scene load.  
        Essential fields are assigned here. 
    */
    void Start()
    {
        runSpeed = speed;   // use run speed to keep track of moving speed, as speed is overwritten upon Idle 

        anim = GetComponent<Animator>(); 
        if (PatrolRight) 
        {
            isFacingRight = true;   // set destination and flip image if patrol enemy were to start with patrolling right. 
            destinationIndex = 1; 
            Flip();
        }
        else
        {
            destinationIndex = 0; 
        }

        bodyOffset = GetComponent<Collider2D>().bounds.size.x; 
    }
 

    /*
        Update is called once per frame, movement related fields are updated here. 
    */
    void Update()
    {
        // retrieve the current destination, calculate remaining distance between enemy & their patrol destination. 
        currentDestination = patrolPoints[destinationIndex].transform.position; 
        float remDistance = Vector2.Distance(transform.position, currentDestination); 

        if (remDistance < 0.02f && isRunning) // if enemy has arrived at one endpoint, begin idle state 
        {
            StartCoroutine(Idle());    
        } 

        if (isIdleComplete) // when Idle state has completed, alter the destination, change sprite direction, update animation state. 
        {
            destinationIndex = (destinationIndex == 0) ? 1 : 0; // if currently 0, set to 1, else set to 0, update the new destination. 
            currentDestination = patrolPoints[destinationIndex].transform.position;
            isFacingRight = !isFacingRight; 
            Flip(); 

            anim.SetBool("isRunning", true);    // back to running 
            isRunning = true; 
            speed = runSpeed; 

            isIdleComplete = false; 
        }
        
        // move enemy towards their destination 
        transform.position = Vector2.MoveTowards(transform.position, currentDestination, speed * Time.deltaTime);
    }


    /*
        Function: Idle() 
        Description: This co-routine is called upon a patrolling enemy reaching an end point.  It begin with setting enemy's speed to zero and 
                    starting the idle animation,  then it will wait for an assigned duration, signaling idle is complete. 
        Parameters:    n/a 
        Returns:       n/a 
    */
    IEnumerator Idle()
    {
        speed = 0;
        anim.SetBool("isRunning", false); 
        isRunning = false; 

        yield return new WaitForSeconds(idleDuration); 

        isIdleComplete = true; 
    }


    /*  
        Function: Flip() 
        Description: This function turns the animation 180 degrees on the X-axis. 
        Parameters:    n/a 
        Returns:       n/a 
    */
    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f; 
        transform.localScale = localScale;
    }

}
