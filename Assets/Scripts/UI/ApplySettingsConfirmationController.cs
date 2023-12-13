
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;



public class ApplySettingsConfirmationController : MonoBehaviour {
    // Attributes (each of the different controls).
    private Button _exitButton;

    // This method is included by default in Unity, executes at the start of an object's lifetime (first frame).
    // Treat this like a constructor.
    void Start() {
        VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;

        _exitButton = root.Q<Button>("exit-btn");

        // Simple events for when cancel/apply are pressed.
        _exitButton.clickable.clicked += ExitButtonPressed;
    }

    private void ExitButtonPressed() {
        UIManager.GetInstance().ToggleUIElement(UIType.ApplySettingsConfirmation);
    }    
}
