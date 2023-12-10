using System.Collections; 
using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    // speed, reference points, and destination index 
    [SerializeField] private float speed = 5f;
    private float runSpeed; 
    [SerializeField] private float idleDuration = 2f;
    [SerializeField] private GameObject[] patrolPoints; 
    private int destinationIndex; 
    private Vector2 currentDestination; 
    [SerializeField] private bool PatrolRight; 
    private float bodyOffset; 
    private Animator anim; 
    private bool isFacingRight = false; 

    private bool isRunning = true; 
    private bool isIdleComplete = false; 
    
    void Start()
    {
        runSpeed = speed; 

        anim = GetComponent<Animator>(); 
        if (PatrolRight) 
        {
            isFacingRight = true;
            destinationIndex = 1; 
            Flip();
        }
        else
        {
            destinationIndex = 0; 
        }

        bodyOffset = GetComponent<Collider2D>().bounds.size.x; 
    }
 

    void Update()
    {
        // retrieve the current destination, calculate remaining distance between enemy & their patrol destination. 
        currentDestination = patrolPoints[destinationIndex].transform.position; 
        float remDistance = Vector2.Distance(transform.position, currentDestination); 

        if (remDistance < 0.02f && isRunning) 
        {
            StartCoroutine(Idle());    
        } 

        if (isIdleComplete)
        {
            destinationIndex = (destinationIndex == 0) ? 1 : 0; // if currently 0, set to 1, else set to 0, update the new destination. 
            currentDestination = patrolPoints[destinationIndex].transform.position;
            isFacingRight = !isFacingRight; 
            Flip(); 

            anim.SetBool("isRunning", true); 
            isRunning = true; 

            speed = runSpeed; 

            isIdleComplete = false; 
        }

        transform.position = Vector2.MoveTowards(transform.position, currentDestination, speed * Time.deltaTime);
    }

    IEnumerator Idle()
    {
        speed = 0;
        anim.SetBool("isRunning", false); 
        isRunning = false; 

        yield return new WaitForSeconds(idleDuration); 

        isIdleComplete = true; 
    }

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f; 
        transform.localScale = localScale;
    }

}
