using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "../Resources/DynamicGameDataObject", menuName = "ScriptableObjects/DynamicGameData")]
public class DynamicGameData : ScriptableObject {
    // Treat this as basically a DTO for data binding.
    public TimeSpan RemainingTime { get; set; }
    public int LivesRemaining { get; set; }
    public int CollectiblesFound { get; set; }
    public int CollectibleUnlockThreshold { get; set; }

    // Need more.
}