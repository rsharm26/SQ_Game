using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

public class LevelSelectorManager : MonoBehaviour {
    private Button _exitButton;

    void Start() {
        VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
        _exitButton = root.Q<Button>("exit-btn");

        _exitButton.clickable.clicked += ExitButtonPressed;
    }

    private void ExitButtonPressed() {
        this.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
    }
}
