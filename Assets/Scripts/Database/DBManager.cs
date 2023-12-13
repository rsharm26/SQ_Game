/* Filename: DBManager.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Rohin Sharma
 * Date: December 13, 2023
 * Description: This file houses a SINGLETON that is responsible for all things database.
                It gives a handle to connect to the database, methods to execute queries, and the ability to close the connection.
                Note it is important to realize that the caller is responsible for closing connection AND exception handling.
 */
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using UnityEngine;



/*  
 * Class: DBManager.
 * Purpose: This class is the singleton to manage all database related operations.
            It is a singleton and relatively simple.
 */
public class DBManager {
    // Attributes.
    private static DBManager _instance;

    // Properties.
    public SqliteConnection Connection { get; private set; } // Note this is convenient for unit testing (public property).


    /*
     * Method: GetInstance() -- Method with no parameters.
     * Description: This method instantiates (if needed) and gets the private DBManager instance.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    public static DBManager GetInstance() {
        if (_instance == null) {
            return _instance = new DBManager();
        }

        return _instance;
    }

    /*
     * Method: OpenDBConnection() -- Method with 1 parameter.
     * Description: This method opens a connection a local SQLite DB file.
                    IMPORTANT to note this will create the file if the DB does not exist.
     * Parameters: string DBName: the database file to open.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    public void OpenDBConnection(string DBName) {
        Connection = new SqliteConnection($"Data Source={DBName}");
        Connection.Open();

        // Run simple initializer to ensure our DB is always in valid state.
        try{ 
            InitDB();
        } catch (SqliteException se) {
            Debug.Log(se.Message);
        }
    }

    /*
     * Method: CloseDBconnection() -- Method with no parameters.
     * Description: This method closes a connection a local SQLite DB file.
                    The caller is responsible for managing this.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    public void CloseDBConnection() {
        Connection?.Close();
    }

    /*
     * Method: ExecuteParamQueryNonReader() -- Method with 2 parameters.
     * Description: This method executes a parameterized query against the currently connected DB.
     * Parameters: string sql: the query to execute;
                   Dictionary<string, object> sqlParams: the query parameters and their values (default null).
     * Outputs: Nothing.
     * Return Values: an integer specifying the number of rows affected (if appropriate).
     */
    public int ExecuteParamQueryNonReader(string sql, Dictionary<string, object> sqlParams = null) {
        int affectedRows = 0;

        if (Connection != null && Connection.State == ConnectionState.Open) {
            using SqliteCommand command = Connection.CreateCommand();
            command.CommandText = sql;

            // Future version make terse using command.Parameters.AddRange().
            if (sqlParams != null) {
                foreach (KeyValuePair<string, object> param in sqlParams) {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            affectedRows = command.ExecuteNonQuery();
        }

        return affectedRows;
    }

    /*
     * Method: ExecuteParamQueryReader() -- Method with 2 parameters.
     * Description: This method executes a parameterized query against the currently connected DB.
     * Parameters: string sql: the query to execute;
                   Dictionary<string, object> sqlParams: the query parameters and their values (default null).
     * Outputs: Nothing.
     * Return Values: an SqliteDataReader object containing data returned by the database.
     * Extra Notes: This is most useful for SELECT statements.
     */
    public SqliteDataReader ExecuteParamQueryReader(string sql, Dictionary<string, object> sqlParams = null) {
        if (Connection != null && Connection.State == ConnectionState.Open) {
            using SqliteCommand command = Connection.CreateCommand();
            command.CommandText = sql;

            // Future version make terse using command.Parameters.AddRange().
            if (sqlParams != null) {
                foreach (KeyValuePair<string, object> param in sqlParams) {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            return command.ExecuteReader();
        }

        return null;
    }

    /*
     * Method: InitDB() -- Method with no parameter.
     * Description: This method executes statements to initialize the DB if appropriate.
                    This ensures valid state whenever the DB is accessed.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
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
