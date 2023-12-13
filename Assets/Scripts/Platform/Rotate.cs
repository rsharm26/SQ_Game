/* Filename: Rotate.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Minchul Hwang
 * Date: December 13, 2023
 * Description: This file serves to rotate the chainsaw in the ship.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float speed = 2f;

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(0, 0, 360 * speed * Time.deltaTime);
    }
}
