using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;    // library needed for text field
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;



// NOTE FROM/FOR RS, current updating of DynamicGameData is bad.
// It is sloppy to update both the tabulator and scriptable object in the same places.
// Attempt an OnPropertyUpdate() using the observer pattern during refactor, this should work.
public class Tabulator : MonoBehaviour
{
    // field to hold DynamicGameData handle.
    DynamicGameData gameData;

    // backing field & property for lives
    [SerializeField]
    private int lives = 3;
    public int Lives
    {
        get{return lives;}
        set{ lives = value; gameData.LivesRemaining = value; }
    }
    
    // fields related to level remaining time
    private float startTime;
    private float elapsedTime; 
    [SerializeField]
    private float allowedTime = 60f;

    private TimeSpan remainingTime => TimeSpan.FromSeconds(allowedTime - elapsedTime); 

    public TimeSpan RemainingTime {get {return remainingTime;} }

    // fields related to collectable / potential portal blocking objects 
    [SerializeField]
    private GameObject portalBlock; 
    [SerializeField]
    private int unlockThreshold = 12; 
    public int UnlockThreshold { get => unlockThreshold; }
    private bool blockDestroyed;

    public int collect;

    public int Collect {get => collect; set { collect = value; gameData.CollectiblesFound = value; } }      // property to track # of collectables

    [SerializeField]
    private int LevelBaseScore = 50; 

    [SerializeField]
    private int BonusPerLife = 10; 
    // property Win, set when player encounters portal (or satisfies other win condition)
    public bool Win {get; set;}

    // field to track if win or loss should be determined.
    private bool shouldCheckWinOrLoss;

    private void Start()
    {
        startTime = Time.time; 
        Win = false;
        blockDestroyed = false;
        shouldCheckWinOrLoss = true;
        collect = 0;

        gameData = GameDataManager.GetInstance();
        gameData.CollectiblesFound = collect;
        gameData.CollectibleUnlockThreshold = unlockThreshold;
        gameData.LivesRemaining = lives;
        gameData.BonusPerLife = BonusPerLife;
        gameData.LevelBaseScore = LevelBaseScore;
    }

    private void Update()
    {
        elapsedTime = Time.time - startTime;
        gameData.RemainingTime = remainingTime;

        if (shouldCheckWinOrLoss) 
        {
            checkWinOrLoss();
        }

        if (Collect >= unlockThreshold && !blockDestroyed)      // if player collected enough pickups, destroy the block that stops them from accessing winning portal
        {
            blockDestroyed = true; 
            Destroy(portalBlock); 
        }
    }

    private void checkWinOrLoss() {
        if (Win || lives == 0 || elapsedTime >= allowedTime) 
        {
            gameData.Win = Win;
            UIManager.GetInstance().ToggleUIElement(UIType.WinLossMenu);
            shouldCheckWinOrLoss = false;
            Time.timeScale = 0;

            if (Win) 
            {
                int overallScore = LevelBaseScore + Collect + Convert.ToInt16(remainingTime.TotalSeconds) + (Lives * BonusPerLife);

                DBManager dBManager = DBManager.GetInstance();
                dBManager.OpenDBConnection("PixelAndy.db");

                try
                {
                    dBManager.ExecuteParamQueryNonReader(
                   @"INSERT INTO UserScore (UserID, LevelID, Score) VALUES " +
                       $@"({gameData.UserID}, {Regex.Match(SceneManager.GetActiveScene().name, "[0-9]").Value}, {overallScore});");
                }
                catch (SqliteException se)
                {
                    Debug.Log(se.Message);
                }

                dBManager.CloseDBConnection();

                //dBManager.ExecuteParamQueryNonReader(
                //    @"INSERT INTO UserScore (UserID, LevelID, Score) VALUES " +
                //        $@"({gameData.UserID}, {Regex.Match(SceneManager.GetActiveScene().name, "[0-9]").Value}, {overallScore});"
            }
        } 
    }

    /*private void checkLost()
    {
        if (lives == 0 || elapsedTime >= allowedTime)   // lose if ran out of lives or time. 
        {
            UIManager.GetInstance().ToggleUIElement(UIType.WinLossMenu);
        }
    }

    private void checkWin()
    {
        if (Win)
        {
            // set all scores, load win scene
            PlayerPrefs.SetInt("TimeBonus", Convert.ToInt16(remainingTime.TotalSeconds)); 
            PlayerPrefs.SetInt("LevelScore", LevelBaseScore); 
            PlayerPrefs.SetInt("CollectBonus", Collect); 
            PlayerPrefs.SetInt("LivesBonus", Lives * BonusPerLife);
            UIManager.GetInstance().ToggleUIElement(UIType.WinLossMenu);
        }
    }*/
    
}
