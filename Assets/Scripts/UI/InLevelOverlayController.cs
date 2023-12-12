using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InLevelOverlayController : MonoBehaviour {
    private DynamicGameData _gameData;

    private void Start() {
        // JANK, reset color...
        this.GetComponent<UIDocument>().rootVisualElement
            .Q<Label>("collectibles-text").style.color = new StyleColor(new Color32(202, 189, 62, 255));
        _gameData = GameDataManager.GetInstance();
        _gameData.DataUpdated += RefreshBoundData;
        RefreshBoundData(); // Must call once manually so data is set before level starts.
    }

    private void RefreshBoundData() {
        Label timeText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("time-text");
        Label livesText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("lives-text");
        Label collectiblesText = this.GetComponent<UIDocument>().rootVisualElement.Q<Label>("collectibles-text");

        timeText.text = $"{(int)_gameData.RemainingTime.TotalMinutes}:{_gameData.RemainingTime.Seconds:D2}";
        livesText.text = $"x{_gameData.LivesRemaining}";
        collectiblesText.text = $"x{_gameData.CollectiblesFound}";

        if (_gameData.CollectiblesFound >= _gameData.CollectibleUnlockThreshold) {
            collectiblesText.style.color = new StyleColor(new Color32(255, 255, 15, 255));
        }
    }
}
