/* Filename: LevelSelectorManager.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Rohin Sharma
 * Date: December 13, 2023
 * Description: This file houses a monobehavior object that is responsible for managing the level selection view.
 */
using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;



/*  
 * Class: LevelSelectorManager.
 * Purpose: This class is the code-behind for the level selection view.
            It is relatively simple since level selection is static for now.
 */
public class LevelSelectorManager : MonoBehaviour {
    // Attributes.
    // Buttons for now.
    private Button _exitButton;
    private List<Button> _levelButtons;


    // This method is included by default in Unity, executes at the start of an object's lifetime (first frame).
    // Treat this like a constructor.    
    void Start() {
        VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;

        // Simply get all buttons that exist in the level selector UI element and bind an event to them.
        string[] levelButtonNames = new string[] {"level1-btn", "level2-btn", "level3-btn", "level4-btn"};
        _levelButtons = new List<Button>();

        _exitButton = root.Q<Button>("exit-btn");
        _exitButton.clickable.clicked += CloseWindow;

        foreach (string buttonName in levelButtonNames) {
            Button currentButton = root.Q<Button>(buttonName);
            currentButton.clickable.clicked += () => PlayableButtonPressed(currentButton);
            _levelButtons.Add(currentButton);
        }
    }


    /*
     * Method: CloseWindow() -- Method with no parameters.
     * Description: This method is called whenever the exit button is pressed, basically just closes the leaderboard.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void CloseWindow() {
        UIManager.GetInstance().ToggleUIElement(UIType.LevelSelection);
    }


    /*
     * Method: PlayableButtonPressed() -- Method with 1 parameter.
     * Description: This method is called whenever a playable level button is pressed, simply loads the desired scene.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void PlayableButtonPressed(Button pressedButton) {
        UIManager.GetInstance().CloseAllActiveElements(); // Must close all UI elements first.

        // Note this is not the best way to do this, a convenience play as I'm out of time.
        string sceneName = "Level " + Regex.Match(pressedButton.name, "[0-9]").Value;
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1; // Ensure time is not frozen (else game cannot be played).
    }
}
