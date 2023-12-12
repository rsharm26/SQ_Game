using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "../Resources/DynamicGameDataObject", menuName = "ScriptableObjects/DynamicGameData")]
public class DynamicGameData : ScriptableObject {
    public event Action DataUpdated;

    // Treat this as basically a DTO for data binding.
    // Backing fields...
    private TimeSpan _remainingTime;
    private int _livesRemaining;
    private int _collectiblesFound;
    private bool _win;

    public TimeSpan RemainingTime { get => _remainingTime; set { _remainingTime = value; DataUpdated?.Invoke(); } }
    public int LivesRemaining { get => _livesRemaining; set { _livesRemaining = value; DataUpdated?.Invoke(); } }
    public int CollectiblesFound { get => _collectiblesFound; set { _collectiblesFound = value; DataUpdated?.Invoke(); } }
    public int CollectibleUnlockThreshold { get; set; }

    public bool Win { get => _win; set { _win = value; DataUpdated?.Invoke(); } }
    public int BonusPerLife { get; set; }
    public int LevelBaseScore { get; set; }
}