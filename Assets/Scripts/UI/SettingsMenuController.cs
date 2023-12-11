using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class SettingsMenuController : MonoBehaviour {
    [SerializeField] private UIDocument _document;

    private Button _cancelButton;
    private Button _applyButton;
    private DropdownField _screenResDropdown;
    private DropdownField _displayModeDropdown;
    private DropdownField _graphicsQualityDropdown;

    // Start is called before the first frame update
    void Start() {
        VisualElement root = _document.rootVisualElement;
        _cancelButton = root.Q<Button>("cancel-btn");
        _applyButton = root.Q<Button>("apply-btn");

        _cancelButton.clickable.clicked += CancelButtonPressed;
        _applyButton.clickable.clicked += ApplyButtonPressed;

        // IMPORTANT, use player prefs as a caching tool.
        // If the user cancels, make it so the dropdowns revert back to player prefs.

        InitResolutionDropdown();
        InitModeDropdown();
        InitGraphicsQualityDropdown();
    }

    private void CancelButtonPressed() {
        this.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
    }

    private void ApplyButtonPressed() {
        Resolution desiredRes = Screen.resolutions[_screenResDropdown.index];
        Screen.SetResolution(desiredRes.width, desiredRes.height, (FullScreenMode)Enum.Parse(typeof(FullScreenMode), _displayModeDropdown.value));
        QualitySettings.SetQualityLevel(_graphicsQualityDropdown.index, true);
    }

    private void InitResolutionDropdown() {
        VisualElement root = _document.rootVisualElement;
        _screenResDropdown = root.Q<DropdownField>("screen-res-dropdown");

        // Use Screen.resolutions, returns an array of structs containing screen res + hz.
        // Format screen resolution exact to Resolution toString, easier comparison in LINQ below.
        _screenResDropdown.choices = Screen.resolutions.Select(res => $"{res.width} x {res.height} @ ({res.refreshRate}Hz)").ToList();
        _screenResDropdown.index = Screen.resolutions
            .Select((res, index) => (res, index)) // Get res + index per each member in Screen.resolutions.
            .First(kv => kv.res.ToString() == Screen.currentResolution.ToString()).index; // Compare all res's against current resolution, find match and return index.
    }

    private void InitModeDropdown() {
        VisualElement root = _document.rootVisualElement;
        _displayModeDropdown = root.Q<DropdownField>("display-mode-dropdown");

        FullScreenMode[] screenModes = (FullScreenMode[])Enum.GetValues(typeof(FullScreenMode));

        _displayModeDropdown.choices = screenModes
            .Select(mode => mode.ToString())
            .ToList();
        _displayModeDropdown.index = screenModes
            .Select((mode, index) => (mode, index))
            .First(kv => kv.mode == Screen.fullScreenMode).index;
    }

    private void InitGraphicsQualityDropdown() {
        VisualElement root = _document.rootVisualElement;
        _graphicsQualityDropdown = root.Q<DropdownField>("graphics-quality-dropdown");

        _graphicsQualityDropdown.choices = QualitySettings.names.ToList();
        _graphicsQualityDropdown.index = QualitySettings.GetQualityLevel();
    }
}
