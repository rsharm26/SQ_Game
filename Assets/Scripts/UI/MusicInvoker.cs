/* Filename: MusicInvoker.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Rohin Sharma
 * Date: December 13, 2023
 * Description: This file houses a monobehavior object that is responsible for simply invoking the music for a level.
                It is attached to each level that needs music and the developer can simply select the tune in the unity editor.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*  
 * Class: InLevelOverlayInvoker.
 * Purpose: This class simply calls on music container to play a certian song.. 
 */
public class MusicInvoker : MonoBehaviour {
    // Attribute that can be modified in the editor.
    [SerializeField]
    private SongType _songType; 

    // This method is included by default in Unity, executes at the start of an object's lifetime (first frame).
    // Treat this like a constructor.
    void Start() {
        MusicManager.GetInstance().PlayMusic(_songType); // Singleton instance handle, play song on start.
    }
}
