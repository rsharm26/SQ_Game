using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "../Resources/DynamicGameDataObject", menuName = "ScriptableObjects/DynamicGameData")]
public class DynamicGameData : ScriptableObject {
    public event Action DataUpdated;
    public event Action LeaderIndexUpdated;

    // Treat this as basically a DTO for data binding.
    // Backing fields...
    private TimeSpan _remainingTime;
    private int _livesRemaining;
    private int _collectiblesFound;
    private bool _win;
    private int _leaderboardIndex;

    public int UserID { get; set; }
    public string UserName { get; set; }
    public TimeSpan RemainingTime { get => _remainingTime; set { _remainingTime = value; DataUpdated?.Invoke(); } }
    public int LivesRemaining { get => _livesRemaining; set { _livesRemaining = value; DataUpdated?.Invoke(); } }
    public int CollectiblesFound { get => _collectiblesFound; set { _collectiblesFound = value; DataUpdated?.Invoke(); } }
    public int CollectibleUnlockThreshold { get; set; }

    public bool Win { get => _win; set { _win = value; DataUpdated?.Invoke(); } }
    public int BonusPerLife { get; set; }
    public int LevelBaseScore { get; set; }

    // THIS SHOULD REALLY NOT BE HERE.
    public int LeaderboardIndex { get => _leaderboardIndex; set { _leaderboardIndex = value; LeaderIndexUpdated?.Invoke(); } }
}