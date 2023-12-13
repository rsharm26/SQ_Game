/* Filename: DynamicGameData.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Rohin Sharma
 * Date: December 13, 2023
 * Description: This file houses a scriptable object used to store session-wide data relating to ...
                ... player data (lives, collectibles found, etc.) alongside user-level DB data and a settings option.
                Note the actual scriptable component object lives in Resources.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;



/*  
 * Class: DynamicGameData.
 * Purpose: This class is effectively a DTO for player and user-level dynamic data.
 * It only has properties, either auto or backed, and basic events that external actors can bind to.
 * Note that it is a scriptable object, meaning we're basically using a data container with session persistence.
 * This is convenient compared to regular monobehavior as we don't have to bind it to an object in the scene...
 * ... and it is more convenient than a singleton as we can still serialize.
 */
[CreateAssetMenu(fileName = "../Resources/DynamicGameDataObject", menuName = "ScriptableObjects/DynamicGameData")]
public class DynamicGameData : ScriptableObject
{
    // Backing fields.
    private int _collectiblesFound; // Player's collectible count.
    private int _leaderboardIndex; // Index of the last selected leaderboard option.
    private int _livesRemaining;
    private TimeSpan _remainingTime;
    private bool _win;

    // Properties.
    // Note that many of the properties invoke an event, which effectively acts as an OnPropertyChanged().
    // In other words, we notify externally subscribed entities of any changes so they can act accordingly.
    public int CollectiblesFound { 
        get => _collectiblesFound; 
        set { 
            _collectiblesFound = value; 
            DataUpdated?.Invoke(); 
            } 
    }

    public int LeaderboardIndex { 
        get => _leaderboardIndex; 
        set { 
            _leaderboardIndex = value; 
            LeaderIndexUpdated?.Invoke(); 
            } 
    }

    public int LivesRemaining { 
        get => _livesRemaining; 
        set { 
            _livesRemaining = value; 
            DataUpdated?.Invoke(); 
            } 
    }

    public TimeSpan RemainingTime { 
        get => _remainingTime; 
        set { 
            _remainingTime = value; 
            DataUpdated?.Invoke(); 
            } 
    }

    public bool Win { 
        get => _win; 
        set { 
            _win = value; 
            DataUpdated?.Invoke(); 
            } 
    }

    public int LevelBaseScore { get; set; }
    public int UserID { get; set; }
    public string UserName { get; set; }
    public int BonusPerLife { get; set; }
    public int CollectibleUnlockThreshold { get; set; }


    // Events.
    // Note that DataUpdated is called quite often, hence it should be decoupled from LeaderIndexUpdated (rarely called).
    public event Action DataUpdated; // Called for any subscribers interested in changes to lives, time, collectibles, and win flag.
    public event Action LeaderIndexUpdated; // Called for subscribers listening to leaderboard changes
    
}