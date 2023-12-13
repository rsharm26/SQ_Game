using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;    // library needed for text field
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;


/*
    Class: Tabulator 
    Purpose: keep track of fields that are related to player performance, including score, collectable, remaining time, life bonus. 
            This script also updates the database upon player completing a level. 
*/
public class Tabulator : MonoBehaviour
{
    DynamicGameData gameData;   // field to hold DynamicGameData handle.

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
    private float allowedTime = 60f;        // allowed level time is default to 60, unless changed from editor

    private TimeSpan remainingTime => TimeSpan.FromSeconds(allowedTime - elapsedTime); 

    public TimeSpan RemainingTime {get {return remainingTime;} }


    // fields related to collectable / potential portal blocking objects 
    [SerializeField]
    private GameObject portalBlock; 
    [SerializeField]
    private int unlockThreshold = 12;   // # of collectables required to unlock a level is set to 12 unless otherwise specified 
    public int UnlockThreshold { get => unlockThreshold; }
    private bool blockDestroyed;

    public int collect;
    public int Collect {get => collect; set { collect = value; gameData.CollectiblesFound = value; } }      // property to track # of collectables

    // score calculation related fields
    [SerializeField]
    private int LevelBaseScore = 50;    // base score for completing a level 

    [SerializeField]
    private int BonusPerLife = 10;      // bonus for each life remaining 
    // property Win, set when player encounters portal (or satisfies other win condition)
    public bool Win {get; set;}

    // field to track if win or loss should be determined.
    private bool shouldCheckWinOrLoss;


    /*
    Start is called once from scene loading. essential fields are set here. 
    */
    private void Start()
    {
        // record starting time, ensuring portal block is existent, collectable is 0, win state is false. 
        startTime = Time.time; 
        Win = false;
        blockDestroyed = false;
        shouldCheckWinOrLoss = true;
        collect = 0;

        // update all fields for gameData.
        gameData = GameDataManager.GetInstance();
        gameData.CollectiblesFound = collect;
        gameData.CollectibleUnlockThreshold = unlockThreshold;
        gameData.LivesRemaining = lives;
        gameData.BonusPerLife = BonusPerLife;
        gameData.LevelBaseScore = LevelBaseScore;
    }


    /*
    Update is called per frame, constantly updated actions are performed here. 
    */
    private void Update()
    {
        // update remaining time. 
        elapsedTime = Time.time - startTime;
        gameData.RemainingTime = remainingTime;

        if (shouldCheckWinOrLoss)   // this block will stop being called when a win/loss menu is called 
        {
            checkWinOrLoss();
        }

        if (Collect >= unlockThreshold && !blockDestroyed)      // if player collected enough pickups, destroy the block that stops them from accessing winning portal
        {
            blockDestroyed = true; // boolean is here to ensure this block of code is not executed more than once. 
            Destroy(portalBlock); 
        }
    }


    /*
        Function:       checkWinOrLoss() 
        Description:    This function is called per frame and determine whether any winning or losing condition has been met. 
                        In the case of lives has ran out, time has ran out,  timescale will be set to 0 to retain the current time record. 
                        In the case of winning, the current overall score will be updated to the game's database. 
        Parameters:    n/a 
        Returns:       n/a 
    */
    private void checkWinOrLoss() {
        if (Win || lives == 0 || elapsedTime >= allowedTime) 
        {
            gameData.Win = Win;
            UIManager.GetInstance().ToggleUIElement(UIType.WinLossMenu);
            Time.timeScale = 0;
            shouldCheckWinOrLoss = false; // Must set to false otherwise the menu will constantly redraw itself.

            if (Win) 
            {
                int overallScore = LevelBaseScore + Collect + Convert.ToInt16(remainingTime.TotalSeconds) + (Lives * BonusPerLife);

                DBManager dBManager = DBManager.GetInstance();
                dBManager.OpenDBConnection("PixelAndy.db");

                dBManager.ExecuteParamQueryNonReader(
                    @"INSERT INTO UserScore (UserID, LevelID, Score) VALUES " +
                        $@"({gameData.UserID}, {Regex.Match(SceneManager.GetActiveScene().name, "[0-9]").Value}, {overallScore});"
                );

                dBManager.CloseDBConnection();
            }
        } 
    }
}
