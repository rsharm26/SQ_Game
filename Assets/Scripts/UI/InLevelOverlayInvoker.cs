using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InLevelOverlayInvoker : MonoBehaviour {
    void Start() {
        UIContainer uiContainer = UIManager.GetInstance();
        uiContainer.ToggleUIElement(UIType.InLevelOverlay);
    }
}
