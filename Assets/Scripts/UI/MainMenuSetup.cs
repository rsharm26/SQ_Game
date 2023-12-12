using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuSetup : MonoBehaviour {
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;
    DynamicGameData _gameData;

    private void Start() {
        _gameData = GameDataManager.GetInstance();
        VisualElement root = _document.rootVisualElement; // UXML (cavnas that you can throw elements on).
        root.Clear(); // Clear/re-draw for onvalidate purposes.
        root.styleSheets.Add(_styleSheet);

        // Define a few containers (parent first, children after).
        VisualElement container = Create("container");
        VisualElement titleBox = Create("title-box");
        VisualElement controlBox = Create("control-box");
        VisualElement frostedPanel = Create("frosted-panel");

        // Title.
        Label gameTitle = Create<Label>("game-title");
        gameTitle.text = "Pixel Andy's\n<u>Adventure</u>";

        // Textbox for user's name.
        TextField userNameInput = Create<TextField>("user-name-input");
        userNameInput.maxLength = 10;
        userNameInput.label = "Enter your name: ";
        userNameInput.value = _gameData.UserName;

        // Button array.
        Button playBtn = Create<Button>("play-btn");
        playBtn.text = "PLAY";
        playBtn.SetEnabled(!string.IsNullOrEmpty(_gameData.UserName)); // A bool for active player would be better.

        Button leaderBtn = Create<Button>("leader-btn");
        leaderBtn.text = "LEADERBOARD";

        Button settingsBtn = Create<Button>("settings-btn");
        settingsBtn.text = "SETTINGS";

        Button quitBtn = Create<Button>("quit-btn");
        quitBtn.text = "QUIT";

        // Studio Attribution.
        Label studioAttribution = Create<Label>("studio-attr");
        studioAttribution.text = "By Order66";

        // Bind both control boxes to the container.
        container.Add(frostedPanel);
        container.Add(titleBox);
        container.Add(controlBox);
        container.Add(studioAttribution);

        // Bind specific elements to their parents.
        titleBox.Add(gameTitle);
        controlBox.Add(userNameInput);
        controlBox.Add(playBtn);
        controlBox.Add(leaderBtn);
        controlBox.Add(settingsBtn);
        controlBox.Add(quitBtn);

        // Bind the whole thing to the entire canvas.
        root.Add(container);

        // Bind events to each button the main menu.
        UIContainer uiContainer = UIManager.GetInstance();
        playBtn.clickable.clicked += PlayButtonPressed;
        leaderBtn.clickable.clicked += () => uiContainer.ToggleUIElement(UIType.Leaderboard);
        settingsBtn.clickable.clicked += () => uiContainer.ToggleUIElement(UIType.Settings);
        quitBtn.clickable.clicked += QuitButtonPressed;

        userNameInput.RegisterValueChangedCallback((evt) => OnTextFieldValueChanged(evt));
    }

    // Helpers to create visual elements with class name.
    private VisualElement Create(string className) {
        return Create<VisualElement>(className);
    }

    private T Create<T>(string className) where T : VisualElement, new() {
        T element = new();
        element.AddToClassList(className);
        return element;
    }

    private void OnTextFieldValueChanged(ChangeEvent<string> evt) {
        VisualElement root = _document.rootVisualElement;

        if (evt.newValue.Trim().Length != 0) {
            root.Q<Button>(className: "play-btn")?.SetEnabled(true);    
        } else {
            root.Q<Button>(className: "play-btn")?.SetEnabled(false);
        }
    }

    private void PlayButtonPressed() {
        VisualElement root = _document.rootVisualElement;
        string name = root.Q<TextField>(className: "user-name-input").text.Trim();

        DBManager dbManager = DBManager.GetInstance();
        dbManager.OpenDBConnection("PixelAndy.db");

        using SqliteDataReader reader = dbManager.ExecuteParamQueryReader(
            $@"SELECT UserID FROM User WHERE UserName = $name;",
            new Dictionary<string, object> {
                { "$name", name }
            }
        );

        int userID = 0;

        if (!reader.Read()) {
            using SqliteDataReader insertReader = dbManager.ExecuteParamQueryReader(
                $@"INSERT INTO User (UserName) VALUES ($name);
                   SELECT last_insert_rowid();",
                new Dictionary<string, object> {
                    { "$name", name }
                }
            );

            if (insertReader.Read()) {
                userID = insertReader.GetInt32(0);
            }
        } else {
            userID = reader.GetInt32(0);
        }

        _gameData.UserID = userID;
        _gameData.UserName = name;

        UIContainer uiContainer = UIManager.GetInstance();
        uiContainer.ToggleUIElement(UIType.LevelSelection);
    }

    private void QuitButtonPressed() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Exit in development.
        #endif

        Application.Quit(); // Exit in production.
    }
}
