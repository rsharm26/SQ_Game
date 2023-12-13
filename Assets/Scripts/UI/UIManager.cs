/* Filename: UIManager.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Rohin Sharma
 * Date: December 13, 2023
 * Description: This file houses a SINGLETON that is responsible for creating a handle to the UIContainer scriptable object.
                This is a relatively simple yet effective solution, means we don't need an instance of the scriptable object per scene.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*  
 * Class: UIManager.
 * Purpose: This class is the singleton to access the UIContainer.
            It is a singleton and relatively simple.
 */
public class UIManager {
    // Attributes, just the UIContainer instance.
    private static UIContainer _instance;

    /*
     * Method: GetInstance() -- Method with no parameters.
     * Description: This method instantiates (if needed) and gets the private UIContainer instance.
                    Again this is the alternative to having to bind UIContainer to a scene.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    public static UIContainer GetInstance() {
        if (_instance == null) {
            _instance = Resources.Load<UIContainer>("UIContainerObject");
        }

        return _instance;
    }
}
