using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class WinLossMenuController : MonoBehaviour {
    private Button _levelsButton;
    private Button _leaderButton;
    private Button _menuButton;

    private void Start() {
        VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
        _levelsButton = root.Q<Button>("level-selector-btn");
        _leaderButton = root.Q<Button>("leader-btn");
        _menuButton = root.Q<Button>("menu-btn");

        UIContainer uiContainer = UIManager.GetInstance();

        _levelsButton.clickable.clicked += () => uiContainer.ToggleUIElement(UIType.LevelSelection);
        _leaderButton.clickable.clicked += () => uiContainer.ToggleUIElement(UIType.Leaderboard);
        _menuButton.clickable.clicked += ReturnToMainMenu;
    }

    private void ReturnToMainMenu() {
        UIManager.GetInstance().CloseAllActiveElements();
        SceneManager.LoadScene("MainMenu");
    }
}
