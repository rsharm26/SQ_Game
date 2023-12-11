using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuSetup : MonoBehaviour {
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;

    private void Start() {
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

        // Button array.
        Button playBtn = Create<Button>("play-btn");
        playBtn.text = "PLAY";

        Button leaderBtn = Create<Button>("leader-btn");
        leaderBtn.text = "LEADERBOARD";

        Button settingsBtn = Create<Button>("settings-btn");
        settingsBtn.text = "SETTINGS";

        Button quitBtn = Create<Button>("quit-btn");
        quitBtn.text = "QUIT";

        // TESTING.
        TextField userNameInput = Create<TextField>("user-name-input");
        userNameInput.maxLength = 10;
        userNameInput.label = "Enter your name: ";

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
        playBtn.clickable.clicked += () => uiContainer.InvokeUIElement(UIType.LevelSelection);
        leaderBtn.clickable.clicked += () => uiContainer.InvokeUIElement(UIType.Leaderboard);
        settingsBtn.clickable.clicked += () => uiContainer.InvokeUIElement(UIType.Settings);
        quitBtn.clickable.clicked += QuitButtonPressed;

        // PENDING REMOVAL.
        InitDB();
    }

    // Helpers to create visual elements with class name.
    private VisualElement Create(string className) {
        return Create<VisualElement>(className);
    }

    private T Create<T>(string className) where T : VisualElement, new() {
        T element = new T();
        element.AddToClassList(className);
        return element;
    }

    private void QuitButtonPressed() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Exit in development.
        #endif

        Application.Quit(); // Exit in production.
    }

    // BAD, move this back into DB manager, can execute every time a connection is open (not too expensive).
    private void InitDB() {
        DBManager dbManager = DBManager.GetInstance();
        dbManager.OpenDBConnection(DBName: "PixelAndy.db");

        dbManager.ExecuteParamQueryNonReader($@"CREATE TABLE IF NOT EXISTS User (UserID INTEGER PRIMARY KEY NOT NULL, UserName VARCHAR(10) NOT NULL);");
        dbManager.ExecuteParamQueryNonReader(
            $@"CREATE TABLE IF NOT EXISTS UserScore (" + 
                "UserScoreID INTEGER PRIMARY KEY NOT NULL," +
                "UserID INT NOT NULL," +
                "LevelID INT NOT NULL," +
                "Score INT NOT NULL," +
                "CONSTRAINT FK_UserScore_UserID FOREIGN KEY (UserID) REFERENCES User(UserID)" +
              ");"
        );

        dbManager.CloseDBConnection();
    }
}
