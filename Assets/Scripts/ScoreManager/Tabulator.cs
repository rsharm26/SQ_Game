using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;    // library needed for text field
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tabulator : MonoBehaviour
{
    //  what to load upon winning / losing
    [SerializeField]
    private string winScene; 

    [SerializeField]
    private string lostScene; 

    // backing field & property for lives
    [SerializeField]
    private int lives = 3;
    public int Lives
    {
        get{return lives;}
        set{ lives = value; }
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
    private bool blockDestroyed; 

    public int Collect {get; set;}      // property to track # of collectables

    [SerializeField]
    private int LevelBaseScore = 50; 

    [SerializeField]
    private int BonusPerLife = 10; 
    // property Win, set when player encounters portal (or satisfies other win condition)
    public bool Win {get; set;}

    private void Start()
    {
        startTime = Time.time; 
        Win = false; 
        blockDestroyed = false; 
    }

    private void Update()
    {
        elapsedTime = Time.time - startTime; 
        checkLost();
        checkWin(); 

        if (Collect >= unlockThreshold && !blockDestroyed)      // if player collected enough pickups, destroy the block that stops them from accessing winning portal
        {
            blockDestroyed = true; 
            Destroy(portalBlock); 
        }
    }

    private void checkLost()
    {
        if (lives == 0 || elapsedTime >= allowedTime)   // lose if ran out of lives or time. 
        {
            SceneManager.LoadScene(lostScene);
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
            SceneManager.LoadScene(winScene);
        }
    }
    
}
