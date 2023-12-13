/* Filename: MusicManager.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Rohin Sharma
 * Date: December 13, 2023
 * Description: This file houses a SINGLETON that is responsible for creating a handle to the MusicContainer scriptable object.
                This is a relatively simple yet effective solution, means we don't need an instance of the scriptable object per scene.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*  
 * Class: MusicManager.
 * Purpose: This class is the singleton to access the MusicContainer.
            It is a singleton and relatively simple.
 */
public class MusicManager {
    // Attributes, just the MusicContainer instance.
    private static MusicContainer _instance;


    /*
     * Method: GetInstance() -- Method with no parameters.
     * Description: This method instantiates (if needed) and gets the private MusicContainer instance.
                    Again this is the alternative to having to bind MusicContainer to a scene.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    public static MusicContainer GetInstance() {
        if (_instance == null) {
            _instance = Resources.Load<MusicContainer>("MusicContainerObject");
        }

        return _instance;
    }
}
