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

public class LeaderboardManager : MonoBehaviour {
    private Button _exitButton;
    private DropdownField _levelSeclectorDropdown;
    private ScrollView _list;
    private DynamicGameData _gameData;

    void Start() {
        _gameData = GameDataManager.GetInstance();
        VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
        _exitButton = root.Q<Button>("exit-btn");
        _levelSeclectorDropdown = root.Q<DropdownField>("level-select");
        _list = root.Q<ScrollView>("records-list");

        _exitButton.clickable.clicked += ExitButtonPressed;

        _levelSeclectorDropdown.RegisterValueChangedCallback((evt) => {
            Debug.Log($"The value has changed to {evt.newValue}.");
            OnDropdownValueChange(evt);
        });

        InitLevelSelectDropdown();

        _gameData.LeaderIndexUpdated += ReinitLevelDropdown;
    }

    private void ExitButtonPressed() {
        UIManager.GetInstance().ToggleUIElement(UIType.Leaderboard);
    }

    private void InitLevelSelectDropdown() {
        _levelSeclectorDropdown.choices = Enumerable
            .Range(1, 5) // MAGIC.
            .Select((x) => $"Level {x}")
            .ToList();
        _levelSeclectorDropdown.index = 0; // This will trigger the callback below, getting us our records from the DB.
        // This ensures something is present before the user even opens up leaderboard.
    }

    public void ReinitLevelDropdown() {
        _levelSeclectorDropdown.index = _levelSeclectorDropdown.index + 1;
        _levelSeclectorDropdown.index = _gameData.LeaderboardIndex;
    }

    private void OnDropdownValueChange(ChangeEvent<string> evt) {
        _list.Clear();

        List<Tuple<string, string>> records = new List<Tuple<string, string>>();
        DBManager dbManager = DBManager.GetInstance();
        dbManager.OpenDBConnection(DBName: "PixelAndy.db");

        try {
            using SqliteDataReader reader = dbManager.ExecuteParamQueryReader(
                @"SELECT U.UserName, 
                US.Score 
                FROM User U 
                INNER JOIN UserScore US ON US.UserID = U.UserID WHERE US.LevelID = $LevelID 
                ORDER BY US.Score DESC 
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

        VisualElement listRow = new VisualElement();
        listRow.AddToClassList("records-list-header-row");

        if (records.Count == 0) {
            listRow.Add(new Label("No records found"));

            _list.Add(listRow);
        } else {
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
