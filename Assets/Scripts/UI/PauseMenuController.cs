using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour {
    private Button _resumeButton;
    private Button _settingsButton;
    private Button _quitButton;

    private void Start() {
        VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
        _resumeButton = root.Q<Button>("resume-btn");
        _settingsButton = root.Q<Button>("settings-btn");
        _quitButton = root.Q<Button>("quit-btn");

        _resumeButton.clickable.clicked += CloseWindow;
        UIContainer uiContainer = UIManager.GetInstance();
        _settingsButton.clickable.clicked += () => uiContainer.ToggleUIElement(UIType.Settings);
        _quitButton.clickable.clicked += ReturnToMainMenu;
    }

    private void CloseWindow() {
        UIManager.GetInstance().ToggleUIElement(UIType.PauseMenu);
    }

    private void ReturnToMainMenu() {
        CloseWindow(); // See if needed during actual testing.
        SceneManager.LoadScene("MainMenu");
    }
}
