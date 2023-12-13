/* Filename: PauseMenuController.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Rohin Sharma
 * Date: December 13, 2023
 * Description: This file houses a monobehavior object that is responsible for managing pause menu view.
                It is relatively simple, mainly sets up data on the view itself and binds to an event that refreshes said data.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;



/*  
 * Class: PauseMenuController.
 * Purpose: This class is the code-behind for the pause menu view.
            It is responsible for setting it up and binding an event to keep the view's data updated.
            Basically, this includes a very primitive version of data binding.
 */
public class PauseMenuController : MonoBehaviour {
    // Attributes.
    private Button _resumeButton;
    private Button _settingsButton;
    private Button _quitButton;
    private DynamicGameData _gameData; // Scriptable DTO.

    // This method is included by default in Unity, executes at the start of an object's lifetime (first frame).
    // Treat this like a constructor.
    private void Start() {
        VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
        UIContainer uiContainer = UIManager.GetInstance();

        _resumeButton = root.Q<Button>("resume-btn");
        _settingsButton = root.Q<Button>("settings-btn");
        _quitButton = root.Q<Button>("quit-btn");

        // Bind each button click to an event.
        _resumeButton.clickable.clicked += CloseWindow;
        _settingsButton.clickable.clicked += () => uiContainer.ToggleUIElement(UIType.Settings);
        _quitButton.clickable.clicked += ReturnToMainMenu;

        // Bind another event that refreshes data on this view.
        _gameData = GameDataManager.GetInstance();
        _gameData.DataUpdated += RefreshBoundData;
        RefreshBoundData(); // Must call once manually so data is set before pause menu opened.
    }


    /*
     * Method: CloseWindow() -- Method with no parameters.
     * Description: This method simply calls the UIManager to draw a specific UI element and sets time back to on (so game works).
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void CloseWindow() {
        UIManager.GetInstance().ToggleUIElement(UIType.PauseMenu);
        Time.timeScale = 1;
    }


    /*
     * Method: ReturnToMainMenu() -- Method with no parameters.
     * Description: This method simply closes all UI elements and loads the main menu scene..
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void ReturnToMainMenu() {
        UIManager.GetInstance().ToggleUIElement(UIType.InLevelExit);
    }

    /*
     * Method: RefreshBoundData() -- Method with no parameters.
     * Description: This method is called each time there are updates to the GameData object's specific fields.
                    It then updates the view's labels to reflect these updates (propagates).
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void RefreshBoundData() {
        Label timeText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("time-text");
        Label livesText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("lives-text");
        Label collectiblesText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("collectibles-text");

        timeText.text = $"{(int)_gameData.RemainingTime.TotalMinutes}:{_gameData.RemainingTime.Seconds:D2}";
        livesText.text = $"x{_gameData.LivesRemaining}";
        collectiblesText.text = $"x{_gameData.CollectiblesFound}";

        // If the user has collected enough fruit, change text to signal they can win.
        if (_gameData.CollectiblesFound >= _gameData.CollectibleUnlockThreshold) {
            collectiblesText.style.color = new StyleColor(new Color32(255, 255, 15, 255));
        } else {
            collectiblesText.style.color = new StyleColor(new Color32(255, 255, 255, 255));
        }
    }
}
