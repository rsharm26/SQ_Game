using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

// BASIC, refactor to setup/add buttons to button box via code.
// Query list of all scenes and run regex to parse for "Level #" name.
// Assign these scenes to respective buttons.

public class LevelSelectorManager : MonoBehaviour {
    [SerializeField] private UIDocument _document;

    private Button _exitButton;
    private Button _level1Button;
    private Button _level2Button;
    private Button _level3Button;
    private Button _level4Button;
    private Button _level5Button;

    void Start() {
        VisualElement root = _document.rootVisualElement;
        _exitButton = root.Q<Button>("exit-btn");
        _level1Button = root.Q<Button>("level1-btn");
        _level2Button = root.Q<Button>("level2-btn");
        _level3Button = root.Q<Button>("level3-btn");
        _level4Button = root.Q<Button>("level4-btn");
        _level5Button = root.Q<Button>("level5-btn");

        _exitButton.clickable.clicked += ExitButtonPressed;
        _level1Button.clickable.clicked += () => PlayableButtonPressed(_level1Button);
        _level2Button.clickable.clicked += () => PlayableButtonPressed(_level2Button);
        _level3Button.clickable.clicked += () => PlayableButtonPressed(_level3Button);
        _level4Button.clickable.clicked += () => PlayableButtonPressed(_level4Button);
        _level5Button.clickable.clicked += () => PlayableButtonPressed(_level5Button);

        // EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
    }

    private void ExitButtonPressed() {
        this.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
    }

    private void PlayableButtonPressed(Button pressedButton) {
        string sceneName = "Level " + Regex.Match(pressedButton.name, "[0-9]").Value;
        SceneManager.LoadScene(sceneName);
    }
}
