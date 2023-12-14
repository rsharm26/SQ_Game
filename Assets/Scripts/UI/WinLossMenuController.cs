/* Filename: WinLossMenuController.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Rohin Sharma
 * Date: December 13, 2023
 * Description: This file houses a monobehavior object that is responsible for managing win/loss menu view.
                It is relatively simple, mainly sets up data on the view itself and binds to an event that refreshes said data.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;



/*  
 * Class: WinLossMenuController.
 * Purpose: This class is the code-behind for the win/loss menu view.
            It is responsible for setting it up and binding an event to keep the view's data updated.
            Basically, this includes a very primitive version of data binding.
 */
public class WinLossMenuController : MonoBehaviour {
    // Attributes.
    private Button _levelsButton;
    private Button _leaderButton;
    private Button _menuButton;
    private Button _replayButton;
    private DynamicGameData _gameData; // Scriptable DTO.


    // This method is included by default in Unity, executes at the start of an object's lifetime (first frame).
    // Treat this like a constructor.    
    private void Start() {
        VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
        UIContainer uiContainer = UIManager.GetInstance();

        _levelsButton = root.Q<Button>("level-selector-btn");
        _leaderButton = root.Q<Button>("leader-btn");
        _menuButton = root.Q<Button>("menu-btn");
        _replayButton = root.Q<Button>("replay-btn");

        // Bind simple events to each button press.
        _levelsButton.clickable.clicked += () => uiContainer.ToggleUIElement(UIType.LevelSelection);
        _leaderButton.clickable.clicked += InvokeLeaderboard;
        _menuButton.clickable.clicked += ReturnToMainMenu;
        _replayButton.clickable.clicked += ReplayLevel;

        // Bind a data updating event, basically refresh our view when certain GameData fields are updated.
        _gameData = GameDataManager.GetInstance();
        _gameData.DataUpdated += RefreshBoundData;
        RefreshBoundData(); // Must call once manually so data is set before pause menu opened.
    }


    /*
     * Method: InvokeLeaderboard() -- Method with no parameters.
     * Description: This method is called to render the leaderboard UI element.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void InvokeLeaderboard() {
        // Note we adjust gameData index here to trigger the event that will refresh the leaderboard records view.
        _gameData.LeaderboardIndex = int.Parse(Regex.Match(SceneManager.GetActiveScene().name, @"[0-9]").Value) - 1;
        UIManager.GetInstance().ToggleUIElement(UIType.Leaderboard);
    }

    private void ReturnToMainMenu() {
        UIManager.GetInstance().ToggleUIElement(UIType.InLevelExit);
    }

    private void ReplayLevel() {
        UIManager.GetInstance().CloseAllActiveElements();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }        

    /*
     * Method: RefreshBoundData() -- Method with no parameters.
     * Description: This event is called whenever certain GameData fields change, thereby propagating changes to the view.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     * Extra Notes: VERY basic binding...
     */
    private void RefreshBoundData() {
        // Get all the different labels that must be modified.
        Label titleText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("title");
        Label overallScoreText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("overall-score-text");
        Label livesScoreText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("lives-score-text");
        Label collectiblesScoreText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("collectibles-score-text");
        Label timeScoreText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("time-score-text");
        Label baseScoreText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("base-score-text");
        VisualElement pinkDudeImage = this.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("pink-dude");

        // Calculate data for said labels.
        int overallScore =
            _gameData.LevelBaseScore +
            _gameData.CollectiblesFound +
            Convert.ToInt16(_gameData.RemainingTime.TotalSeconds) +
            (_gameData.LivesRemaining * _gameData.BonusPerLife);

        Sprite pinkDude = Resources.Load<Sprite>("PinkDudeSad");
        string winOrLoss = "Lose :(";

        if (_gameData.Win) {
            pinkDude = Resources.Load<Sprite>("PinkDudeHappy");
            winOrLoss = "Win!";
        }

        // Adjust labels/view.
        titleText.text = $"You {winOrLoss}";
        pinkDudeImage.style.backgroundImage = new StyleBackground(pinkDude.texture);
        overallScoreText.text = $"You scored: <u>{overallScore:D3}</u>";
        livesScoreText.text = $"{_gameData.LivesRemaining * _gameData.BonusPerLife:D3}";
        collectiblesScoreText.text = $"{_gameData.CollectiblesFound:D3}";
        timeScoreText.text = $"{Convert.ToInt16(_gameData.RemainingTime.TotalSeconds):D3}";
        baseScoreText.text = $"+ {_gameData.LevelBaseScore:D3}";
    }
}
