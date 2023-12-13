/* Filename: SettingMenuController.cs
 * Project: SQ Term Project PixelAndysAdventure
 * By: Rohin Sharma
 * Date: December 13, 2023
 * Description: This file houses a monobehavior object that is responsible for managing settings menu view.
                It is relatively simple, mainly sets up data on the view itself and adjusts some player preferences.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;



public class SettingsMenuController : MonoBehaviour {
    // Constants.
    private const float kDefaultVolume = 50.0f;

    // Attributes (each of the different controls).
    private Button _cancelButton;
    private Button _applyButton;
    private DropdownField _screenResDropdown;
    private DropdownField _displayModeDropdown;
    private DropdownField _graphicsQualityDropdown;
    private Slider _volumeSlider;


    // This method is included by default in Unity, executes at the start of an object's lifetime (first frame).
    // Treat this like a constructor.
    void Start() {
        VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;

        _cancelButton = root.Q<Button>("cancel-btn");
        _applyButton = root.Q<Button>("apply-btn");
        _screenResDropdown = root.Q<DropdownField>("screen-res-dropdown");
        _displayModeDropdown = root.Q<DropdownField>("display-mode-dropdown");
        _graphicsQualityDropdown = root.Q<DropdownField>("graphics-quality-dropdown");
        _volumeSlider = root.Q<Slider>("volume-slider");

        // Simple events for when cancel/apply are pressed.
        _cancelButton.clickable.clicked += CancelButtonPressed;
        _applyButton.clickable.clicked += ApplyButtonPressed;

        // Call setup methods.
        InitResolutionDropdown();
        InitModeDropdown();
        InitGraphicsQualityDropdown();
        InitVolumeSlider();
    }

    /*
     * Method: CancelButtonPressed() -- Method with no parameters.
     * Description: This method simply returns the different controls to their previous options using PlayerPrefs.
                    Then it closes the settings window.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void CancelButtonPressed() {
        _screenResDropdown.index = PlayerPrefs.GetInt("screenResIndex");
        _displayModeDropdown.index = PlayerPrefs.GetInt("screenModeIndex");
        _graphicsQualityDropdown.index = PlayerPrefs.GetInt("graphicsIndex");
        _volumeSlider.value = PlayerPrefs.GetFloat("volume");
        UIManager.GetInstance().ToggleUIElement(UIType.Settings);
    }

    /*
     * Method: ApplyButtonPressed() -- Method with no parameters.
     * Description: This method adjusts different settings like screen res and mode, then sets PlayerPrefs.
                    PlayerPrefs caching is important for CancelButtonPressed to work correctly and for volume (not inherently cached).
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void ApplyButtonPressed() {
        Resolution desiredRes = Screen.resolutions[_screenResDropdown.index];
        Screen.SetResolution(desiredRes.width, desiredRes.height, (FullScreenMode)Enum.Parse(typeof(FullScreenMode), _displayModeDropdown.value));
        QualitySettings.SetQualityLevel(_graphicsQualityDropdown.index, true);
        MusicManager.GetInstance().AdjustVolume(_volumeSlider.value / 100); // For some reason must / 100...

        PlayerPrefs.SetInt("screenResIndex", _screenResDropdown.index);
        PlayerPrefs.SetInt("screenModeIndex", _displayModeDropdown.index);
        PlayerPrefs.SetInt("graphicsIndex", _graphicsQualityDropdown.index);
        PlayerPrefs.SetFloat("volume", _volumeSlider.value);
    }

    /*
     * Method: InitResolutionDropdown() -- Method with no parameters.
     * Description: This method sets up the resolution dropdown list and also initializes its PlayerPrefs entry.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void InitResolutionDropdown() {
        // Use Screen.resolutions, returns an array of structs containing screen res + hz.
        // Format screen resolution exact to Resolution toString, easier comparison in LINQ below.
        _screenResDropdown.choices = Screen.resolutions.Select(res => $"{res.width} x {res.height} @ ({res.refreshRate}Hz)").ToList();
        _screenResDropdown.index = Screen.resolutions
            .Select((res, index) => (res, index)) // Get res + index per each member in Screen.resolutions.
            .First(kv => kv.res.ToString() == Screen.currentResolution.ToString()).index; // Compare all res's against current resolution, find match and return index.

        PlayerPrefs.SetInt("screenResIndex", _screenResDropdown.index);
    }

    /*
     * Method: InitModeDropdown() -- Method with no parameters.
     * Description: This method sets up the screen mode dropdown list and its PlayerPrefs entry.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void InitModeDropdown() {
        FullScreenMode[] screenModes = (FullScreenMode[])Enum.GetValues(typeof(FullScreenMode));

        _displayModeDropdown.choices = screenModes
            .Select(mode => mode.ToString())
            .ToList();
        _displayModeDropdown.index = screenModes
            .Select((mode, index) => (mode, index))
            .First(kv => kv.mode == Screen.fullScreenMode).index;

        PlayerPrefs.SetInt("screenModeIndex", _displayModeDropdown.index);
    }

    /*
     * Method: InitGraphicsQualityDropdown() -- Method with no parameters.
     * Description: This method sets up the graphics quality dropdown and its PlayerPrefs entry.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void InitGraphicsQualityDropdown() {
        _graphicsQualityDropdown.choices = QualitySettings.names.ToList();
        _graphicsQualityDropdown.index = QualitySettings.GetQualityLevel();

        PlayerPrefs.SetInt("graphicsIndex", _graphicsQualityDropdown.index);
    }

    /*
     * Method: InitVolumeSlider() -- Method with no parameters.
     * Description: This method sets up the volume slider and sets its PlayerPrefs entry.
     * Parameters: None.
     * Outputs: Nothing.
     * Return Values: Nothing.
     */
    private void InitVolumeSlider() {
        if (PlayerPrefs.HasKey("volume")) {
            _volumeSlider.value = PlayerPrefs.GetFloat("volume");
        } else {
            _volumeSlider.value = kDefaultVolume;
            PlayerPrefs.SetFloat("volume", kDefaultVolume);
        }
    }
}
