/* Filename: CameraController.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Minchul Hwang
 * Date: December 13, 2023
 * Description: When first create a project, 
 *              a function returns the camera to its original position when the character dies.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]private Transform Player;

    // Update is called once per frame
    private void Update()
    {
        transform.position = new Vector3(Player.position.x, Player.position.y, transform.position.z);
    }
}
