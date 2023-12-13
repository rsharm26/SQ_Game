/* Filename: InLevelOverlayController.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Rohin Sharma
 * Date: December 13, 2023
 * Description: This file houses a monobehavior object that is responsible for managing the in-level overlay view.
                So, this is the code-behind for the overlay the displays lives, collectibles, and time in the actual level.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;



/*  
 * Class: InLevelOverlayController.
 * Purpose: This class is the code-behind for the in-level overlay view.
            It is responsible for setting it up and binding an event to keep the view's data updated.
            Basically, this is a very primitive version of data binding.
 */
public class InLevelOverlayController : MonoBehaviour {
    // Attributes.
    private DynamicGameData _gameData; // Scriptable DTO.

    // This method is included by default in Unity, executes at the start of an object's lifetime (first frame).
    // Treat this like a constructor.
    private void Start() {
        _gameData = GameDataManager.GetInstance(); // Singleton instance handle.
        _gameData.DataUpdated += RefreshBoundData;
        RefreshBoundData(); // Must call once manually so data is set before level starts.
    }


    /*
     * Method: RefreshBoundData() -- Method with no parameters.
     * Description: This method shall update certain UI sub-elements within the in-level overlay.
                    Primitive data binding, we simply update all labels once game-related data is modified.
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

        if (_gameData.CollectiblesFound >= _gameData.CollectibleUnlockThreshold) {
            collectiblesText.style.color = new StyleColor(new Color32(255, 255, 15, 255));
        } else {
            collectiblesText.style.color = new StyleColor(new Color32(202, 189, 62, 255));
        }
    }
}
