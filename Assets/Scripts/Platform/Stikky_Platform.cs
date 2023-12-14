/* Filename: Stikky_Platform.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Minchul Hwang
 * Date: December 13, 2023
 * Description: This file is helpful when the platform performs the collider role.
 *              When the player goes up to the platform, it acts as an object and holds it in place.
 *              In addition to fixing the platform, 
 *              it also prevents the player from becoming stuck and unable to move even when hitting a part other than the top of the platform.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stikky_Platform : MonoBehaviour
{
    // The role of OnCollisionEnter2D and OnCollisionExit2D is to anchor the player to the platform.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
    // This trigger allows the platform to be attached only when the player touches the top of the platform, not the sides.

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.transform.SetParent(transform);
        }

    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.transform.SetParent(null);
        }
    }

}
