using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

// Singleton.
public class DBManager : MonoBehaviour {
    private static DBManager _instance;
    public SqliteConnection Connection { get; private set; } // ADJUST to attr later.

    public static DBManager GetInstance() {
        if (_instance != null) {
            return _instance;
        }

        // Does the object containing the DB manager exist?
        _instance = FindObjectOfType<DBManager>();

        if (_instance == null) {
            // Basically, make a gameobject and attach this script to it, store a reference to the added component.
            // Do this as it styncs with Unity lifecycle methods (like OnDestroy()), although not entirely necessary.
            GameObject DBSingelton = new("DBManager");
            _instance = DBSingelton.AddComponent<DBManager>();
            #if !UNITY_INCLUDE_TESTS
            DontDestroyOnLoad(DBSingelton); // Persistent across scenes.
            #endif
        }

        return _instance;
    }

    public void OpenDBConnection(string DBName) {
        Connection = new SqliteConnection($"Data Source={DBName}");
        Connection.Open();

        InitDB();
    }

    public void CloseDBConnection() {
        Connection?.Close();
    }

    // Specific to CREATE/DROP.
    public int ExecuteParamQueryNonReader(string sql, Dictionary<string, object> sqlParams = null) {
        int affectedRows = 0;

        if (Connection != null && Connection.State == ConnectionState.Open) {
            using SqliteCommand command = Connection.CreateCommand();
            command.CommandText = sql;

            // Make terse using command.Parameters.AddRange().
            if (sqlParams != null) {
                foreach (KeyValuePair<string, object> param in sqlParams) {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            affectedRows = command.ExecuteNonQuery();
        }

        return affectedRows;
    }

    // Fine for U/R/D.
    public SqliteDataReader ExecuteParamQueryReader(string sql, Dictionary<string, object> sqlParams = null) {
        if (Connection != null && Connection.State == ConnectionState.Open) {
            using SqliteCommand command = Connection.CreateCommand();
            command.CommandText = sql;

            // Make terse using command.Parameters.AddRange().
            if (sqlParams != null) {
                foreach (KeyValuePair<string, object> param in sqlParams) {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            return command.ExecuteReader();
        }

        return null;
    }

    // Remove gameobject and "reset" singleton.
    private void OnDestroy() {
        if (_instance != null) {
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }

    private void InitDB() {
        ExecuteParamQueryNonReader($@"CREATE TABLE IF NOT EXISTS User (UserID INTEGER PRIMARY KEY NOT NULL, UserName VARCHAR(10) NOT NULL);");
        ExecuteParamQueryNonReader(
            $@"CREATE TABLE IF NOT EXISTS UserScore (" +
                "UserScoreID INTEGER PRIMARY KEY NOT NULL," +
                "UserID INT NOT NULL," +
                "LevelID INT NOT NULL," +
                "Score INT NOT NULL," +
                "CONSTRAINT FK_UserScore_UserID FOREIGN KEY (UserID) REFERENCES User(UserID)" +
              ");"
        );
    }
}
