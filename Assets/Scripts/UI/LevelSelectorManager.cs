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

public class LevelSelectorManager : MonoBehaviour {
    private Button _exitButton;
    private Button _level1Button;
    private Button _level2Button;
    private Button _level3Button;
    private Button _level4Button;
    private Button _level5Button;

    // BASIC, refactor to setup/add buttons to button box via code.
    // Query list of all scenes and run regex to parse for "Level #" name.
    // Assign these scenes to respective buttons.

    void Start() {
        VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
        _exitButton = root.Q<Button>("exit-btn");
        _level1Button = root.Q<Button>("level1-btn");
        _level2Button = root.Q<Button>("level2-btn");
        _level3Button = root.Q<Button>("level3-btn");
        _level4Button = root.Q<Button>("level4-btn");
        _level5Button = root.Q<Button>("level5-btn");

        _exitButton.clickable.clicked += CloseWindow;
        _level1Button.clickable.clicked += () => PlayableButtonPressed(_level1Button);
        _level2Button.clickable.clicked += () => PlayableButtonPressed(_level2Button);
        _level3Button.clickable.clicked += () => PlayableButtonPressed(_level3Button);
        _level4Button.clickable.clicked += () => PlayableButtonPressed(_level4Button);
        _level5Button.clickable.clicked += () => PlayableButtonPressed(_level5Button);

        // EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
    }

    private void CloseWindow() {
        UIManager.GetInstance().ToggleUIElement(UIType.LevelSelection);
    }

    private void PlayableButtonPressed(Button pressedButton) {
        UIManager.GetInstance().CloseAllActiveElements();
        string sceneName = "Level " + Regex.Match(pressedButton.name, "[0-9]").Value;
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1;
    }
}
