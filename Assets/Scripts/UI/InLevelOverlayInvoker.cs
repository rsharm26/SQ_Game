/* Filename: InLevelOverlayInvoker.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Rohin Sharma
 * Date: December 13, 2023
 * Description: This file houses a monobehavior object that is responsible for simply invoking the in-level overlay.
                It is attached to each level that needs an overlay, can be made extensible to support different overlays in the future.
                This would be achieved by exposing a serialized enum where the developer can specify exact overlay type.
                This is similar to how the music invoker works. 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*  
 * Class: InLevelOverlayInvoker.
 * Purpose: This class simply calls on UIContainer to draw the overlay UI element. 
 */
public class InLevelOverlayInvoker : MonoBehaviour {
    void Start() {
        UIContainer uiContainer = UIManager.GetInstance(); // Singleton instance handle.
        uiContainer.ToggleUIElement(UIType.InLevelOverlay);
    }
}
