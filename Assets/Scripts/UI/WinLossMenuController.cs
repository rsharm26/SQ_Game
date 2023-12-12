using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class WinLossMenuController : MonoBehaviour {
    private Button _levelsButton;
    private Button _leaderButton;
    private Button _menuButton;
    private DynamicGameData _gameData;

    private void Start() {
        VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
        _levelsButton = root.Q<Button>("level-selector-btn");
        _leaderButton = root.Q<Button>("leader-btn");
        _menuButton = root.Q<Button>("menu-btn");

        UIContainer uiContainer = UIManager.GetInstance();

        _levelsButton.clickable.clicked += () => uiContainer.ToggleUIElement(UIType.LevelSelection);
        _leaderButton.clickable.clicked += InvokeLeaderboard;
        _menuButton.clickable.clicked += ReturnToMainMenu;

        _gameData = GameDataManager.GetInstance();
        _gameData.DataUpdated += RefreshBoundData;
        RefreshBoundData(); // Must call once manually so data is set before pause menu opened.
    }

    private void InvokeLeaderboard() {
        _gameData.LeaderboardIndex = int.Parse(Regex.Match(SceneManager.GetActiveScene().name, @"[0-9]").Value) - 1;
        UIManager.GetInstance().ToggleUIElement(UIType.Leaderboard);
    }

    private void ReturnToMainMenu() {
        UIManager.GetInstance().CloseAllActiveElements();
        SceneManager.LoadScene("MainMenu");
    }

    private void RefreshBoundData() {
        Label titleText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("title");
        Label overallScoreText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("overall-score-text");
        Label livesScoreText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("lives-score-text");
        Label collectiblesScoreText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("collectibles-score-text");
        Label timeScoreText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("time-score-text");
        Label baseScoreText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("base-score-text");
        VisualElement pinkDudeImage = this.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("pink-dude");

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

        titleText.text = $"You {winOrLoss}";
        pinkDudeImage.style.backgroundImage = new StyleBackground(pinkDude.texture);
        overallScoreText.text = $"You scored: <u>{overallScore:D3}</u>";
        livesScoreText.text = $"{_gameData.LivesRemaining * _gameData.BonusPerLife:D3}";
        collectiblesScoreText.text = $"{_gameData.CollectiblesFound:D3}";
        timeScoreText.text = $"{Convert.ToInt16(_gameData.RemainingTime.TotalSeconds):D3}";
        baseScoreText.text = $"+ {_gameData.LevelBaseScore:D3}";
    }
}
