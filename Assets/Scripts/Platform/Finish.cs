using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    // Start is called before the first frame update
    private bool levelCompleted = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Palyer" && !levelCompleted)
        {
            Invoke("CompleteLevel", 2f);    //give time to player
            levelCompleted = true;
            //CompleteLevel();              //go directly
        }
    }

    private void CompleteLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
}
