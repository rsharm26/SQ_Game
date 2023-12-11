
using UnityEngine;

// Not setting the direction for returning to patrol correctly. 

public class FlyingEnemy : MonoBehaviour
{

    [SerializeField] private float speed;
    private GameObject player; 
    public bool chase = false; 
    private Vector3 origin; 
    private bool atOrigin = true; 

    private Vector3 faceLeft; 
    private Vector3 faceRight; 

    private Animator anim; 

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>(); 
        player = GameObject.FindGameObjectWithTag("Player"); 
        origin = transform.position; 

        if (origin.x < player.transform.position.x) 
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

    // Update is called once per frame
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

    private void Chase()
    {
        // update position towards the player
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        atOrigin = false; 
        Turn(); 
    }

    private void Turn()
    {
        transform.rotation =  Quaternion.Euler((transform.position.x > player.transform.position.x) ? faceLeft : faceRight); 
    }

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

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f; 
        transform.localScale = localScale;
    }

}
