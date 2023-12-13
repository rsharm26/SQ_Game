
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;



public class InLevelExitConfirmationController : MonoBehaviour {
    // Attributes (each of the different controls).
    private Button _yesButton;
    private Button _noButton;

    // This method is included by default in Unity, executes at the start of an object's lifetime (first frame).
    // Treat this like a constructor.
    void Start() {
        VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;

        _yesButton = root.Q<Button>("yes-btn");
        _noButton = root.Q<Button>("no-btn");

        // Simple events for when cancel/apply are pressed.
        _yesButton.clickable.clicked += YesButtonPressed;
        _noButton.clickable.clicked += NoButtonPressed;
    }

    private void YesButtonPressed() {
        UIManager.GetInstance().CloseAllActiveElements();
        SceneManager.LoadScene("MainMenu");
    }

    private void NoButtonPressed() {
        UIManager.GetInstance().ToggleUIElement(UIType.InLevelExit);
    }    
}
