using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

public class LeaderboardManager : MonoBehaviour {
    [SerializeField] private UIDocument _document;

    private Button _exitButton;
    private DropdownField _levelSeclectorDropdown;
    private ListView _list;

    void Start() {
        VisualElement root = _document.rootVisualElement;
        _exitButton = root.Q<Button>("exit-btn");
        _levelSeclectorDropdown = root.Q<DropdownField>("level-select");
        _list = root.Q<ListView>("records-list");

        _exitButton.clickable.clicked += ExitButtonPressed;

        _levelSeclectorDropdown.RegisterValueChangedCallback((evt) => {
            Debug.Log($"The value has changed to {evt.newValue}.");
            OnDropdownValueChange(evt);
        });

        InitLevelSelectDropdown();
    }

    private void ExitButtonPressed() {
        this.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
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
        _levelSeclectorDropdown.index = 0;
    }

    private void OnDropdownValueChange(ChangeEvent<string> evt) {
        List<string> records = new List<string>();
        DBManager dbManager = DBManager.GetInstance();
        dbManager.OpenDBConnection(DBName: "PixelAndy.db");

        try {
            using SqliteDataReader reader = dbManager.ExecuteParamQueryReader(
                @"SELECT U.UserName, US.Score FROM User U INNER JOIN UserScore US ON US.UserID = U.UserID WHERE US.LevelID = $LevelID ORDER BY US.Score DESC;",
                new Dictionary<string, object>() { { "$LevelID", Regex.Match(evt.newValue, @"[0-9]+").Value } }
            );

            while (reader.Read()) {
                string name = reader.GetString(0);
                int score = reader.GetInt32(1);

                records.Add($"{name} --> {score}");
            }

        } catch (SqliteException se) {
            Debug.Log(se.Message);
            records.Clear();
        }

        _list.itemsSource = records.Count == 0 ? new List<string>() { "No records found..." } : records;
        dbManager.CloseDBConnection();
    }
}
