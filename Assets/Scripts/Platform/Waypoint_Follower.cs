/* Filename: Waypoint_Follower.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Minchul Hwang
 * Date: December 13, 2023
 * Description: This file is a file that causes platforms or various traps to move vertically, horizontally, or diagonally.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint_Follower : MonoBehaviour
{
    [SerializeField] private GameObject[] waypoints;
    private int currentWayPointIndex = 0;

    [SerializeField] private float speed = 2f;

    // This method changes the speed to 0 and makes it go in the opposite direction when the platform arrives at a designated area.
    private void Update()
    {
        if (Vector2.Distance(waypoints[currentWayPointIndex].transform.position, transform.position) < .1f)
        {
            currentWayPointIndex++;
            if(currentWayPointIndex >= waypoints.Length)
            {
                currentWayPointIndex = 0;
            }
        }
        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWayPointIndex].transform.position, Time.deltaTime * speed);
    }
}
