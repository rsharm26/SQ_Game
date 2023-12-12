using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour {
    private Button _resumeButton;
    private Button _settingsButton;
    private Button _quitButton;
    private DynamicGameData _gameData;

    private void Start() {
        VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
        _resumeButton = root.Q<Button>("resume-btn");
        _settingsButton = root.Q<Button>("settings-btn");
        _quitButton = root.Q<Button>("quit-btn");

        _resumeButton.clickable.clicked += CloseWindow;
        UIContainer uiContainer = UIManager.GetInstance();
        _settingsButton.clickable.clicked += () => uiContainer.ToggleUIElement(UIType.Settings);
        _quitButton.clickable.clicked += ReturnToMainMenu;

        _gameData = GameDataManager.GetInstance();
        InitDataBindings();
    }

    private void CloseWindow() {
        UIManager.GetInstance().ToggleUIElement(UIType.PauseMenu);
    }

    private void ReturnToMainMenu() {
        CloseWindow(); // See if needed during actual testing.
        SceneManager.LoadScene("MainMenu");
    }

    private void InitDataBindings() {
        Label timeText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("time-text");
        Label livesText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("lives-text");
        Label collectiblesText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("collectibles-text");

        timeText.bindingPath = "_gameData.RemainingTime";
        livesText.bindingPath = "_gameData.LivesRemaining";
        collectiblesText.bindingPath = "_gameData.CollectiblesFound";

        timeText.text = "Test Time";
        livesText.text = "Test Lives";
        collectiblesText.text = "Test Collectibles";
    }
}
