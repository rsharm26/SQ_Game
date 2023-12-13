/* Filename: LeaderboardController.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Rohin Sharma
 * Date: December 13, 2023
 * Description: This file houses a monobehavior object that is responsible for managing leaderboard view.
                it is mainly responsible for initializing the view (including a DB call) and has a simple re-init event.
 */
using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Playables;
using UnityEngine.Profiling;
using UnityEngine.UIElements;



/*  
 * Class: LeaderboardController.
 * Purpose: This class is the code-behind for the leaderboard view.
            It is responsible for setting it up and binding an event to keep the view's data updated.
            Basically, this includes a very primitive version of data binding.
 */
public class LeaderboardController : MonoBehaviour {
    // Constants.
    private const int kLevelCount = 4;

    // Attributes.
    // Most of these are the UI elements we need to setup/bind.
    private Button _exitButton;
    private DropdownField _levelSeclectorDropdown;
    private ScrollView _list;
    private DynamicGameData _gameData; // Scriptable DTO.


    // This method is included by default in Unity, executes at the start of an object's lifetime (first frame).
    // Treat this like a constructor.
    void Start() {
        _gameData = GameDataManager.GetInstance();
        VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;

        _exitButton = root.Q<Button>("exit-btn");
        _levelSeclectorDropdown = root.Q<DropdownField>("level-select");
        _list = root.Q<ScrollView>("records-list");

        // Bind event to exit button click.
        _exitButton.clickable.clicked += ExitButtonPressed;

        // Another event for when the dropdown list option changes (must refresh high scores at this point).
        _levelSeclectorDropdown.RegisterValueChangedCallback((evt) => {
            Debug.Log($"The value has changed to {evt.newValue}.");
            OnDropdownValueChange(evt);
        });

        InitLevelSelectDropdown();

        // Final event to re-init, this lets us adjust the leaderboard within a level so when the user opens it...
        // ... it is already on the current level (instead of whatever they selected last).
        _gameData.LeaderIndexUpdated += ReinitLevelDropdown;
    }


    /*
     * Method: ExitButtonPressed() -- Method with no parameters.
     * Description: This method is called whenever the exit button is pressed, basically just closes the leaderboard.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void ExitButtonPressed() {
        UIManager.GetInstance().ToggleUIElement(UIType.Leaderboard);
    }

    /*
     * Method: InitLevelSelectDropdown() -- Method with no parameters.
     * Description: This method is called in Start() and sets up the level select dropdown.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void InitLevelSelectDropdown() {
        _levelSeclectorDropdown.choices = Enumerable
            .Range(1, kLevelCount)
            .Select((x) => $"Level {x}")
            .ToList();
        _levelSeclectorDropdown.index = _gameData.LeaderboardIndex; 
        // This will trigger the callback below, getting us our records from the DB.
        // This ensures something is present before the user even opens up leaderboard.
    }

    /*
     * Method: ReinitLevelDropdown() -- Method with no parameters.
     * Description: This event is called whenever the leaderboard index in gamedata is updated.
                    We basically pull the latest index value from it.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    public void ReinitLevelDropdown() {
        _levelSeclectorDropdown.index = _levelSeclectorDropdown.index + 1; // Incredibly janky but this ensures our incoming _gameData value is indeed new.
        _levelSeclectorDropdown.index = _gameData.LeaderboardIndex; // Triggers event below.
    }

    /*
     * Method: OnDropdownValueChange() -- Method with 1 parameter.
     * Description: This event is called whenever the leaderboard level selection changes.
     * Parameters: ChangeEvent<string> evt: an event that contains old/new values of the changed element (string values).
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void OnDropdownValueChange(ChangeEvent<string> evt) {
        _list.Clear(); // MUST clear list first.

        List<Tuple<string, string>> records = new List<Tuple<string, string>>();
        DBManager dbManager = DBManager.GetInstance();
        dbManager.OpenDBConnection(DBName: "PixelAndy.db");

        // Fetch the in-DB records.
        try {
            using SqliteDataReader reader = dbManager.ExecuteParamQueryReader(
                @"SELECT U.UserName, 
                US.Score 
                FROM User U 
                INNER JOIN UserScore US ON US.UserID = U.UserID WHERE US.LevelID = $LevelID 
                ORDER BY US.Score DESC, U.UserName ASC 
                LIMIT 10;",
                new Dictionary<string, object>() { { "$LevelID", Regex.Match(evt.newValue, @"[0-9]").Value } }
            );

            while (reader.Read()) {
                string name = reader.GetString(0);
                int score = reader.GetInt32(1);

                records.Add(new Tuple<string, string>(name, score.ToString()));
            }

        } catch (SqliteException se) {
            Debug.Log(se.Message);
            records.Clear();
        }

        // Build the user-facing list itself, starting with a header.
        VisualElement listRow = new VisualElement();
        listRow.AddToClassList("records-list-header-row");

        if (records.Count == 0) {
            listRow.Add(new Label("No records found"));

            _list.Add(listRow);
        } else {
            // We know at least one record exists, so add a visualelement into the list per each record.
            // So, we have name + score in a nice format.
            listRow.Add(new Label("<u>Name</u>"));
            listRow.Add(new Label("<u>Score</u>"));
            _list.Add(listRow);

            foreach(Tuple<string, string> record in records) {
                listRow = new VisualElement();
                listRow.AddToClassList("records-list-regular-row");
                
                listRow.Add(new Label($"{record.Item1}"));
                listRow.Add(new Label($"{record.Item2}"));

                _list.Add(listRow);                    
            }
        }

        dbManager.CloseDBConnection();
    }
}
