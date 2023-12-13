using System.Collections;
using System.Collections.Generic;
/* Filename: MainMenuSetup.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Rohin Sharma
 * Date: December 13, 2023
 * Description: This file houses a monobehavior object that is responsible for managing the main menu overlay.
                Note this is different from every other UI element as it builds itself completely from code.
                Did this as a test and because main menu is static.
 */
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UIElements;

/*
 * Class: MainMenuSetup.
 * Purpose: This class is the code-behind for the main menu overlay.
            It builds itself via code.
 */
public class MainMenuSetup : MonoBehaviour {
    // Constants.
    private const int kMaxNameLength = 10;

    // Attributes.
    [SerializeField] private UIDocument _document; // Expose a basically empty UI document (canvas).
    [SerializeField] private StyleSheet _styleSheet; // Stylesheet to use.
    DynamicGameData _gameData; // Scriptable DTO.

    // This method is included by default in Unity, executes at the start of an object's lifetime (first frame).
    // Treat this like a constructor.
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
        userNameInput.maxLength = kMaxNameLength;
        userNameInput.label = "Enter your name: ";
        userNameInput.value = _gameData.UserName; // Get cached name if possible.

        // Button array.
        Button playBtn = Create<Button>("play-btn");
        playBtn.text = "PLAY";
        playBtn.SetEnabled(!string.IsNullOrEmpty(_gameData.UserName)); // A bool for active player would be better, todo in future.

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

        // Event specific to validating user name entry.
        userNameInput.RegisterValueChangedCallback((evt) => OnTextFieldValueChanged(evt));
    }

    /*
     * Method: Create() -- Method with 1 parameter.
     * Description: This method is called as a helper to create a visual element with a specific class.
     * Parameters: string className: the class name to use (for USS).
     * Outputs: Nothing.
     * Return Values: a VisualElement specified by the caller.
     * Extra Notes: VisualElement is the generic class for Button, Label, List, etc. thus is encapsulates most elements.
     */
    private VisualElement Create(string className) {
        return Create<VisualElement>(className);
    }

    /*
     * Method: Create<T>() -- Template method with 1 parameter.
     * Description: This method is called as a helper to create a visual element of a specific type with a specific class.
     * Parameters: string className: the class name to use (for USS).
     * Outputs: Nothing.
     * Return Values: a VisualElement of T (sub-type) specified by the caller.
     * Extra Notes: Same as Create() above, logic here is for any generic visual container (basically a div) we can use Create() above.
                    Else, use this one for the exact visual element required.
     */
    private T Create<T>(string className) where T : VisualElement, new() {
        T element = new();
        element.AddToClassList(className);
        return element;
    }

    /*
     * Method: OnTextFieldValueChanged() -- Method with 1 parameter.
     * Description: This event is called whenever the user name text input box changes.
     * Parameters: ChangeEvent<string> evt: an event that contains old/new values of the changed element (string values).
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void OnTextFieldValueChanged(ChangeEvent<string> evt) {
        VisualElement root = _document.rootVisualElement;

        // Only need to ensure it is not empty.
        if (evt.newValue.Trim().Length != 0) {
            root.Q<Button>(className: "play-btn")?.SetEnabled(true);
        } else {
            root.Q<Button>(className: "play-btn")?.SetEnabled(false);
        }
    }

    /*
     * Method: PlayButtonPressed() -- Method with no parameters.
     * Description: This method is called whenever the play button is pressed.
                    It basically gets a handle on the user's existing DB userID, else creates one for them.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void PlayButtonPressed() {
        VisualElement root = _document.rootVisualElement;
        string name = root.Q<TextField>(className: "user-name-input").text.Trim();

        DBManager dbManager = DBManager.GetInstance();
        dbManager.OpenDBConnection("PixelAndy.db");

        // Begin by checking if the user already exists in the User table.
        try {
            using SqliteDataReader reader = dbManager.ExecuteParamQueryReader(
                $@"SELECT UserID FROM User WHERE UserName = $name;",
                new Dictionary<string, object> {
                    { "$name", name }
                }
            );

            int userID = 0;

            // If they do not, add them into the table and get the userID back.
            // Note that userID is PK + AUTO_INCREMENT, best to read it back.
            if (!reader.Read()) {
                using SqliteDataReader insertReader = dbManager.ExecuteParamQueryReader(
                    $@"INSERT INTO User (UserName) VALUES ($name); 
                    SELECT last_insert_rowid();",
                    new Dictionary<string, object> {
                        { "$name", name }
                    }
                );

                // If something came back from last_insert_rowid(), get it and set it as userID.
                if (insertReader.Read()) {
                    userID = insertReader.GetInt32(0);
                }
            } else {
                // If they exist, just get their ID.
                userID = reader.GetInt32(0);
            }

            _gameData.UserID = userID;
            _gameData.UserName = name;

            UIContainer uiContainer = UIManager.GetInstance();
            uiContainer.ToggleUIElement(UIType.LevelSelection);
        } catch (SqliteException se) {
            Debug.Log(se.Message);
        }
    }


    /*
     * Method: QuitButtonPressed() -- Method with no parameters.
     * Description: This method is called whenever the quit button is pressed.
                    It simply exits the game.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void QuitButtonPressed() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Exit in development.
        #endif

        Application.Quit(); // Exit in production.
    }
}
